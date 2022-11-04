using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace dotnete.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            Log = "";
            InputVideoPath = "";
            OutputPath = "";
            AudioPath = "";
        }
        private string log;
        private string inputVideoPath;
        private string outputPath;
        private string audioPath;

        public string AudioPath
        {
            get { return audioPath;  }
            set { audioPath = value; OnPropertyChanged(); }
        }
        public string OutputPath
        {
            get { return outputPath; }
            set { outputPath = value; OnPropertyChanged(); }
        }
        public string Log
        {
            get { return log; }
            set
            {
                log = value;
                OnPropertyChanged();
            }
        }
        public string InputVideoPath
        {
            get { return inputVideoPath; }
            set
            {
                inputVideoPath = value;
                OnPropertyChanged();
            }
        }

    }
}
