using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BalloonNotify
{
    public partial class Form1 : Form
    {
        public string server = "None";

        public System.Windows.Forms.Timer checkTimer = new System.Windows.Forms.Timer();
        public System.Windows.Forms.Timer messageTimer = new System.Windows.Forms.Timer();
        public Stopwatch stopwatch = new Stopwatch();
        public Stopwatch updatestopwatch = new Stopwatch();

        public int X = 0;
        public int Y = 0;

        public bool MouseActive = false;
        public bool Updated = false;


        public Form1()
        {
            this.Visible = false;
            InitializeComponent();


            messageTimer.Interval = 1000;
            messageTimer.Tick += MessageTimer_Tick;
            messageTimer.Start();

            checkTimer.Interval = 1000;
            checkTimer.Tick += CheckTimer_Tick;
            checkTimer.Start();

            stopwatch.Start();
            updatestopwatch.Start();

            server = File.ReadAllText("Server.txt");
            if (server == "None" || server == "")
            {
                Application.Exit();
            }

        }

        private void MessageTimer_Tick(object sender, EventArgs e)
        {
            List<string> checkList = new List<string>();
            checkList.Add("default");
            checkList.Add(Environment.GetEnvironmentVariable("COMPUTERNAME"));

            foreach (string name in checkList)
            {
                try
                {
                    System.Net.WebClient wc = new System.Net.WebClient();
                    string webData = wc.DownloadString("http://" + server + "/" + name + ".html");
                    var notification = new System.Windows.Forms.NotifyIcon()
                    {
                        Visible = true,
                        Icon = System.Drawing.SystemIcons.Application,
                        BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info,
                        BalloonTipTitle = webData[0].ToString(),
                        BalloonTipText = webData[1].ToString(),
                    };

                    // Display for 5 seconds.
                    notification.ShowBalloonTip(500000000);

                    notification.BalloonTipClicked += Notification_BalloonTipClicked;
                    notification.BalloonTipClosed += Notification_BalloonTipClosed;
                    notification.BalloonTipShown += Notification_BalloonTipShown;
                    
                }
                catch (Exception f)
                {
                    //MessageBox.Show(f.Message);
                }
            }
            
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (updatestopwatch.Elapsed.Seconds > 60)
            {
                Updated = false;
                updatestopwatch.Restart();
            }


            if (MousePosition.X != X && MousePosition.Y != Y)
            {
               
                if (MouseActive == false && Updated  == false)
                {
                    try
                    {
                        System.Net.WebClient wc = new System.Net.WebClient();
                        string webData = wc.DownloadString("http://" + server + "/" + Environment.GetEnvironmentVariable("COMPUTERNAME") + "-Active.html");
                        MouseActive = true;
                        Updated = true;
                    }
                    catch
                    {
                        Updated = false;
                    }
                  
                }
                stopwatch.Restart();
            }

            if (stopwatch.Elapsed.Seconds > 360 && Updated == false)
            {
                
                try
                {
                    System.Net.WebClient wc = new System.Net.WebClient();
                    string webData = wc.DownloadString("http://" + server + "/" + Environment.GetEnvironmentVariable("COMPUTERNAME") + "-InActive.html");
                    MouseActive = false;
                    Updated = true;
                }
                catch
                {
                    Updated = false;
                }
            }

            X = MousePosition.X;
            Y = MousePosition.Y;
        }





        private void Form1_Load(object sender, EventArgs e)
        {
            
        }



        private void Notification_BalloonTipShown(object sender, EventArgs e)
        {
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                wc.DownloadString("http://" + server + "/" + Environment.GetEnvironmentVariable("COMPUTERNAME") + "-Displayed.html");
            }
            catch (Exception f)
            {
                //MessageBox.Show(f.Message);
            }

        }

        private void Notification_BalloonTipClosed(object sender, EventArgs e)
        {
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                string webData = wc.DownloadString("http://" + server + "/" + Environment.GetEnvironmentVariable("COMPUTERNAME") + "-Clicked.html");
            }
            catch
            {

            }
        }

        private void Notification_BalloonTipClicked(object sender, EventArgs e)
        {
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                string webData = wc.DownloadString("http://" + server + "/" + Environment.GetEnvironmentVariable("COMPUTERNAME") + "-Clicked.html");
            }
            catch
            {

            }
        }





  

 


    }
}
