using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TestCandidat
{
    public class SpectacularViewModel : ISpectacularViewModel
    {
        #region Variables

        private ICommand _execute, _stop;
        private BackgroundWorker _worker;
        private Stopwatch _popupWatch;

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Properties

        private int _milliSeconds;

        public int MilliSeconds
        {
            get { return _milliSeconds; }
            set
            {
                if (!IsExecuting)
                {
                    _milliSeconds = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _refreshDelayInMilliseconds;

        public int RefreshDelayInMilliseconds
        {
            get { return _refreshDelayInMilliseconds; }
            set
            {
                _refreshDelayInMilliseconds = value;
                NotifyPropertyChanged();
            }
        }

        private double _executionRatio;

        public double ExecutionRatio
        {
            get { return _executionRatio; }
            private set
            {
                _executionRatio = value > 1.0 ? 1.0 : value;
                NotifyPropertyChanged();
                Trace.WriteLine("ExecutionRatio: " + value);
            }
        }

        private bool popupVisible;

        public bool PopupVisible
        {
            get { return popupVisible; }
            private set
            {
                popupVisible = value;
                NotifyPropertyChanged();
            }
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isExecuting;

        public bool IsExecuting
        {
            get { return _isExecuting; }
        }

        public ICommand Execute
        {
            get { return _execute; }
        }

        public ICommand StopExecution
        {
            get { return _stop; }
        }

        #endregion Properties

        public SpectacularViewModel()
        {
            _isExecuting = false;
            _popupWatch = new Stopwatch();
            _execute = new RelayCommand<object>(ExecuteCommand, CanExecuteCommand);
            _stop = new RelayCommand<object>(StopCommand, CanStopCommand);

            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.Loaded += MainWindowOnLoaded;
        }

        ~SpectacularViewModel()
        {
            _execute = null;
            _stop = null;
            _popupWatch = null;
            GC.Collect();
        }


        #region BackgroundWorker

        private void MainWindowOnLoaded(object sender, RoutedEventArgs e)
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += worker_DoWork;
            _worker.ProgressChanged += worker_ProgressWork;
            _worker.RunWorkerCompleted += worker_CompleteWork;
        }

        private double _currentProgressValue;

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bckworker = (sender as BackgroundWorker);
            if (bckworker == null)
                e.Result = false;

            _currentProgressValue = 0.0;
            Stopwatch progressWatch = new Stopwatch();
            progressWatch.Start();

            double elapsed = 0.0;
            double nextTrigger = 0.0;

            while (!bckworker.CancellationPending && _popupWatch.ElapsedMilliseconds < MilliSeconds)
            {
                nextTrigger += _refreshDelayInMilliseconds;

                while (true)
                {
                    elapsed = progressWatch.ElapsedMilliseconds;

                    //Get the waiting time.
                    double diff = nextTrigger - elapsed;

                    //Waiting is not necessary.
                    if (diff <= 0f)
                    {
                        break;
                    }

                    //Thread.SpinWait used for very short elapsed time. This helps to have the app more responsible and to have a more accurate timer.
                    //Take the margin of 30 ms delay
                    if (diff < 1f)
                        Thread.SpinWait(10);
                    else if (diff < 5f)
                        Thread.SpinWait(100);
                    else if (diff < 10f)
                        Thread.Sleep(1);
                    else
                        Thread.Sleep(10);
                }

                double step = (RefreshDelayInMilliseconds * 1.0 / MilliSeconds);
                _currentProgressValue += step;
                bckworker.ReportProgress((int) (_currentProgressValue * 100));
                Trace.WriteLine("Progress timer: " + progressWatch.Elapsed);
            }

            e.Result = !bckworker.CancellationPending;
        }


        private void worker_ProgressWork(object sender, ProgressChangedEventArgs e)
        {
            Task.Factory.StartNew(() => { ExecutionRatio = (e.ProgressPercentage / 100.0); });
        }

        private void worker_CompleteWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsExecuting)
            {
                Trace.WriteLine("Popup timer: " + _popupWatch.Elapsed);
                PopupVisible = true;
                ExecutionRatio = 1;
            }
        }

        #endregion

        #region Commands

        public bool CanExecuteCommand(object parameter)
        {
            return !_isExecuting;
        }

        public void ExecuteCommand(object parameter)
        {
            if (!_isExecuting)
            {
                _isExecuting = true;
                ExecutionRatio = 0.0;
                Trace.WriteLine("Start time: " + DateTime.Now.ToString("hh:mm:ss:ffff"));
                _worker.RunWorkerAsync();
                _popupWatch.Start();
            }
        }

        public bool CanStopCommand(object parameter)
        {
            return _isExecuting;
        }

        public void StopCommand(object parameter)
        {
            if (_isExecuting)
            {
                _isExecuting = false;
                _worker.CancelAsync();
                PopupVisible = false;
                _popupWatch.Stop();
                _popupWatch.Reset();
            }
        }

        #endregion
    }
}