using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace UserNotify
{
    public partial class MessageForm : Form
    {
        public string heading = "";
        public string text = "";
        public string server = "";
        public int port = 0;
        public string page = "";

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public System.Windows.Forms.Timer buttonTimer = new System.Windows.Forms.Timer();
        public int buttons = 5;

        public System.Net.WebClient wc = new System.Net.WebClient();

        public System.Windows.Forms.Timer fadeTimer = new System.Windows.Forms.Timer();
        public System.Windows.Forms.Timer flashTimer = new System.Windows.Forms.Timer();
        public double fade = 0;

        public MessageForm(string heading, string text, string server , int port, string page)
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;

            this.GotFocus += MessageForm_GotFocus;

            messageTextBox.GotFocus += MessageTextBox_GotFocus;

            fadeTimer.Interval = 50;
            fadeTimer.Tick += FadeTimer_Tick;

            flashTimer.Interval = 1000;
            flashTimer.Tick += FlashTimer_Tick;
            flashTimer.Start();
            

            this.FormBorderStyle = FormBorderStyle.None;
            messageTextBox.BorderStyle = BorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            buttonTimer.Interval = 1000;
            buttonTimer.Tick += ButtonTimer_Tick;

            this.heading = heading;
            this.text = text;
            this.server = server;
            this.port = port;
            this.page = page;


            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = System.Drawing.SystemIcons.Application;
            WebRequestAndClose("http://" + server + ":" + port + "/api/values/" + page + "-Displayed", false);
            

            this.TopMost = true;
            this.Text = heading;
            this.titleText.Text = heading;

            messageTextBox.Text = text;
           
            this.Click += MessageForm_Click;

            okBtn.Enabled = false;
            noBtn.Enabled = false;

            this.MouseDown += MessageForm_MouseDown;
            middleMessageText.MouseDown += Label1_MouseDown;
            messageTextBox.MouseDown += MessageTextBox_MouseDown;
            titleText.MouseDown += TitleText_MouseDown;

            this.Opacity = 0;
          
        }

        private void TitleText_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int WM_NCLBUTTONDOWN = 0xA1;
                int HT_CAPTION = 0x2;
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
            catch
            {

            }
            buttonTimer.Start();
            middleMessageText.Text = "Please read the message";
        }

        private void MessageTextBox_GotFocus(object sender, EventArgs e)
        {
            messageTextBox.DeselectAll();
            okBtn.Focus();
        }

        private void MessageForm_GotFocus(object sender, EventArgs e)
        {
           
        }

        private void MessageTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int WM_NCLBUTTONDOWN = 0xA1;
                int HT_CAPTION = 0x2;
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
            catch
            {

            }
            buttonTimer.Start();
            middleMessageText.Text = "Please read the message";
        }

        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            if (middleMessageText.Visible)
            {
                middleMessageText.Visible = false;
            }
            else
            {
                middleMessageText.Visible = true;
            }
            
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            this.Opacity = Convert.ToDouble(fade);
            fade += 0.05;
            messageTextBox.DeselectAll();
            if (fade == 1) { fadeTimer.Stop(); }
        }

        private void Label1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int WM_NCLBUTTONDOWN = 0xA1;
                int HT_CAPTION = 0x2;
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
            catch
            {

            }
            buttonTimer.Start();
            middleMessageText.Text = "Please read the message";
        }

        private void ButtonTimer_Tick(object sender, EventArgs e)
        {
            okBtn.Text = buttons.ToString();
            
            if (buttons == 0)
            {
                okBtn.Text = "Ok";
                okBtn.Enabled = true;
                noBtn.Enabled = true;
                buttonTimer.Stop();
                messageTextBox.Visible = true;
                flashTimer.Stop();
            }
            else
            {
                buttons--;
            }
            
        }

        private void MessageForm_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int WM_NCLBUTTONDOWN = 0xA1;
                int HT_CAPTION = 0x2;
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
            catch
            {

            }
            buttonTimer.Start();
            middleMessageText.Text = "Please read the message";
        }

        private void MessageForm_Click(object sender, EventArgs e)
        {
            noBtn.Enabled = true;
            okBtn.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(CallClicked, "");
            this.Close();
        }

        private void CallClicked(object test)
        {
            WebRequestAndClose("http://" + server + ":" + port + "/api/values/" + page + "-Clicked", false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(CallUClicked, "");
            this.Close();
        }

        private void CallUClicked(object test)
        {
            WebRequestAndClose("http://" + server + ":" + port + "/api/values/" + page + "-UClicked", false);
        }

        private void MessageForm_Load(object sender, EventArgs e)
        {
            fadeTimer.Start();
            messageTextBox.DeselectAll();
        }

        string WebRequestAndClose(string URL, bool requireReqply)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

            string result = "";
            try
            {
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.01; Windows NT 5.0)";

                request.Timeout = 2000;
                request.ReadWriteTimeout = 2000;

                var response = request.GetResponse();

                if (requireReqply)
                {
                   
                    using (Stream s = response.GetResponseStream())
                    {
                        char[] buffer = new char[4096];
                        StringBuilder sb = new StringBuilder();
                        using (StreamReader sr = new StreamReader(s, System.Text.Encoding.UTF8))
                        {
                            for (int read = sr.Read(buffer, 0, 4096); read != 0; read = sr.Read(buffer, 0, 4096))
                            {
                                sb.Append(buffer, 0, read);
                            }
                            result = sb.ToString();
                        }
                    }
                }
                request.Abort();
            }
            catch
            {
                request.Abort();
            }

            return result;
        }
    }
}
