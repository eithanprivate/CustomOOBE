using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CustomOOBE
{
    public class KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        private IntPtr hookID = IntPtr.Zero;
        private LowLevelKeyboardProc proc;

        public KeyboardHook()
        {
            proc = HookCallback;
        }

        public void InstallHook()
        {
            hookID = SetHook(proc);
        }

        public void UninstallHook()
        {
            if (hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookID);
                hookID = IntPtr.Zero;
            }
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);

                // Bloquear combinaciones de teclas
                bool ctrlPressed = (GetAsyncKeyState(0x11) & 0x8000) != 0;
                bool altPressed = (GetAsyncKeyState(0x12) & 0x8000) != 0;
                bool shiftPressed = (GetAsyncKeyState(0x10) & 0x8000) != 0;

                // Ctrl+Alt+Del
                if (ctrlPressed && altPressed && vkCode == 0x2E)
                    return (IntPtr)1;

                // Alt+F4
                if (altPressed && vkCode == 0x73)
                    return (IntPtr)1;

                // Ctrl+Shift+Esc (Task Manager)
                if (ctrlPressed && shiftPressed && vkCode == 0x1B)
                    return (IntPtr)1;

                // Ctrl+Tab
                if (ctrlPressed && vkCode == 0x09)
                    return (IntPtr)1;

                // Windows Key
                if (vkCode == 0x5B || vkCode == 0x5C)
                    return (IntPtr)1;

                // Alt+Tab
                if (altPressed && vkCode == 0x09)
                    return (IntPtr)1;
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
    }
}
