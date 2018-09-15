using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Accord.Video.FFMPEG;

using AForge.Video;
using AForge.Video.DirectShow;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using NAudio.Wave;

namespace ScreenRecorderNew
{
    internal class MainWindowViewModel 
    {
        #region Private fields
        private IVideoSource _videoSource;
        private VideoFileWriter _writer;
        private bool _recording;
        private DateTime? _firstFrameTime;
        public NAudio.Wave.WaveIn waveSource = null;
        public NAudio.Wave.WaveFileWriter waveFile = null;
        bool _isAudio = true;
        PictureBox _PictureBox;
        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            heigth = 480;
            widht = 640;
            VideoDevices = new List<FilterInfo>();
           
            IsDesktopSource = true;
            IpCameraUrl = "http://88.53.197.250/axis-cgi/mjpg/video.cgi?resolution=320×240";
        }

        #endregion

        #region Properties

        public List<FilterInfo> VideoDevices { get; set; }

        public bool IsDesktopSource { get; set; }

        public bool IsWebcamSource { get; set; }

        public bool IsIpCameraSource { get; set; }

        public string IpCameraUrl { get; set; }

        public FilterInfo CurrentDevice { get; set; }

     
        #endregion

        public List<FilterInfo> GetVideoDevices()
        {
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in devices)
            {
                VideoDevices.Add(device);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
           
            return VideoDevices;
        }

        public void StartCamera(FilterInfo filterInfo = null,PictureBox pictureBox=null)
        {
            if (IsDesktopSource)
            {
               
                var rectangle = new Rectangle();
                foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                {
                    rectangle = Rectangle.Union(rectangle, screen.Bounds);
                }
                heigth = rectangle.Height;
                widht = rectangle.Width;
                _videoSource = new ScreenCaptureStream(rectangle);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();
            }
            else if (IsWebcamSource)
            {
                _PictureBox = pictureBox;
                CurrentDevice = filterInfo;
                heigth = 480;
                widht = 640;
                if (CurrentDevice != null)
                {
                    _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                    
                    _videoSource.NewFrame += video_NewFrame;
                    _videoSource.Start();
                }
                else
                {
                    MessageBox.Show("Current device can't be null");
                }
            }
            else if (IsIpCameraSource)
            {
                _videoSource = new MJPEGStream(IpCameraUrl);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        const Int32 CURSOR_SHOWING = 0x00000001;

        Bitmap CaptureScreen(bool CaptureMouse,Bitmap bitmapinput)
        {
            Bitmap result = bitmapinput;

            try
            {
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

                    if (CaptureMouse)
                    {
                        CURSORINFO pci;
                        pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                        if (GetCursorInfo(out pci))
                        {
                            if (pci.flags == CURSOR_SHOWING)
                            {
                                DrawIcon(g.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
                                g.ReleaseHdc();
                            }
                        }
                    }
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }
        int heigth, widht;
        public void CopyTo(byte[] Buffer, int Length, Bitmap bitmap)
        {

            var bits = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

            Parallel.For(0, bitmap.Height, Y =>
            {
                var absStride = Math.Abs(bits.Stride);

                Marshal.Copy(bits.Scan0 + (Y * bits.Stride), Buffer, Y * absStride, absStride);
            });

            bitmap.UnlockBits(bits);
        }
        bool _firstFrame = true;
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if(IsWebcamSource)
                {
                    _PictureBox.Image = (Bitmap)eventArgs.Frame.Clone();
                }
                if (_recording)
                {
                    using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                    {
                        Bitmap bitmapnew;
                        bitmapnew = bitmap;
                        if (IsDesktopSource)
                        {
                            bitmapnew = CaptureScreen(true,bitmap);
                        }
                        var _videoBuffers = ImageToByte(bitmapnew);
                        var bits = bitmapnew.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmapnew.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

                        Parallel.For(0, heigth, Y =>
                        {
                            var absStride = Math.Abs(bits.Stride);

                            Marshal.Copy(bits.Scan0 + (Y * bits.Stride), _videoBuffer, Y * absStride, absStride);
                        });

                        bitmapnew.UnlockBits(bits);
                        if (_firstFrameTime != null)
                        {
                            _lastFrameTask?.Wait();
                            _lastFrameTask = _ffmpegIn.WriteAsync(_videoBuffer, 0, _videoBuffer.Length);
                        }
                        else
                        {
                            if (_firstFrame)
                            {
                                if (!WaitForConnection(_ffmpegIn, 5000))
                                {
                                    throw new Exception("Cannot connect Video pipe to FFmpeg");
                                }
                                _firstFrame = false;
                            }
                            _firstFrameTime = DateTime.Now;
                            _lastFrameTask?.Wait();
                            _lastFrameTask = _ffmpegIn.WriteAsync(_videoBuffer, 0, _videoBuffer.Length);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                //MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButtons.OK,
                //    MessageBoxIcon.Error);
                //StopCamera();
            }
        }
        
        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        Task _lastFrameTask;
        public void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= video_NewFrame;
            }
           // Image = null;
        }

        public void StopRecording()
        {
            _recording = false;
            _videoSource.SignalToStop();
            _videoSource.NewFrame -= video_NewFrame;
            _ffmpegIn.Flush();
            _ffmpegIn.Dispose();
            if (_isAudio)
            {
                waveSource.StopRecording();
                _audioPipe.Flush();
                _audioPipe.Dispose();
            }
           Process.WaitForExit();
          //  MessageBox.Show((startTime - DateTime.Now).TotalSeconds.ToString());
            // _writer.Close();
            //_writer.Dispose();
        }
        DateTime startTime;
        public void StartRecording(bool IsAudio,int AudioDeviceIndex=-1,int Channel=0)
        {
            _isAudio = IsAudio;
            _firstFrameTime = null;
            startTime = DateTime.Now;
            ffmpegWriter();
            _recording = true;
            if (IsAudio)
            {
                StartAudioRecord(AudioDeviceIndex,Channel);
            }
            
        }
        public static Process StartFFmpeg(string Arguments, string OutputFileName)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "ffmpeg.exe",
                    Arguments = Arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,

                },
                EnableRaisingEvents = true
            };
            process.ErrorDataReceived += (s, e) => ProcessTheErrorData(s, e);
            process.Start();
            process.BeginErrorReadLine();
            return process;
        }

        private static void ProcessTheErrorData(object sender, DataReceivedEventArgs e)
        {
            // throw new NotImplementedException();
        }

        NamedPipeServerStream _ffmpegIn;
        NamedPipeServerStream _audioPipe;
        byte[] _videoBuffer;
        const string PipePrefix = @"\\.\pipe\";
        public void ffmpegWriter()
        {
            if (File.Exists(Program.Localpath + "\\output.mp4"))
            {
                File.Delete(Program.Localpath + "\\output.mp4");
            }

            _videoBuffer = new byte[widht * heigth * 4];
            var audioPipeName = GetPipeName();
            var videoPipeName = GetPipeName();
            string audioInArgs="", audioOutArgs = "";

            var videoInArgs = $@" -thread_queue_size 512 -use_wallclock_as_timestamps 1 -f rawvideo -pix_fmt rgb32 -video_size {widht}x{heigth} -i \\.\pipe\{videoPipeName}";
            // var videoOutArgs = $"-vcodec libx264 -crf 15 -pix_fmt yuv420p -preset ultrafast -r 10";
            var videoOutArgs = "-vcodec libx264 -crf 25 -pix_fmt yuv420p -preset ultrafast -r 10";
            if (_isAudio)
            {
                audioInArgs = $" -thread_queue_size 512 -f s16le -acodec pcm_s16le -ar 44100 -ac 2 -i {PipePrefix}{audioPipeName}";
                //audioOutArgs = "-c:a aac -strict -2 -b:a 256k";
                audioOutArgs = "-c:a aac -strict -2 -b:a 256k";
                  var audioBufferSize = (int)((1000.0 / 10) * 44.1 * 2 * 2 * 2);
                _audioPipe = new NamedPipeServerStream(audioPipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, audioBufferSize);
                _ffmpegIn = new NamedPipeServerStream(videoPipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, _videoBuffer.Length);
                Process = StartFFmpeg($"{videoInArgs} {audioInArgs} {videoOutArgs} {audioOutArgs} \"{Program.Localpath + "\\output.mp4"}\"", Program.Localpath + "\\output.mp4");
            }
            else
            {
                _ffmpegIn = new NamedPipeServerStream(videoPipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, _videoBuffer.Length);
                Process = StartFFmpeg($"{videoInArgs} {videoOutArgs} \"{Program.Localpath + "\\output.mp4"}\"", Program.Localpath + "\\output.mp4");
            }
        }
        Process Process;
        bool WaitForConnection(NamedPipeServerStream ServerStream, int Timeout)
        {
            var asyncResult = ServerStream.BeginWaitForConnection(Ar => { }, null);

            if (asyncResult.AsyncWaitHandle.WaitOne(Timeout))
            {
                ServerStream.EndWaitForConnection(asyncResult);

                return ServerStream.IsConnected;
            }

            return false;
        }
        static string GetPipeName() => $"record-{Guid.NewGuid()}";
      

       

        #region Record Audio
        void StartAudioRecord(int AudioDeviceIndex,int Channel)
        {
            waveSource = new NAudio.Wave.WaveIn();
            waveSource.WaveFormat = new NAudio.Wave.WaveFormat(44100, Channel);
            waveSource = new WaveIn
            {
                DeviceNumber = AudioDeviceIndex,
                BufferMilliseconds = (int)Math.Ceiling( (double)(1000 /10)),
                NumberOfBuffers = 3,
                WaveFormat = new  WaveFormat(44100, 16, Channel)
        };
            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);
           // waveFile = new NAudio.Wave.WaveFileWriter(Program.Localpath+"\\raw.wav", waveSource.WaveFormat);
            waveSource.StartRecording();
        }
        private void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }
            if (waveFile != null)
            {
              //  waveFile.Dispose();
              //  waveFile = null;
            }
        }
        bool _firstAudio = true;
        Task _lastAudio;
        private void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            //if (waveFile != null)
           // {
              //  waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                //waveFile.Flush();
                if (_firstAudio)
                {
                    if (!WaitForConnection(_audioPipe,5000))
                    {
                        throw new Exception("Cannot connect Audio pipe to FFmpeg");
                    }

                    _firstAudio = false;
                }

                _lastAudio?.Wait();

                _lastAudio = _audioPipe.WriteAsync(e.Buffer, 0, e.Buffer.Length);
           // }
        }
        #endregion
    }
}
