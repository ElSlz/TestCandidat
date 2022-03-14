using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace TestCandidat
{
    
    public class MainViewModel : INotifyPropertyChanged
    {
        public SpectacularViewModel Spectacular { get; private set; }
        
        public MainViewModel()
        {
            Spectacular = new SpectacularViewModel
            {
                MilliSeconds = 2000,
                Text = "Toto", 
                RefreshDelayInMilliseconds=233
            };
            PopupVisible = false;
            Spectacular.PopupEvent += new OnPopupVisibleHandler(Callback);
        }
        
        ~MainViewModel()
        {
            Spectacular.PopupEvent -= Callback;
            Spectacular = null;
        }
        
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
        private bool popupVisible;
        public bool PopupVisible
        {
            get
            {
                return popupVisible;
            }
            private set
            {
                popupVisible = value;
                NotifyPropertyChanged();
            }
        }
        #endregion 
        
        private void Callback(object sender, bool display)
        {
            PopupVisible = display;
        }
        
    }

}
