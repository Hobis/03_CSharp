using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NewEdge_002
{
    public sealed partial class MainForm : Form
    {
        // ::
        public MainForm()
        {
            InitializeComponent();

            this.p_InitOnce();
        }

        // ::
        private void p_InitOnce()
        {
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.MaximizeBox = false;

            Size t_size = this.Size;
            t_size.Width = 1024;
            t_size.Height = 768;
            this.ClientSize = t_size;
            this.MinimumSize = this.Size;
            this.Text = "HorizontalText Ver 7.21";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.webBrowser1.ObjectForScripting = this;
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.AllowWebBrowserDrop = false;
            this.webBrowser1.ScrollBarsEnabled = false;

            String t_name = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            String t_src = Path.Combine(Environment.CurrentDirectory, t_name + ".html");
            this.webBrowser1.Navigate(t_src);
        }

        // ::
        private void p_Load(object sender, EventArgs ea)
        {

        }
    }
}
