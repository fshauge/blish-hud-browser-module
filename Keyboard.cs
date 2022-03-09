using System.Runtime.InteropServices;

namespace WikiModule
{
    public static unsafe class Keyboard
    {
        [DllImport("user32.dll", EntryPoint = "GetKeyboardState", SetLastError = true)]
        private static extern bool GetKeyboardState(byte* lpKeyState);

        [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA", SetLastError = true)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", EntryPoint = "ToAscii", SetLastError = true)]
        private static extern int ToAscii(uint uVirtKey, uint uScanCode, byte* lpKeyState, out char lpchar, uint uFlags);

        public static bool TryGetChar(uint key, out char ch)
        {
            var kbs = stackalloc byte[256];
            GetKeyboardState(kbs);
            return ToAscii(key, MapVirtualKey(key, 0), kbs, out ch, 0) > 0;
        }
    }
}
