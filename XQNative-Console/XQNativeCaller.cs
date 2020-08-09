using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XQNative_Console
{
    public class XQNativeCaller
    {
        public static string Version = "1.2.2";
        public static MainWindow mainWindow;
        [DllExport("XQNC_CallMenu",CallingConvention =System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void CallConsole()
        {
            if (mainWindow == null)
            {
                mainWindow = new MainWindow();
                mainWindow.Show();
                //mainWindow.setQQ(qq);
                mainWindow.Closed += MainWindow_Closed;
            }
            else
            {
                mainWindow.Activate();
            }
        }

        [DllExport("XQNC_EventInitialize", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void XQNC_EventInitialize(string qq)
        {
            //mainWindow.WriteLog("Info", "初始化事件抵达");
           // mainWindow.Plugins_Initialiaze(qq);
        }

        [DllExport("XQNC_EventEnable", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void XQNC_EventEnable()
        {
            //mainWindow.Plugins_Enable();
        }

        [DllExport("XQNC_EventDisable", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void XQNC_EventDisable()
        {
            mainWindow.Plugins_Disable();
        }

        [DllExport("XQNC_EventGroupMsg", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void XQNC_EventGroupMsg(int msgid,string fromgroup,string qq,string msg)
        {
            mainWindow.Plugins_GroupMsg(msgid,long.Parse(fromgroup),long.Parse(qq),msg);
        }

        [DllExport("XQNC_EventPrivateMsg", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void XQNC_EventPrivateMsg(int msgid, string qq, string msg)
        {
            mainWindow.Plugins_PrivateMsg(msgid,long.Parse(qq), msg);
        }


        //用来返回authcode对应的数据目录。
        [DllExport("XQNC_GetAppDirectory", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static string XQNC_GetAppDirectory(int authcode)
        {
            var cqplugin = mainWindow.CQPlugins.ToList().Find(p => p.AuthCode == authcode);
            if (cqplugin != null)
            {
                return Path.Combine( Directory.GetCurrentDirectory(), "CQPlugins","data" ,"app",cqplugin.PluginID);
            }
            else
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "CQPlugins", "data", "app", "UnkonwPlugins");
            }
        }

        private static void MainWindow_Closed(object sender, EventArgs e)
        {
            mainWindow.Plugins_Disable();
            mainWindow = null;
        }
    }
}
