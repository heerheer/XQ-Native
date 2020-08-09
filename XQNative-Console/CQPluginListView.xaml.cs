using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using static XQNative_Console.MainWindow;

namespace XQNative_Console
{
    /// <summary>
    /// CQPluginListView.xaml 的交互逻辑
    /// </summary>
    public partial class CQPluginListView : Window
    {
        public ObservableCollection<CQPlugin> CQPlugins
        {
            get
            {
                return XQNativeCaller.mainWindow.CQPlugins;
            }
        }

        public CQPluginListView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_ReloadPlugins(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show("暂未完成,感谢酷Q带给我的陪伴");
            XQNativeCaller.mainWindow.Plugins_Load();

            XQNativeCaller.mainWindow.Plugins_Initialiaze();
        }

        private void Btn_Events(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("暂未完成,感谢酷Q带给我的陪伴");
        }

        private void Btn_Dir(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("Explorer.exe", Directory.GetCurrentDirectory() + "\\CQPlugins");
        }

        private void Btn_Menu(object sender, RoutedEventArgs e)
        {
            if (PluginsList.SelectedIndex < 0)
                return;
            var cqplugin = PluginsList.SelectedItem as CQPlugin;
            //WriteLog("Info", $"呼出应用Menu{menuitem.Header.ToString()},函数为{func}");
            if (cqplugin.MenuFuncs.Count >= 1)
            {
                IntPtr handle = Kernel32.GetProcAddress(cqplugin.Handle, cqplugin.MenuFuncs[0].Function);
                if (handle.ToInt32() == 0)
                {
                    MessageBox.Show("有毒吧,句柄为0");
                }
                else
                {
                    try
                    {
                        CQMenu cqmenu = (CQMenu)Marshal.GetDelegateForFunctionPointer(handle, typeof(CQMenu));
                        cqmenu.Invoke();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void Button_Initialized(object sender, EventArgs e)
        {
            //设置右键菜单为null
            this.MenusBtn.ContextMenu = null;
        }

        private void MenusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PluginsList.SelectedIndex < 0)
                return;
            this.MenusCM.PlacementTarget = this.MenusBtn;
            this.MenusCM.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
            this.MenusCM.Items.Clear();
            foreach (var item in ((CQPlugin)PluginsList.SelectedItem ).MenuFuncs)
            {
                var mitem = new MenuItem() { Header = item.Name,Tag= (CQPlugin)PluginsList.SelectedItem };
                mitem.Click += Mitem_Click;
                this.MenusCM.Items.Add(mitem);
            }

            //显示菜单
            this.MenusCM.IsOpen =true;
        }

        private void Mitem_Click(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            CQPlugin m = mi.Tag as CQPlugin;
            if (m.MenuFuncs.Count >= 1)
            {
                IntPtr handle = Kernel32.GetProcAddress(m.Handle, m.MenuFuncs.Find(s=>s.Name==mi.Header.ToString()).Function);
                if (handle.ToInt32() == 0)
                {
                    MessageBox.Show("有毒吧,句柄为0");
                }
                else
                {
                    try
                    {
                        CQMenu cqmenu = (CQMenu)Marshal.GetDelegateForFunctionPointer(handle, typeof(CQMenu));
                        cqmenu.Invoke();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
    }
}