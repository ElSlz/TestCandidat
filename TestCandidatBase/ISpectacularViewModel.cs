using System.ComponentModel;
using System.Windows.Input;

namespace TestCandidat
{
    public interface ISpectacularViewModel : INotifyPropertyChanged
    {
        
        int MilliSeconds { get; set; }
        int RefreshDelayInMilliseconds { get; set; }
        double ExecutionRatio { get; }
        string Text { get; set; }

        bool IsExecuting { get; }
        ICommand Execute { get; }

        ICommand StopExecution { get; }
    }  


}
