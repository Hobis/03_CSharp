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
                    string t_basePath = Environment.CurrentDirectory;
                    string t_addPath = (string)args[0];
                    string t_path = Path.Combine(t_basePath, t_addPath);
                    //MessageBox.Show("t_path: " + t_path);
                    //Process.Start(t_path,);
                    ProcessStartInfo t_psi = new ProcessStartInfo();
                    t_psi.WorkingDirectory = Path.GetDirectoryName(t_path);
                    //MessageBox.Show("t_psi.WorkingDirectory: " + t_psi.WorkingDirectory);
                    t_psi.FileName = Path.GetFileName(t_path);
                    //MessageBox.Show("t_psi.FileName: " + t_psi.FileName);
                    Process.Start(t_psi);

                    break;
                }

            case Win_Message_Types.Win_Center_Location:
                {
                    this.CenterToScreen();

                    break;
                }

            case Win_Message_Types.Win_Exit:
                {
                    this.Close();

                    break;
                }

            case Win_Message_Types.Win_Copy_Folder:
                {
                    DialogResult t_dr = this.folderBrowserDialog1.ShowDialog();
                    string t_path = Environment.CurrentDirectory;
                    string t_path2 = this.folderBrowserDialog1.SelectedPath;                    
                    //MessageBox.Show("t_path: " + t_path);
                    //MessageBox.Show("t_path2: " + t_path2);
                    try
                    {
                        DirectoryCopy(t_path, t_path2, true);
                    }
                    catch (Exception) { }

                    //string t_path3 = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    //String t_name = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
                    //MessageBox.Show("t_path3: " + t_path3);
                    //MessageBox.Show("t_name: " + t_name);
                    //CreateShortCut(t_path, t_path3, t_name);

                    string t_path3 = Path.Combine(t_path2, "main1.exe");
                    //MessageBox.Show("t_path3: " + t_path3);
                    //appShortcutToDesktop(t_path3);
                    //CreateShortcut();

                    break;
                }
            }
        }


        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
/*
        private void CreateShortCut(string shortCutPath, string originalPath, string file)
        {
            WshShellClass WshShell = new WshShellClass();


            IWshRuntimeLibrary.IWshShortcut objShortcut;

            // 바로가기를 저장할 경로를 지정한다.
            objShortcut = (IWshRuntimeLibrary.IWshShortcut)WshShell.CreateShortcut(shortCutPath + "\\" + file.Substring(0, file.LastIndexOf(".")) + ".lnk");

            // 바로가기에 프로그램의 경로를 지정한다.
            objShortcut.TargetPath = originalPath + "\\" + file;
            // 시작 위치를 지정한다.

            objShortcut.WorkingDirectory = originalPath;

            // 바로가기의 description을 지정한다.
            objShortcut.Description = "어쩌구 바로가기";

            // 바로가기 아이콘을 지정한다.
            objShortcut.IconLocation = originalPath + "\\" + file;

            // 바로가기를 저장한다.
            objShortcut.Save();
        }*/
/*
        private static void appShortcutToDesktop(string linkName)
        {
            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            using (StreamWriter writer = new StreamWriter(deskDir + "\\" + linkName + ".url"))
            {
                string app = System.Reflection.Assembly.GetExecutingAssembly().Location;
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=file:///" + app);
                writer.WriteLine("IconIndex=0");
                string icon = app.Replace('\\', '/');
                writer.WriteLine("IconFile=" + icon);
                writer.Flush();
            }
        }*/
        /*
        private void CreateShortcut()
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\Notepad.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "New shortcut for a Notepad";
            shortcut.Hotkey = "Ctrl+Shift+N";
            shortcut.TargetPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\notepad.exe";
            shortcut.Save();
        }*/

    }
}
