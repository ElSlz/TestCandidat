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
    
    public class MainViewModel
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
        }
        
    }

}
