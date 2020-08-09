using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XQNative_Console
{
    public class CQPlugin
    {
        /// <summary>
        /// 插件句柄
        /// </summary>
        public IntPtr Handle { get; set; }

        public string PluginAuthor { get; set; }

        public string PluginVersion { get; set; }

        public string PluginName { get; set; }

        public string PluginID { get; set; }

        public string PluginPath { get; set; }

        public int AuthCode { get; set; } = Guid.NewGuid().GetHashCode();


        public string _eventPrivateMsg { get; set; } = "_eventPrivateMsg";
        public string _eventGroupMsg { get; set; } = "_eventGroupMsg";
        public string _eventEnable { get; set; } = "_eventEnable";
        public string _eventDisable { get; set; } = "_eventDisable";
        public string _eventStartUp { get; set; } = "_eventStartup";

        public List<MenuNode> MenuFuncs { get; set; } = new List<MenuNode>();
        
        ~CQPlugin()
        {
            Kernel32.FreeLibrary(Handle.ToInt32());
        }

        public class MenuNode
        {
            public string Name { get; set; }
            public string Function { get; set; }
        }
    }


}
