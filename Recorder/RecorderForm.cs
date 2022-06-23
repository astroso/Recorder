using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Recorder.Helpers;
using Recorder.Helpers.Hook;
using Recorder.Helpers.Modules;
using Recorder.Helpers.Modules.Listeners;

namespace Recorder
{
    public partial class RecorderForm : Form
    {
        private void Init()
        {
            var manager = ModuleManager.GetManager();
            _gameListener = (GameListener)manager["GameListener"];

            if (!File.Exists("clicks.txt"))
                File.Create("clicks.txt").Close();

            ConsoleHelpers.PrintLine("info", "waiting minecraft instance.", Purple);

            while (_gameListener.IsGameForeground && Process.GetProcessesByName("javaw")[0] != null)
            {
                Thread.Sleep(5);

                Console.Clear();
                ConsoleHelpers.PrintLine("listener",
                    $@"instance found: {Process.GetProcessesByName("javaw")[0].MainWindowTitle}", Purple);
                ConsoleHelpers.PrintLine("info", "try clicking (on instance), recording process will start soon.",
                    Purple);

                break;
            }

            Thread.Sleep(2000);

            Console.Clear();
            ConsoleHelpers.PrintLine("info", "please start clicking (note: only instance clicks will be saved).",
                Purple);
            ConsoleHelpers.PrintLine("info",
                "clicks will be saved on exit, so be sure to close this window after you've finished.", Purple);

            _mouseHook.MouseClickDown += MouseClickHandler;
            _mouseHook.MouseClickUp += MouseClickHandler;
            _mouseHook.Hook();

            _currentTime = DateTime.Now;
        }

        private void MouseClickHandler(object sender, MouseEventArgs e)
        {
            var delay = (DateTime.Now - _currentTime).TotalMilliseconds;
            var roundedDelay = (int)Math.Round(delay);

            if ((_gameListener.IsMinecraftForeground || _gameListener.IsAzLauncherForeground) &&
                !_gameListener.IsCursorVisible &&
                roundedDelay < 150)
            {
                ClickDelays.Add(roundedDelay.ToString());

                _counter++;
                _sum += delay;

                Console.SetCursorPosition(0, 2);
                ConsoleHelpers.PrintLine("listener", $@"{_counter} clicks saved on the text file.", Purple);
                ConsoleHelpers.PrintLine("info", $@"average cps: {1000d / (_sum / _counter) / 2:0.00}",
                    Purple);
            }

            _currentTime = DateTime.Now;
        }

        public void ProperExit()
        {
            File.AppendAllLines("clicks.txt", ClickDelays);

            _mouseHook.MouseClickDown -= MouseClickHandler;
            _mouseHook.MouseClickUp -= MouseClickHandler;
            MouseHook.Unhook();

            Process.GetCurrentProcess().Kill();
        }

        #region Global Vars

        private static RecorderForm _instance;

        private readonly MouseHook _mouseHook = new MouseHook();
        private DateTime _currentTime;
        private GameListener _gameListener;

        private static readonly List<string> ClickDelays = new List<string>();

        private static int _counter;
        private static double _sum;

        private static readonly Color Purple = Color.FromArgb(135, 145, 216);

        #endregion

        #region .ctor

        public RecorderForm()
        {
            InitializeComponent();
            Load += RecorderForm_Load;
            FormClosing += Recorder_FormClosing;
        }

        public static RecorderForm GetInstance()
        {
            return _instance ?? (_instance = new RecorderForm());
        }

        private void RecorderForm_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Recorder_FormClosing(object sender, FormClosingEventArgs e)
        {
            ProperExit();
        }

        #endregion
    }
}