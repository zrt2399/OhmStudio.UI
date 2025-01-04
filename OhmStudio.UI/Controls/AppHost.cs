using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace OhmStudio.UI.Controls
{
    /// <summary>
    /// 表示可以将外部exe进程嵌入的控件。
    /// </summary>
    public class AppHost : Control
    {
        private const int GWL_STYLE = -16;
        private const int WS_CAPTION = 0x00C00000;
        //private const int WS_THICKFRAME = 0x00040000;
        private const int WS_BORDER = 0x00800000;

        private Process _process = null;
        private System.Windows.Forms.Panel PART_Host;

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

        public static readonly DependencyProperty ExePathProperty =
            DependencyProperty.Register(nameof(ExePath), typeof(string), typeof(AppHost), new PropertyMetadata(string.Empty, (sender, e) =>
            {
                if (sender is AppHost appHost)
                {
                    appHost.StartAndEmbedProcess();
                }
            }));

        public static readonly DependencyProperty ArgumentsProperty =
            DependencyProperty.Register(nameof(Arguments), typeof(string), typeof(AppHost), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsSingleInstanceProperty =
            DependencyProperty.Register(nameof(IsSingleInstance), typeof(bool), typeof(AppHost), new PropertyMetadata(true));

        static AppHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AppHost), new FrameworkPropertyMetadata(typeof(AppHost)));
        }

        public string ExePath
        {
            get => (string)GetValue(ExePathProperty);
            set => SetValue(ExePathProperty, value);
        }

        public string Arguments
        {
            get => (string)GetValue(ArgumentsProperty);
            set => SetValue(ArgumentsProperty, value);
        }

        public bool IsSingleInstance
        {
            get => (bool)GetValue(IsSingleInstanceProperty);
            set => SetValue(IsSingleInstanceProperty, value);
        }

        public bool IsInit { get; private set; }

        public IntPtr EmbededWindowHandle => _process?.MainWindowHandle ?? IntPtr.Zero;

        public override void OnApplyTemplate()
        {
            if (PART_Host != null)
            {
                PART_Host.Resize -= PART_Host_Resize;
            }
            base.OnApplyTemplate();
            PART_Host = (GetTemplateChild("PART_Host") as WindowsFormsHost).Child as System.Windows.Forms.Panel;
            PART_Host.Resize += PART_Host_Resize;
            if (!IsInit)
            {
                IsInit = true;
                StartAndEmbedProcess();
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongW")]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint newLong);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongW")]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private void PART_Host_Resize(object sender, EventArgs e)
        {
            SetBounds();
        }

        /// <summary>
        /// 激活嵌入的窗口。
        /// </summary>
        public void ActivateWindow()
        {
            if (_process == null || EmbededWindowHandle == IntPtr.Zero)
            {
                return;
            }

            SendMessage(EmbededWindowHandle, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        public void SetBounds()
        {
            SetBounds(PART_Host.Width, PART_Host.Height);
        }

        public void SetBounds(int width, int height)
        {
            if (_process == null || EmbededWindowHandle == IntPtr.Zero)
            {
                return;
            }

            if (width <= 0 || height <= 0)
            {
                return;
            }

            MoveWindow(EmbededWindowHandle, 0, 0, width, height, true);

            ActivateWindow();//激活
        }

        private static Process GetRunningProcess(string processName, string targetExePath)
        {
            // 获取所有与指定名称匹配的进程
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process process in processes)
            {
                var exePath = process.MainModule?.FileName;

                // 比较路径，确保是目标的 .exe 文件
                if (exePath?.Equals(targetExePath, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return process;
                }
            }

            return null;
        }

        public async void StartAndEmbedProcess()
        {
            if (!IsInit)
            {
                return;
            }
            string processPath = ExePath;
            var runningProcess = GetRunningProcess(Path.GetFileNameWithoutExtension(processPath), processPath);
            if (runningProcess != null && IsSingleInstance)
            {
                KillProcess(runningProcess);
            }

            if (string.IsNullOrWhiteSpace(ExePath))
            {
                KillEmbedProcess();
                return;
            }
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo(processPath);
                //info.WindowStyle = ProcessWindowStyle.Minimized;//默认最大化，不弹出界面。
                //info.Arguments = $"-popupwindow";//Unity的命令行参数 
                //processStartInfo.WorkingDirectory = Directory.GetParent(processPath)?.FullName;
                processStartInfo.Arguments = Arguments;
                processStartInfo.CreateNoWindow = true;

                _process = Process.Start(processStartInfo);

                if (_process == null)
                {
                    return;
                }

                _process.WaitForInputIdle();
                var hostHandle = PART_Host.Handle;
                await Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < 100 && EmbededWindowHandle == IntPtr.Zero; i++)
                    {
                        Thread.Sleep(10);
                    }
                    if (EmbededWindowHandle != IntPtr.Zero)
                    {
                        if (EmbedApp(hostHandle, EmbededWindowHandle))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                SetBounds();
                            });
                        }
                        else
                        {
                            KillEmbedProcess();
                        }
                    }
                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception)
            {
                KillEmbedProcess();
                throw;
            }
        }

        /// <summary>
        /// 将外进程嵌入到当前程序。
        /// </summary>
        /// <param name="panelHwnd"></param>
        /// <param name="processHwnd"></param>
        /// <returns></returns>
        private bool EmbedApp(IntPtr panelHwnd, IntPtr processHwnd)
        {
            //是否嵌入成功标志，用作返回值
            var isEmbedSuccess = false;

            var wndStyle = GetWindowLong(processHwnd, GWL_STYLE);
            wndStyle &= ~WS_BORDER;
            wndStyle &= ~WS_CAPTION;
            SetWindowLong(processHwnd, GWL_STYLE, (uint)wndStyle);

            if (processHwnd != (IntPtr)0 && panelHwnd != (IntPtr)0)
            {
                //把本窗口句柄与目标窗口句柄关联起来
                var setTime = 0;
                while (!isEmbedSuccess && setTime < 100)
                {
                    // Put it into this form
                    isEmbedSuccess = SetParent(processHwnd, panelHwnd) != IntPtr.Zero;
                    Thread.Sleep(10);
                    setTime++;
                }

                // Remove border and whatnot
                //Win32Api.SetWindowLong(processHwnd, Win32Api.GWL_STYLE, Win32Api.WS_CHILDWINDOW | Win32Api.WS_CLIPSIBLINGS | Win32Api.WS_CLIPCHILDREN | Win32Api.WS_VISIBLE);

                //Move the window to overlay it on this window
                //Win32Api.MoveWindow(EmbededWindowHandle, 0, 0, (int)ActualWidth, (int)ActualHeight, true);
            }

            return isEmbedSuccess;
        }

        private void KillProcess(Process process)
        {
            if (process != null && !process.HasExited)
            {
                process.CloseMainWindow();
                //process.Close();
                //process.Dispose();
                process.Kill();
            }
        }

        /// <summary>
        /// 关闭当前嵌入的进程。
        /// </summary>
        public void KillEmbedProcess()
        {
            KillProcess(_process);
            _process = null;
        }
    }
}