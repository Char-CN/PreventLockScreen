using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace PreventLockScreen
{
    public partial class Form1 : Form
    {
        #region Windows API 声明
        [DllImport("kernel32.dll")]
        private static extern uint SetThreadExecutionState(uint esFlags);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern IntPtr OpenDesktop(string hDesktop, int Flags, bool Inherit, uint DesiredAccess);

        [DllImport("user32.dll")]
        private static extern bool CloseDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        private static extern bool SwitchDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        private static extern IntPtr GetThreadDesktop(uint dwThreadId);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();
        #endregion

        #region 常量定义
        private const uint ES_CONTINUOUS = 0x80000000;
        private const uint ES_DISPLAY_REQUIRED = 0x00000002;
        private const int VK_NUMLOCK = 0x90;
        private const int TIMER_INTERVAL = 30000; // 30秒
        private const int MAX_CLICK_COUNT = 10000; // 最大点击次数
        private const int KEYBD_EVENT_KEYUP = 2;
        private const uint DESKTOP_SWITCHDESKTOP = 0x0100;
        #endregion

        #region 私有字段
        private Timer _timer;
        private bool _isRunning;
        private NotifyIcon _notifyIcon;
        private int _clickCount;
        #endregion

        public Form1()
        {
            InitializeComponent();
            InitializeForm();
            InitializeTimer();
            InitializeTrayIcon();
            UpdateUIState();
        }

        #region 初始化方法
        private void InitializeForm()
        {
            _isRunning = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ShowInTaskbar = false;
        }

        private void InitializeTimer()
        {
            _timer = new Timer { Interval = TIMER_INTERVAL };
            _timer.Tick += Timer_Tick;
        }

        private void InitializeTrayIcon()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));

            _notifyIcon = new NotifyIcon
            {
                Icon = (Icon)resources.GetObject("$this.Icon"),
                Text = "防锁屏助手",
                Visible = true
            };

            CreateTrayContextMenu();
            _notifyIcon.DoubleClick += (s, e) => ShowWindow();
        }

        private void CreateTrayContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("显示窗口", null, (s, e) => ShowWindow());
            contextMenu.Items.Add("退出", null, (s, e) => ExitApplication());
            _notifyIcon.ContextMenuStrip = contextMenu;
        }
        #endregion

        #region 屏幕锁屏状态检测
        /// <summary>
        /// 检测是否处于屏幕锁屏状态
        /// </summary>
        /// <returns>如果屏幕被锁定返回true，否则返回false</returns>
        private bool IsScreenLocked()
        {
            try
            {
                // 方法1：尝试打开默认桌面
                IntPtr desktop = OpenDesktop("default", 0, false, DESKTOP_SWITCHDESKTOP);
                if (desktop == IntPtr.Zero)
                {
                    return true; // 无法打开默认桌面，可能被锁定
                }

                // 获取当前线程的桌面
                IntPtr currentDesktop = GetThreadDesktop(GetCurrentThreadId());
                bool isLocked = desktop != currentDesktop;

                CloseDesktop(desktop);
                return isLocked;
            }
            catch
            {
                // 如果检测失败，为了安全起见，假设没有锁屏
                return false;
            }
        }

        /// <summary>
        /// 备用方法：通过检测屏保状态判断是否锁屏
        /// </summary>
        /// <returns>如果检测到屏保活动返回true</returns>
        private bool IsScreenSaverActive()
        {
            try
            {
                // 通过SystemParametersInfo检测屏保状态
                bool isActive = false;
                SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, ref isActive, 0);
                return isActive;
            }
            catch
            {
                return false;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref bool pvParam, uint fWinIni);

        private const uint SPI_GETSCREENSAVERRUNNING = 0x0072;
        #endregion

        #region 窗口操作
        private void ShowWindow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void ExitApplication()
        {
            _notifyIcon.Visible = false;
            Application.Exit();
        }
        #endregion

        #region 防锁屏功能
        private void BtnOnToggle_Click(object sender, EventArgs e)
        {
            _isRunning = !_isRunning;
            UpdateUIState();
        }

        private void UpdateUIState()
        {
            if (_isRunning)
            {
                StartAntiLock();
            }
            else
            {
                StopAntiLock();
            }

            UpdateUI();
        }

        private void StartAntiLock()
        {
            SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED);
            _timer.Start();
        }

        private void StopAntiLock()
        {
            SetThreadExecutionState(ES_CONTINUOUS);
            _timer.Stop();
        }

        private void UpdateUI()
        {
            var statusText = _isRunning ? "已开启" : "已关闭";
            var statusColor = _isRunning ? Color.Green : Color.Red;
            var buttonText = _isRunning ? "关闭防锁屏" : "开启防锁屏";

            msg.Text = $"状态：{statusText}";
            msg.ForeColor = statusColor;
            btn_on.Text = buttonText;

            _notifyIcon.Text = $"防锁屏状态 - {statusText}";
        }
        #endregion

        #region 定时器事件
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                // 检测屏幕是否被锁定，如果被锁定则不执行防锁屏操作
                if (IsScreenLocked() || IsScreenSaverActive())
                {
                    // 屏幕已锁定或屏保激活，跳过本次执行
                    return;
                }

                ExecuteAntiLockAction();
                ManageClickCount();
            }
            catch (Exception)
            {
                // 记录异常并重启定时器
                RestartTimer();
            }
        }

        private void ExecuteAntiLockAction()
        {
            Debug.WriteLine("_clickCount + 1  ->  _clickCount: " + _clickCount);
            SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED);
            PressNumLock();
            _clickCount++;
        }

        private void ManageClickCount()
        {
            if (_clickCount >= MAX_CLICK_COUNT)
            {
                RestartTimer();
                _clickCount = 0;
            }
        }

        private void RestartTimer()
        {
            _timer.Stop();
            _timer.Start();
        }

        private void PressNumLock()
        {
            keybd_event((byte)VK_NUMLOCK, 0, 0, 0); // 按下
            keybd_event((byte)VK_NUMLOCK, 0, KEYBD_EVENT_KEYUP, 0); // 释放
        }
        #endregion

        #region 窗体生命周期
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                MinimizeToTray(e);
                return;
            }

            CleanupResources();
            base.OnFormClosing(e);
        }

        private void MinimizeToTray(FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            this.ShowInTaskbar = false;
            _notifyIcon.ShowBalloonTip(1000, "提示", "程序已最小化到托盘", ToolTipIcon.Info);
        }

        private void CleanupResources()
        {
            if (_isRunning)
            {
                SetThreadExecutionState(ES_CONTINUOUS);
                _timer?.Stop();
            }

            _timer?.Dispose();
            _notifyIcon?.Dispose();
        }
        #endregion
    }
}