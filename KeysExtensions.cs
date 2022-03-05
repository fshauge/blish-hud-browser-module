using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;

namespace WikiModule
{
    public static unsafe class KeysExtensions
    {
        [DllImport("user32.dll", EntryPoint = "GetKeyboardState", SetLastError = true)]
        private static extern bool GetKeyboardState(byte* lpKeyState);

        [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA", SetLastError = true)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", EntryPoint = "ToAscii", SetLastError = true)]
        private static extern int ToAscii(uint uVirtKey, uint uScanCode, byte* lpKeyState, out char lpchar, uint uFlags);

        public static char ToChar(this Keys key)
        {
            var kbs = stackalloc byte[256];
            GetKeyboardState(kbs);
            ToAscii((uint)key, MapVirtualKey((uint)key, 0), kbs, out var ch, 0);
            return ch;
        }
    }
}
