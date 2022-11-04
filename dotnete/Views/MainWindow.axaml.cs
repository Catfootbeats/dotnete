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
            var filePath = OpenVideofileDialog("视频文件(*.mp4,*.avi,*.mov,*.flv,*.mkv)", extensions);
            if (filePath != null)
            {
                InputVideoPath.Text = filePath[0];
            }
            else { return; }
        }
        private void AudioInput(object sender, RoutedEventArgs e)
        {
            var extensions = new List<string>() { "wav", "mp3", "acc" };
            var filePath = OpenVideofileDialog("音频文件(*.wav,*.mp3,*.acc)", extensions);
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
                Title = "请选择文件"
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
                Log.Text = "提取音频中~\n";
                ExtractAudio();
            }
            else
            {
                Log.Text = "请输入文件路径和输出路径";
            }
        }
        private void RepAudio(object sender, RoutedEventArgs e)
        {
            if (InputVideoPath.Text != ""&& OutputPath.Text != ""&& InputAudioPath.Text != "")
            {
                Log.Text = "替换音频中~\n";
                ReplaceAudio();
            }
            else
            {
                Log.Text = "请输入文件路径和输出路径";
            }
        }
        private void VideoCom(object sender, RoutedEventArgs e)
        {
            if (InputVideoPath.Text != "" && OutputPath.Text != "")
            {
                Log.Text = "压制视频中~\n";
                EncodeVideo();
            }
            else
            {
                Log.Text = "请输入文件路径和输出路径";
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
        //打开文件选择对话窗
        public string[]? OpenVideofileDialog(string name,List<string> strings)
        {
            OpenFileDialog dialog = new()
            {
                AllowMultiple = false,//该值确定是否可以选择多个文件
                Title = "请选择文件"
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
            //替换音频不会重新编码
            string videoInput = InputVideoPath.Text;
            string videoOutput = OutputPath.Text;
            string audioInput = InputAudioPath.Text;
            string[] split = videoInput.Split(new char[] { '\\' });
            string filePathNew = videoOutput + "\\" + "Replace_" + split[split.Length - 1];
            string strCmd = $"-i \"{videoInput}\" -i \"{audioInput}\" -map 0:0 -map 1:0 -c:v copy -c:a libmp3lame -q:a 0 \"{filePathNew}\"";
            try
            {
                Log.Text = await Task.Run(() => RunProcess(strCmd));
                Log.Text = "完成!";
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
                Log.Text = "完成!";
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
            //策略
            //当都选择复制，不变，转格式
            //当视频不选择复制，改变分辨率等视频相关参数
            //当音频不选择复制，改变比特率相关参数
            try
            {
                Log.Text = await Task.Run(() => RunProcess(strCmd));
                Log.Text = "完成!";
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
            //创建一个ProcessStartInfo对象 并设置相关属性
            var oInfo = new ProcessStartInfo(@".\ffmpeg.exe", Parameters)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };
            //创建一个字符串和StreamReader 用来获取处理结果
            string output = null;
            StreamReader srOutput = null;
            //调用ffmpeg开始处理命令
            var proc = Process.Start(oInfo);
            //proc.WaitForExit();
            //获取输出流
            srOutput = proc.StandardError;
            //转换成string
            output = srOutput.ReadToEnd();
            //关闭处理程序
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
