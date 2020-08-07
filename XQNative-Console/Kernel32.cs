using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XQNative_Console
{
    public static class  Kernel32
    {
        /// <summary>
        /// 载入DLL
        /// </summary>
        /// <param name="dllname"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", EntryPoint = "LoadLibraryA")]
        public static extern IntPtr LoadLibraryA(string dllname);

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", EntryPoint = "FreeLibrary")]
        public static extern int FreeLibrary(int handle);

        /// <summary>
        /// 取方法进程句柄
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="procName"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", EntryPoint = "GetProcAddress")]
        public static extern IntPtr GetProcAddress(IntPtr handle,string procName);
    }
}
