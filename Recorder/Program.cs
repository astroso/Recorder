using System.Diagnostics;
using System.Windows.Forms;
using Recorder.Helpers;
using Recorder.Helpers.Modules;
using Recorder.Helpers.Modules.Listeners;

namespace Recorder
{
    internal static class Program
    {
        private static NativeMethods.ConsoleEventDelegate _consoleHandler;

        private static void Main()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            _consoleHandler = ConsoleEventCallback;
            NativeMethods.SetConsoleCtrlHandler(_consoleHandler, true);

            NativeMethods.TimeBeginPeriod(1u);

            var manager = ModuleManager.GetManager();
            manager["GameListener"] = new GameListener();
            manager.StartModules();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RecorderForm());
        }

        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
                RecorderForm.GetInstance().ProperExit();

            return false;
        }
    }
}