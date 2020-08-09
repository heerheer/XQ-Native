using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Forms =  System.Windows.Forms;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MenuItem = System.Windows.Controls.MenuItem;
using System.Reflection;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Panuon.UI.Silver;

namespace XQNative_Console
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public void ShowW()
        {
            this.Show();
        }
        private string QQ = "";

        private string cqpluginsPath = Directory.GetCurrentDirectory() + "\\CQPlugins\\";

        public ObservableCollection<CQPlugin> CQPlugins { get; set; } = new ObservableCollection<CQPlugin>();//防止崩

        public ObservableCollection<MenuItem> CQPluginsMenu { get; set; } = new ObservableCollection<MenuItem>();//防止崩

        public ObservableCollection<XQNC_Log> Logs { get; set; } = new ObservableCollection<XQNC_Log>();//日志

        private bool autoScroll { get; set; } = true;

        #region 委托

        private delegate string GetAppInfo();

        private delegate int CQInitizlize(int authCode);

        public delegate int CQMenu();

        public delegate int CQeventEnable();

        public delegate int CQeventDisable();

        private delegate int CQGroupMsg(int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, string msg, int font);

        private delegate int CQPrivateMsg(int subType, int msgId, long fromQQ, string msg, int font);

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                WebClient c = new WebClient();
                var unjson = c.DownloadString("https://service-cbcj98qu-1252561238.gz.apigw.tencentcs.com/release/xqnative_update");
                //MessageBoxX.Show(Regex.Unescape(unjson)) ;

                JObject json = JsonConvert.DeserializeObject<JObject>( Regex.Unescape(unjson).Trim('\"'));

                if (json["latest"].ToString() != XQNativeCaller.Version)
                {
                    MessageBoxX.Show(json["new"].ToString(), "新的更新已经降临-" + json["latest"].ToString());
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


            Plugins_Load();
            ////处理ContextMenu
            //foreach (var item in CQPlugins)
            //{
            //    MenuItem menuitem = new MenuItem();
            //    menuitem.Header = item.PluginName;
            //    //读取menu
            //    foreach (var mnode in item.MenuFuncs)
            //    {
            //        MenuItem callmenu = new MenuItem();
            //        callmenu.Header = mnode.Name;
            //        callmenu.Tag = item;
            //        callmenu.Click += Callmenu_Click;
            //        menuitem.Items.Add(callmenu);
            //    }
            //    //添加默认菜单
            //    MenuItem calldetail = new MenuItem();
            //    calldetail.Header = "应用详情";
            //    menuitem.Items.Add(calldetail);
            //    CQPluginsMenu.Add(menuitem);
            //}
        }

        public async void Plugins_Load()
        {
            CQPlugins = new ObservableCollection<CQPlugin>();
            await Task.Factory.StartNew(() =>
            {
                WriteLog("Info", "开始搜索CQ插件...");
                var cqdir = new DirectoryInfo(cqpluginsPath);
                if (!cqdir.Exists)
                    cqdir.Create();//创建默认CQPlugins目录
                var cpaths = cqdir.EnumerateDirectories();
                cpaths = cpaths.Where(s => s.Name != "data");
                WriteLog("Info", $"目录下共有{cpaths.Count()}个CQ插件");
                //读取所有子目录
                foreach (var item in cpaths)
                {
                    var dllpath = System.IO.Path.Combine(item.FullName, "app.dll");
                    var jsonpath = System.IO.Path.Combine(item.FullName, "app.json");
                    WriteLog("Info", $"读取应用{item.Name}");

                    if (!File.Exists(dllpath))
                    {
                        WriteLog("Error", $"找不到文件 {dllpath}");
                        WriteLog("Error", $"读取应用{item.Name}失败");
                        continue;
                    }

                    var tPlugin = new CQPlugin();

                    if (!File.Exists(jsonpath))
                    {
                        WriteLog("Tip", $"请注意尽快添加应用 {item.Name} 的正确的Json文件");
                        WriteLog("Tip", $"缺少Json文件暂时不会导致读取失败。");

                        tPlugin.PluginAuthor = "未知作者";
                        tPlugin.PluginVersion = "未知版本";
                        tPlugin.PluginName = "";

                    }
                    else
                    {
                        //解析Json得到对应事件函数
                        var json = JsonDoctor.DealJson(jsonpath);
                        WriteLog("Info", "对Json一顿操作猛如虎");
                        JObject jo = JsonConvert.DeserializeObject<JObject>(json);

                        tPlugin.PluginAuthor = jo["author"].ToString();
                        tPlugin.PluginVersion = jo["version"].ToString();
                        tPlugin.PluginName = jo["name"].ToString();

                        if (jo.ContainsKey("menu"))
                        {
                            //解析Menu
                            foreach (JObject menun in jo["menu"])
                            {
                                tPlugin.MenuFuncs.Add(new CQPlugin.MenuNode()
                                {
                                    Function = menun["function"].ToString(),
                                    Name = menun["name"].ToString()
                                });
                            }
                        }
                    }

                    WriteLog("Info", $"正在读取{dllpath}");

                    //读取都成功
                    var handle = Kernel32.LoadLibraryA(dllpath);
                    if (handle.ToInt32() == 0)
                    {
                        WriteLog("Error", $"应用 {item.Name} 返回句柄为0");
                        WriteLog("Error", $"读取应用 {item.Name}失败");
                        continue;
                    }
                    WriteLog("Info", $"读取DLL成功，句柄为 {handle.ToInt32()}");
                    var appinfohandle = Kernel32.GetProcAddress(handle, "AppInfo");
                    if (appinfohandle.ToInt32() == 0)
                    {
                        WriteLog("Error", $"应用 {item.Name} AppInfo函数错误");
                        WriteLog("Error", $"读取应用 {item.Name}失败");
                        continue;
                    }
                    WriteLog("Info", $"读取AppInfo成功，句柄为 {appinfohandle}");

                    var getappid = (GetAppInfo)Marshal.GetDelegateForFunctionPointer(appinfohandle, typeof(GetAppInfo));
                    var appid = getappid.Invoke().Split(',')?[1];
                    if (appid != item.Name)
                    {
                        WriteLog("Tip", $"AppInfo内信息与目录名不一致");
                        WriteLog("Tip", $"请尽量保持一致");
                    }

                    tPlugin.Handle = handle;
                    tPlugin.PluginID = appid == "" ? Path.GetFileNameWithoutExtension(dllpath) : appid;
                    tPlugin.PluginPath = dllpath;

                    if (tPlugin.PluginName == "")
                        tPlugin.PluginName = tPlugin.PluginID;

                    CQPlugins.Add(tPlugin);//加入MainWindow下的列表
                    WriteLog("Info", $"应用 {appid} 已加载成功");
                }
            });
            await Task.Delay(2000);
            Plugins_Initialiaze();
        }


        private void Callmenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = (MenuItem)sender;
            CQPlugin cqp = (CQPlugin)((MenuItem)sender).Tag;
            var func = cqp.MenuFuncs.Find(s => s.Name == menuitem.Header.ToString()).Function;
            WriteLog("Info", $"呼出应用Menu{menuitem.Header.ToString()},函数为{func}");
            IntPtr handle = Kernel32.GetProcAddress(cqp.Handle,func  );
            if(handle.ToInt32() == 0)
            {
                MessageBox.Show("有毒吧,句柄为0");
            }
            else
            {
                CQMenu cqmenu = (CQMenu)Marshal.GetDelegateForFunctionPointer(handle, typeof(CQMenu));
                cqmenu.Invoke();
            }
        }

        public async void Plugins_Initialiaze(string qq = "")
        {
            QQ = qq;
            await Task.Factory.StartNew(() =>
            {
                foreach (var item in CQPlugins)
                {
                    WriteLog("Info", $"正在初始化应用{item.PluginName}");
                    var dhandle = Kernel32.GetProcAddress(item.Handle, "Initialize");
                    if (dhandle.ToInt32() == 0)
                    {
                        WriteLog("Error", $"对{item.PluginName}调用Initialize函数失败");
                        continue;
                    }
                    var cqInitizlize = (CQInitizlize)Marshal.GetDelegateForFunctionPointer(dhandle, typeof(CQInitizlize));
                    //WriteLog("Info", $"Delegate Done{QQ}");

                    cqInitizlize.Invoke(item.AuthCode);
                    WriteLog("Info", $"应用{item.PluginName}初始化成功");
                }
            });

            Plugins_StartUp();
        }


        public async void Plugins_Disable()
        {
            await Task.Factory.StartNew(() =>
            {
                foreach (var item in CQPlugins)
                {
                    WriteLog("Info", $"[应用启动]{item.PluginName}");
                    var dhandle = Kernel32.GetProcAddress(item.Handle, item._eventDisable);
                    if (dhandle.ToInt32() == 0)
                    {
                        WriteLog("Error", $"对{item.PluginName}调用{item._eventDisable}函数失败");
                        continue;
                    }
                    var cqe = (CQeventDisable)Marshal.GetDelegateForFunctionPointer(dhandle, typeof(CQeventDisable));
                    cqe.Invoke();
                    WriteLog("Info", $"[私聊消息]已转发至{item.PluginName}");
                }
            });
        }

        public async void Plugins_StartUp()
        {
            await Task.Factory.StartNew(() =>
            {
                foreach (var item in CQPlugins)
                {
                    WriteLog("Info", $"[应用预启动]{item.PluginName}");
                    var dhandle = Kernel32.GetProcAddress(item.Handle, item._eventStartUp);
                    if (dhandle.ToInt32() == 0)
                    {
                        WriteLog("Error", $"对{item.PluginName}调用{item._eventStartUp}函数失败");
                        continue;
                    }
                    var cqe = (CQeventEnable)Marshal.GetDelegateForFunctionPointer(dhandle, typeof(CQeventEnable));
                    cqe.Invoke();  
                }
            });
            Plugins_Enable();
        }

        public async void Plugins_Enable()
        {
            await Task.Factory.StartNew(() =>
            {
                foreach (var item in CQPlugins)
                {
                    WriteLog("Info", $"[应用启动]{item.PluginName}");
                    var dhandle = Kernel32.GetProcAddress(item.Handle, item._eventEnable);
                    if (dhandle.ToInt32() == 0)
                    {
                        WriteLog("Error", $"对{item.PluginName}调用{item._eventEnable}函数失败");
                        continue;
                    }
                    var cqe = (CQeventEnable)Marshal.GetDelegateForFunctionPointer(dhandle, typeof(CQeventEnable));
                    cqe.Invoke();
                }
            });
        }

        public async void Plugins_GroupMsg(int msgId, long fromGroup, long fromQQ, string msg)
        {

            await Task.Factory.StartNew(() =>
            {
                WriteLog("Info", $"[群消息][群:{fromGroup}][QQ:{fromQQ}]{msg}");
                foreach (var item in CQPlugins)
                {
                    var dhandle = Kernel32.GetProcAddress(item.Handle, item._eventGroupMsg);
                    if (dhandle.ToInt32() == 0)
                    {
                        WriteLog("Error", $"对{item.PluginName}调用{item._eventGroupMsg}函数失败");
                        continue;
                    }
                    var cqgmsg = (CQGroupMsg)Marshal.GetDelegateForFunctionPointer(dhandle, typeof(CQGroupMsg));
                    cqgmsg.Invoke(0, msgId, fromGroup, fromQQ, "", msg, 0);
                    WriteLog("Info", $"[群消息]已转发至{item.PluginName}");
                }
            });
        }

        public async void Plugins_PrivateMsg(int msgId, long fromQQ, string msg)
        {
                WriteLog("Info", $"[私聊][QQ:{fromQQ}]{msg}");
                foreach (var item in CQPlugins)
                {
                    var dhandle = Kernel32.GetProcAddress(item.Handle, item._eventPrivateMsg);
                    if (dhandle.ToInt32() == 0)
                    {
                        WriteLog("Error", $"对{item.PluginName}调用{item._eventPrivateMsg}函数失败");
                        continue;
                    }
                await Task.Factory.StartNew(() => { 
                    var cqgmsg = (CQPrivateMsg)Marshal.GetDelegateForFunctionPointer(dhandle, typeof(CQPrivateMsg));
                    cqgmsg.Invoke(0, msgId, fromQQ, msg, 0);
                    WriteLog("Info", $"[私聊消息]已转发至{item.PluginName}");
                
                });
                }
        }

        Forms.NotifyIcon nicon = null;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            this.Hide();
            if (nicon == null)
            {
                nicon = new Forms.NotifyIcon();
                nicon.Icon = new System.Drawing.Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(@"XQNative_Console.logo.ico"));
                nicon.Text = "打开XQNative-Console";
                nicon.Click += Nicon_Click;

            }
            nicon.Visible = true;
            e.Cancel = true;
        }

        private void Nicon_Click(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
            nicon.Visible = false;


        }

        #region 辅助方法


        public void WriteLog(string type, string log,string from = "System")
        {
            /*
             Type种类:
            Error
            Info
            Tip
            Debug
             */
            this.Dispatcher.Invoke(() => {
                var l = new XQNC_Log()
                {
                    Time = DateTime.Now.TimeOfDay,
                    Content = log,
                    From = from,
                    Type = type,
                    Status = "OK"
                };
                Logs.Add(
                    l
                ) ;
                if(autoScroll)
                    LogTbx.ScrollIntoView(l);
            });
        }

        #endregion 辅助方法

        private void Btn_Plugins(object sender, RoutedEventArgs e)
        {
            CQPluginListView view = new CQPluginListView();
            view.Show();
        }
    }
}