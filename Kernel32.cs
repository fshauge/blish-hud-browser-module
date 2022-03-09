using System;
using System.Runtime.InteropServices;

namespace WikiModule
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryA", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("kernel32.dll", EntryPoint = "SetDllDirectoryA", SetLastError = true)]
        public static extern bool SetDllDirectory(string lpPathName);
    }
}
