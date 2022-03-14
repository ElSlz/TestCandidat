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
    public delegate void OnPopupVisibleHandler(object sender, bool value);

    public class SpectacularViewModel : ISpectacularViewModel
    {
        public event OnPopupVisibleHandler PopupEvent;

        #region Variables

        private ICommand _execute, _stop;
        private System.Timers.Timer _popupTimer;
        private System.Timers.Timer _progressTimer;
        private Stopwatch _popupWatch, _progressWatch;

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
                    _popupTimer.Interval = _milliSeconds;
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
                _progressTimer.Interval = _refreshDelayInMilliseconds;
                NotifyPropertyChanged();
            }
        }

        private double _executionRatio;

        public double ExecutionRatio
        {
            get { return _executionRatio; }
            private set
            {
                _executionRatio = value;
                NotifyPropertyChanged();
                Trace.WriteLine("ExecutionRatio: " + value);
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
            _execute = new RelayCommand<object>(ExecuteCommand, CanExecuteCommand);
            _stop = new RelayCommand<object>(StopCommand, CanStopCommand);
            _isExecuting = false;
            _popupWatch = new Stopwatch();
            _progressWatch = new Stopwatch();
            _popupTimer = new System.Timers.Timer();
            _popupTimer.Elapsed += popup_Tick;
            _progressTimer = new System.Timers.Timer();
            _progressTimer.Elapsed += progress_Tick;
        }

        ~SpectacularViewModel()
        {
            _popupTimer.Dispose();
            _progressTimer.Dispose();
            _execute = null;
            _stop = null;
            _popupWatch = null;
            _progressWatch = null;
        }

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
                _popupWatch.Start();
                _popupTimer.Start();
                _progressTimer.Start();
                _progressWatch.Start();
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
                _popupTimer.Stop();
                _progressTimer.Stop();
                _progressWatch.Stop();
                _popupWatch.Stop();
                _isExecuting = false;
                if (PopupEvent != null)
                    PopupEvent(this, false);
            }
        }

        #endregion

        private void popup_Tick(object sender, EventArgs e)
        {
            if (PopupEvent != null)
                PopupEvent(this, true);
            Trace.WriteLine("Popup timer: " + _popupWatch.Elapsed.TotalMilliseconds + " ms");
        }

        private void progress_Tick(object sender, EventArgs e)
        {
            double step = RefreshDelayInMilliseconds * 1.0 / MilliSeconds;
            Task.Factory.StartNew(() => { ExecutionRatio += step; });
            Trace.WriteLine("Progress bar timer : " + _progressWatch.Elapsed.TotalMilliseconds + " ms");
        }
    }
}