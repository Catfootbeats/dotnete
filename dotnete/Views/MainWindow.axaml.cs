using Avalonia.Controls;
using Avalonia.Interactivity;
using dotnete.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace dotnete.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }
        //Button
        private void VideoInput(object sender, RoutedEventArgs e)
        {
            var extensions = new List<string>() { "mp4", "avi", "mov", "flv", "mkv" };
            var filePath = OpenVideofileDialog("��Ƶ�ļ�(*.mp4,*.avi,*.mov,*.flv,*.mkv)", extensions);
            if (filePath != null)
            {
                InputVideoPath.Text = filePath[0];
            }
            else { return; }
        }
        private void AudioInput(object sender, RoutedEventArgs e)
        {
            var extensions = new List<string>() { "wav", "mp3", "acc" };
            var filePath = OpenVideofileDialog("��Ƶ�ļ�(*.wav,*.mp3,*.acc)", extensions);
            if (filePath != null)
            {
                InputAudioPath.Text = filePath[0];
            }
            else { return; }
        }
        private void Output(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new()
            {
                Title = "��ѡ���ļ�"
            };
            var result = dialog.ShowAsync(this);
            var folderPath = result.Result;
            if (folderPath != null)
            {
                OutputPath.Text = folderPath;
            }
            else{ return; }
        }
        private void ExtAudio(object sender, RoutedEventArgs e)
        {
            if (InputVideoPath.Text != "" && OutputPath.Text != "")
            {
                Log.Text = "��ȡ��Ƶ��~\n";
                ExtractAudio();
            }
            else
            {
                Log.Text = "�������ļ�·�������·��";
            }
        }
        private void RepAudio(object sender, RoutedEventArgs e)
        {
            if (InputVideoPath.Text != ""&& OutputPath.Text != ""&& InputAudioPath.Text != "")
            {
                Log.Text = "�滻��Ƶ��~\n";
                ReplaceAudio();
            }
            else
            {
                Log.Text = "�������ļ�·�������·��";
            }
        }
        private void VideoCom(object sender, RoutedEventArgs e)
        {
            if (InputVideoPath.Text != "" && OutputPath.Text != "")
            {
                Log.Text = "ѹ����Ƶ��~\n";
                EncodeVideo();
            }
            else
            {
                Log.Text = "�������ļ�·�������·��";
            }
        }
        //private void Window_Drop_Video(object sender, DragEventArgs e)
        //{
        //    string path = "Drop";
        //    if (e.Data.Contains(DataFormats.FileNames))
        //    {
        //        path = ((System.Array)e.Data.Get(DataFormats.FileNames)).GetValue(0).ToString();
        //    }
        //}
        //���ļ�ѡ��Ի���
        public string[]? OpenVideofileDialog(string name,List<string> strings)
        {
            OpenFileDialog dialog = new()
            {
                AllowMultiple = false,//��ֵȷ���Ƿ����ѡ�����ļ�
                Title = "��ѡ���ļ�"
            };
            FileDialogFilter fileDialogFilter = new()
            {
                Name = name,
                Extensions = strings
            };
            List<FileDialogFilter> fileDialogFilters = new() { fileDialogFilter };
            dialog.Filters = fileDialogFilters;
            var result = dialog.ShowAsync(this);
            return result.Result;
        }
        public async void ReplaceAudio()
        {
            //�滻��Ƶ�������±���
            string videoInput = InputVideoPath.Text;
            string videoOutput = OutputPath.Text;
            string audioInput = InputAudioPath.Text;
            string[] split = videoInput.Split(new char[] { '\\' });
            string filePathNew = videoOutput + "\\" + "Replace_" + split[split.Length - 1];
            string strCmd = $"-i \"{videoInput}\" -i \"{audioInput}\" -map 0:0 -map 1:0 -c:v copy -c:a libmp3lame -q:a 0 \"{filePathNew}\"";
            try
            {
                Log.Text = await Task.Run(() => RunProcess(strCmd));
                Log.Text = "���!";
            }
            catch (Exception e)
            {
                Log.Text = e.ToString();
            }
        }
        public async void ExtractAudio()
        {
            //input
            string videoInput = InputVideoPath.Text;
            string videoOutput = OutputPath.Text;
            string[] split = videoInput.Split(new char[] { '\\' });
            string filePath = videoOutput + "\\" + "Extract_" + split[split.Length - 1];
            //string filePathNew = filePath.Substring(filePath.Length - 4) + "mp3";
            string filePathNew = Path.ChangeExtension(filePath, ".mp3");
            string strCmd = " -i \"" + videoInput + "\" -q:a 0 -map a \"" + filePathNew + "\"";
            //ffmpeg -i sample.mp4 -q:a 0 -map a sample.mp3
            try
            {
                Log.Text = await Task.Run(() => RunProcess(strCmd));
                Log.Text = "���!";
            }
            catch (Exception e)
            {
                Log.Text = e.ToString();
            }
        }
        public async void EncodeVideo()
        {
            //input
            string videoInput = InputVideoPath.Text;
            string videoOutput = OutputPath.Text;
            string videoBitrate = VedioBitrate.Text;
            string videoFPS = FPS.Text;
            string audioBitrate = AudioBitrate.Text;
            string resWide = Width.Text;
            string resHeight = Height.Text;
            string resolution = resWide + "+" + resHeight;
            string[] split = videoInput.Split(new char[] { '\\' });
            string filePath = videoOutput + "\\" + "Encode_" + split[split.Length - 1];
            int videoCodecIndex = VideoEncode.SelectedIndex;
            string videoCodec = "";
            int videoMode = 0;
            switch (videoCodecIndex)
            {
                case 0:
                    videoCodec = "h264";
                    break;
                case 1:
                    videoCodec = "h265";
                    break;
                case 2:
                    videoCodec = "mpeg4";
                    break;
                case 3:
                    videoCodec = "libaom-av1";
                    break;
                case 4:
                    videoMode = 1;
                    break;
            }
            int audioCodecIndex = AudioEncode.SelectedIndex;
            string audioCodec = "";
            int audioMode = 0;
            switch (audioCodecIndex)
            {
                case 0:
                    audioCodec = "mp3";
                    break;
                case 1:
                    audioCodec = "acc";
                    break;
                case 2:
                    audioMode = 1;
                    break;
            }
            int formatIndex = Format.SelectedIndex;
            string format = "";
            switch (formatIndex)
            {
                case 0:
                    format = ".mp4";
                    break;
                case 1:
                    format = ".flv";
                    break;
                case 2:
                    format = ".mov";
                    break;
                case 3:
                    format = ".avi";
                    break;
            }
            string filePathNew = Path.ChangeExtension(filePath, format);
            string strCmd = "";
            if (videoMode == 0)
            {
                if (audioMode == 0)
                {
                    strCmd = "-i" + $" \"{videoInput}\" -b:v {videoBitrate}k -b:a {audioBitrate}k -c:v {videoCodec} -c:a {audioCodec} -s {resolution} -r {videoFPS} \"{filePathNew}\"";
                }
                else
                {
                    strCmd = "-i" + $" \"{videoInput}\" -b:v {videoBitrate}k -c:v {videoCodec} -s {resolution} -r {videoFPS} \"{filePathNew}\"";
                }
            }
            else
            {
                if (audioMode == 0)
                {
                    strCmd = "-i" + $" \"{videoInput}\" -b:a {audioBitrate}k -c:a {audioCodec} \"{filePathNew}\"";
                }
                else
                {
                    strCmd = "-i" + $" \"{videoInput}\" -c copy \"{filePathNew}\"";
                }
            }
            //����
            //����ѡ���ƣ����䣬ת��ʽ
            //����Ƶ��ѡ���ƣ��ı�ֱ��ʵ���Ƶ��ز���
            //����Ƶ��ѡ���ƣ��ı��������ز���
            try
            {
                Log.Text = await Task.Run(() => RunProcess(strCmd));
                Log.Text = "���!";
            }
            catch (Exception e)
            {
                Log.Text = e.ToString();
            }
            //ffmpeg -i .\input.mp4 -b:v 2000k -b:a 320k .\output.mp4

            //ffmpeg - i.\input.mp4 - b:v 2000k - c:a copy .\output.avi
            //ffmpeg - i.\input.mp4 - b:v 2000k - b:a 320k - f wmv.\output.wmv
            //ffmpeg - i.\input.webm - b:v 1800k - b:a 192k - c:v h264 -c:a aac .\output.mp4

        }
        public string RunProcess(string Parameters)
        {
            //����һ��ProcessStartInfo���� �������������
            var oInfo = new ProcessStartInfo(@".\ffmpeg.exe", Parameters)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };
            //����һ���ַ�����StreamReader ������ȡ������
            string output = null;
            StreamReader srOutput = null;
            //����ffmpeg��ʼ��������
            var proc = Process.Start(oInfo);
            //proc.WaitForExit();
            //��ȡ�����
            srOutput = proc.StandardError;
            //ת����string
            output = srOutput.ReadToEnd();
            //�رմ������
            proc.Close();
            if (srOutput != null)
            {
                srOutput.Close();
                srOutput.Dispose();
            }
            return output;
        }

    }
}
