using System.Threading;

namespace Recorder.Helpers.Modules
{
    public class Module
    {
        private bool _lastEnabled;

        private bool _mightRun;
        private Thread _thread;

        protected Module()
        {
            Init();
        }

        protected bool Enabled { get; set; }

        private bool IsRunning { get; set; }

        private void Init()
        {
            _thread = new Thread(ThreadFunc);
        }

        public void Start()
        {
            if (_mightRun) return;
            
            _mightRun = true;
            _thread.Start();
        }

        public void Stop()
        {
            if (!_mightRun) return;
            
            _mightRun = false;
            _thread.Abort();
        }

        private void ThreadFunc()
        {
            IsRunning = true;

            OnStart();

            while (_mightRun)
            {
                Thread.Sleep(1);

                if (_lastEnabled != Enabled)
                {
                    _lastEnabled = Enabled;

                    if (Enabled)
                        OnEnable();
                    else
                        OnDisable();
                }

                if (Enabled)
                    OnCall();
                else
                    Thread.Sleep(20);
            }

            IsRunning = false;
        }

        protected virtual void OnCall()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnStop()
        {
        }
    }
}