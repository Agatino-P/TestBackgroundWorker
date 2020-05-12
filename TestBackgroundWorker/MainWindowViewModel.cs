using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace TestBackgroundWorker
{
    public class MainWindowViewModel : ViewModelBase
    {
        private int _percentage; public int Percentage { get => _percentage; set { Set(() => Percentage, ref _percentage, value); } }
        private int _max = 100; public int Max { get => _max; set { Set(() => Max, ref _max, value); } }
        private string _counter; public string Counter { get => _counter; set { Set(() => Counter, ref _counter, value); }}

        BackgroundWorker worker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };

        private RelayCommand _startCmd;
        public RelayCommand StartCmd => _startCmd ?? (_startCmd = new RelayCommand(
            () => start(),
            () => { return 1 == 1; },
            keepTargetAlive: true
            ));

        private void start()
        {
            Percentage = 0;
            worker.RunWorkerAsync(Max);
        }


        private RelayCommand _stopCmd;
        public RelayCommand StopCmd => _stopCmd ?? (_stopCmd = new RelayCommand(
            () => Stop(),
            () => { return 1 == 1; },
			keepTargetAlive:true
            ));

        private void Stop()
        {
            throw new NotImplementedException();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!(sender is BackgroundWorker worker))
                return;

            if (!(e.Argument is int max))
                return;
            for (int  counter=1; counter <=max; counter++)
            {
                Thread.Sleep(100);
                int percentage = (counter * 100) / max;
                worker.ReportProgress(percentage, counter);
            }
            e.Result = Max;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!(sender is BackgroundWorker worker))
                return;
            if (!(e.UserState is int counter))
                return;

            Percentage = e.ProgressPercentage;
            Counter = counter.ToString();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(e.Result is int result))
                return;

            Percentage = 100;
            Counter = result.ToString();
        }

        public MainWindowViewModel()
        {
            if (IsInDesignMode)
            {
                Percentage = 30;
            }

            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }
    }
}
