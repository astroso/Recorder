using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Recorder.Helpers.Hook.Enums;

namespace Recorder.Helpers.Hook
{
    /// This code is public and it's made by @CooLMine
    /// This is the repo: https://github.com/CooLMinE/MouseHookLib
    /// I edited something but the code is from him
    public class MouseHook
    {
        private static IntPtr _hHook = IntPtr.Zero;

        private NativeMethods.LowLevelMouseProc _mouseProc;

        public event EventHandler<MouseEventArgs> MouseClickDown;
        public event EventHandler<MouseEventArgs> MouseClickUp;

        public void Hook()
        {
            _mouseProc = HookCallback;

            _hHook = NativeMethods.SetWindowsHookEx(14, _mouseProc,
                NativeMethods.GetModuleHandle(Process.GetCurrentProcess().MainModule?.ModuleName), 0);

            if (_hHook == IntPtr.Zero)
                throw new InvalidOperationException("Unable to register mouse hook.");
        }

        public static void Unhook()
        {
            NativeMethods.UnhookWindowsHookEx(_hHook);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0) return NativeMethods.CallNextHookEx(_hHook, nCode, wParam, lParam);

            var hookStruct =
                (NativeMethods.MsllHookStruct)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MsllHookStruct));

            switch ((MouseMessages)wParam)
            {
                case MouseMessages.WM_LBUTTONUP:
                    MouseClickDown?.Invoke(this,
                        new MouseEventArgs(MouseButtons.Left, 1, hookStruct.pt.x, hookStruct.pt.y, 0));
                    break;

                case MouseMessages.WM_LBUTTONDOWN:
                    MouseClickUp?.Invoke(this,
                        new MouseEventArgs(MouseButtons.Left, 1, hookStruct.pt.x, hookStruct.pt.y, 0));
                    break;
            }

            return NativeMethods.CallNextHookEx(_hHook, nCode, wParam, lParam);
        }
    }
}