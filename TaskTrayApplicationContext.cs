using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace UserNotify
{
    class TaskTrayApplicationContext : ApplicationContext
    {
        NotifyIcon notifyIcon = new NotifyIcon();

        public string server = "None";
        public int port = 8000;

        public System.Windows.Forms.Timer checkTimer = new System.Windows.Forms.Timer();
        public System.Windows.Forms.Timer messageTimer = new System.Windows.Forms.Timer();
        public System.Windows.Forms.Timer messageTimer2 = new System.Windows.Forms.Timer();
        public Stopwatch stopwatch = new Stopwatch();
        public Stopwatch updatestopwatch = new Stopwatch();

        public System.Net.WebClient wc = new System.Net.WebClient();

        public int X = 0;
        public int Y = 0;
        public int idle = 0;
        public int count = 0;

        public bool MouseActive = false;
        public bool Updated = false;

        public string thiscomputername = "";
        public string thisusername = "";

        public TaskTrayApplicationContext()
        {
            string tcn = Environment.GetEnvironmentVariable("COMPUTERNAME");// + new Random().Next(1, 1000).ToString();
            string tun = Environment.GetEnvironmentVariable("USERNAME").Replace("#","hashreplacement");

            

            thiscomputername = tcn;
            thisusername = tun;

            notifyIcon.Visible = false;
            
            messageTimer.Interval = 10000;
            messageTimer.Tick += MessageTimer_Tick;
            messageTimer.Start();

            messageTimer2.Interval = 10000;
            messageTimer2.Tick += MessageTimer2_Tick;
            messageTimer2.Start();

            checkTimer.Interval = 10000;
            checkTimer.Tick += CheckTimer_Tick;
            checkTimer.Start();

            stopwatch.Start();
            updatestopwatch.Start();



            try
            {
                server = File.ReadAllText("Server.txt").Split(':')[0].ToString();
                port = Convert.ToInt32(File.ReadAllText("Server.txt").Split(':')[1].ToString());
            }
            catch
            {
                port = 0; 
                messageTimer.Stop();
                checkTimer.Stop();
                stopwatch.Stop();
                updatestopwatch.Stop();
            }
    
            

            if (server == "None" || server == "" || port == 0)
            {
                Application.Exit();
            }

        }

        string WebRequestAndClose(string URL,bool requireReqply)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            
            string result = "";
            try
            {
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.01; Windows NT 5.0)";
                request.Timeout = 5000;
                request.ReadWriteTimeout = 5000;
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
            catch (Exception f)
            {
                request.Abort();
            }
           
            return result;
        }

        private void MessageTimer2_Tick(object sender, EventArgs e)
        {
            try
            {
                string webData = WebRequestAndClose("http://" + server + ":" + port + "/api/values/" + thiscomputername + "---" + thisusername,true);
                string lastmessage = "";
                try { lastmessage = File.ReadAllText(thiscomputername + "-LastMessage.txt"); } catch { }

                string webdatadatetime = webData.Replace("\r", "").Replace("\n", "").Split(';')[2].ToString().TrimStart().TrimEnd();

                if (lastmessage != webdatadatetime)
                {
                    File.WriteAllText(thiscomputername + "-LastMessage.txt", webdatadatetime);
                    MessageForm f2 = new MessageForm(webData.ToString().Split(';')[0].ToString(), webData.ToString().Split(';')[1].ToString(), server, port, thiscomputername + "---" + thisusername);
                    f2.Show();
                  
                }
            }
            catch
            {
                
            }
        }

   


        private void MessageTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                string webData = WebRequestAndClose("http://" + server + ":" + port + "/api/values/" + "default",true);
                string lastmessage = "";

                try { lastmessage = File.ReadAllText("default-LastMessage.txt"); } catch { }

                string webdatadatetime = webData.Replace("\r", "").Replace("\n", "").Split(';')[2].ToString().TrimStart().TrimEnd();

                if (lastmessage != webdatadatetime)
                {
                    File.WriteAllText("default-LastMessage.txt", webdatadatetime);

                    MessageForm f2 = new MessageForm(webData.ToString().Split(';')[0].ToString(), webData.ToString().Split(';')[1].ToString(), server, port, thiscomputername + "---" + thisusername);
                    f2.Show();
                }
            }
            catch 
            {
               
            }
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            WebRequestAndClose("http://" + server + ":" + port + "/api/values/" + thiscomputername + "---" + thisusername + "-Computer", false);

            count++;
            if (Control.MousePosition.X != X && Control.MousePosition.Y != Y)
            {
                idle = 0;
                WebRequestAndClose("http://" + server + ":" + port + "/api/values/" + thiscomputername + "---" + thisusername + "-Active", false);
                X = Control.MousePosition.X;
                Y = Control.MousePosition.Y;
            }
            else
            {
                idle = idle + 10;
                X = Control.MousePosition.X;
                Y = Control.MousePosition.Y;
            }

            if (idle > 600)
            {
                WebRequestAndClose("http://" + server + ":" + port + "/api/values/" + thiscomputername + "---" + thisusername + "-InActive",false);
            }
        }

      

        void Exit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;

            Application.Exit();
        }
    }
}
