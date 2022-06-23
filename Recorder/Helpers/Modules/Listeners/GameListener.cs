using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Recorder.Helpers.Modules.Listeners
{
    public class GameListener : Module
    {
        public bool IsGameForeground => IsMinecraftForeground || IsAzLauncherForeground;

        private bool IsMinecraftForeground { get; set; }

        private bool IsAzLauncherForeground { get; set; }

        private IntPtr MinecraftHandle { get; set; }

        private IntPtr AzLauncherHandle { get; set; }

        public bool IsCursorVisible { get; private set; }

        protected override void OnStart()
        {
            Enabled = true;
        }

        protected override void OnCall()
        {
            MinecraftHandle = NativeMethods.FindWindow("LWJGL", null);
            AzLauncherHandle = NativeMethods.FindWindow("GLFW30", null);

            if (NativeMethods.GetForegroundWindow() == MinecraftHandle)
            {
                IsMinecraftForeground = true;
                IsAzLauncherForeground = false;
            }
            else if (NativeMethods.GetForegroundWindow() == AzLauncherHandle)
            {
                IsMinecraftForeground = false;
                IsAzLauncherForeground = true;
            }
            else
            {
                IsMinecraftForeground = false;
                IsAzLauncherForeground = false;
            }

            if (IsGameForeground)
            {
                var cursorInfo = new NativeMethods.Cursorinfo
                {
                    CbSize = Marshal.SizeOf(typeof(NativeMethods.Cursorinfo))
                };

                NativeMethods.GetCursorInfo(ref cursorInfo);

                if (IsMinecraftForeground)
                    IsCursorVisible = RecorderForm.GetInstance().Cursor.Handle.ToInt32() ==
                                      cursorInfo.HCursor.ToInt32();

                else if (IsAzLauncherForeground)
                    IsCursorVisible = cursorInfo.Flags != 1;
            }

            Thread.Sleep(1);
        }
    }
}