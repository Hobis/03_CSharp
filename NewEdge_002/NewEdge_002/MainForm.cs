using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NewEdge_002
{
    public sealed partial class MainForm : Form
    {
        // :: 생성자
        public MainForm()
        {
            InitializeComponent();
            this.p_InitOnce();
        }

        // :: 한번 초기화
        private void p_InitOnce()
        {
            this.Text = "";
            this.ClientSize = new Size(800, 600);
            this.MinimumSize = this.Size;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.webBrowser1.ObjectForScripting = this;
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.AllowWebBrowserDrop = false;
            this.webBrowser1.ScrollBarsEnabled = false;
            this.webBrowser1.ScriptErrorsSuppressed = false;
            this.webBrowser1.WebBrowserShortcutsEnabled = false;
        }

        // -
        private const string _TakeOver = "?type=!__%40%23%24takeOver";

        // :: 현재 폼 로드완료 (2빠따로 호출됨)
        private void p_This_Load(object sender, EventArgs ea)
        {
            String t_name = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            String t_src = Path.Combine(Environment.CurrentDirectory, t_name + ".html" + _TakeOver);
            this.webBrowser1.Navigate(t_src);
            this.Visible = false;

            //MessageBox.Show("p_This_Load");
        }

        // :: 웹브라우저 Document 로드완료 (1빠따로 호출됨)
        private void p_webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs ebdcea)
        {
            //MessageBox.Show("p_webBrowser1_DocumentCompleted");
        }

        // :: 웹브라우저 키다운 핸들러
        private void p_webBrowser1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs pkdea)
        {
            switch (pkdea.KeyCode)
            {
            case Keys.Escape:
                {
                    this.p_SetFullScreen(false);

                    break;
                }

            case Keys.F5:
                {
                    this.p_Js_Call("p_reload", null);

                    break;
                }
            }
        }

        // -
        private const int WM_SYSCOMMAND = 0x112;
        // -
        private const int SC_MAXIMIZE = 0xf030;
        // ::
        protected override void WndProc(ref Message m)
        {
            if (m.Msg.Equals(WM_SYSCOMMAND))
            {
                if (m.WParam.ToInt32().Equals(SC_MAXIMIZE))
                {
                    this.p_FullScreen_Toggle();
                    return;
                }
            }

            base.WndProc(ref m);
        }

        // :: 풀스크린 토글
        private void p_FullScreen_Toggle()
        {
            if (this.TopMost)
            {
                this.p_SetFullScreen(false);
            }
            else
            {
                this.p_SetFullScreen(true);
            }
        }

        //-
        private Size _tempSize = Size.Empty;
        // :: 풀스크린 설정
        private void p_SetFullScreen(bool b)
        {
            if (b)
            {
                if (!this.TopMost)
                {
                    this.TopMost = true;
                    this._tempSize = this.Size;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                    this.webBrowser1.Focus();
                }
            }
            else
            {
                if (this.TopMost)
                {
                    this.TopMost = false;
                    this.WindowState = FormWindowState.Normal;
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    this.Size = this._tempSize;
                    this._tempSize = Size.Empty;
                }
            }
        }

        // :: Js 호출 보냄
        private void p_Js_Call(string funcName, object[] args)
        {
            try
            {
                this.webBrowser1.Document.InvokeScript(funcName, args);
            }
            catch (Exception)
            {
            }
        }

        // :: Js 호출 받음 노멀
        public void Js_CallBack_n(string type)
        {
            Win_Message_Types t_wmt = (Win_Message_Types)Enum.Parse(typeof(Win_Message_Types), type);
            this.p_Js_CallBack_Core(t_wmt, null);
        }

        // :: Js 호출 받음
        public void Js_CallBack(string type, object[] args)
        {
            Win_Message_Types t_wmt = (Win_Message_Types)Enum.Parse(typeof(Win_Message_Types), type);
            this.p_Js_CallBack_Core(t_wmt, args);
        }

        // :: Js 호출 받음 핵심
        private void p_Js_CallBack_Core(Win_Message_Types wmt, object[] args)
        {
            switch (wmt)
            {
            case Win_Message_Types.Win_Init:
                {
                    //
                    break;
                }

            case Win_Message_Types.Win_Set_Title:
                {
                    string t_name = (string)args[0];
                    this.Text = t_name;

                    break;
                }

            case Win_Message_Types.Win_Set_Visible:
                {
                    bool t_b = (bool)args[0];
                    this.Visible = t_b;

                    break;
                }

            case Win_Message_Types.Win_Set_MinSize:
                {
                    this.WindowState = FormWindowState.Normal;
                    Size t_s = this.Size;
                    t_s.Width = (int)args[0];
                    t_s.Height = (int)args[1];
                    this.MinimumSize = this.DefaultMaximumSize;
                    this.ClientSize = t_s;
                    this.MinimumSize = this.Size;

                    break;
                }


            case Win_Message_Types.Win_Set_Location:
                {
                    this.WindowState = FormWindowState.Normal;
                    Point t_p = this.Location;
                    t_p.X = (int)args[0];
                    t_p.Y = (int)args[1];
                    this.Location = t_p;

                    break;
                }

            case Win_Message_Types.Win_Resize_Max:
                {
                    this.WindowState = FormWindowState.Maximized;

                    break;
                }

            case Win_Message_Types.Win_Resize_Min:
                {
                    this.WindowState = FormWindowState.Minimized;

                    break;
                }

            case Win_Message_Types.Win_Resize_Normal:
                {
                    this.WindowState = FormWindowState.Normal;

                    break;
                }

            case Win_Message_Types.Win_Resize_FullScreen:
                {
                    bool t_b = (bool)args[0];
                    this.p_SetFullScreen(t_b);

                    break;
                }

            case Win_Message_Types.Win_Resize:
                {
                    this.WindowState = FormWindowState.Normal;
                    Size t_s = this.Size;
                    t_s.Width = (int)args[0];
                    t_s.Height = (int)args[1];
                    this.ClientSize = t_s;

                    break;
                }

            case Win_Message_Types.Win_Open:
                {
                    string t_path = (string)args[0];
                    Process.Start(t_path);

                    break;
                }

            case Win_Message_Types.Win_Center_Location:
                {
                    this.CenterToScreen();

                    break;
                }
            }
        }
    }
}
