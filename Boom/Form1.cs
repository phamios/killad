using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Threading;
using CefSharp.WinForms;
using CefSharp;

namespace Boom
{
    public partial class Form1 : Form
    {
        BackgroundWorker workerThread = null; 
        bool _keepRunning = false;
        public ChromiumWebBrowser browser;

        public Form1()
        {
            InitializeComponent();
            InstantiateWorkerThread();
        }

        private void InstantiateWorkerThread()
        {
            workerThread = new BackgroundWorker();
            //workerThread.ProgressChanged += WorkerThread_ProgressChanged;
            workerThread.DoWork += WorkerThread_DoWork;
            //workerThread.RunWorkerCompleted += WorkerThread_RunWorkerCompleted;
            workerThread.WorkerReportsProgress = true;
            workerThread.WorkerSupportsCancellation = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Clear();
                int counter = 0;
                string line;
                string filename = theDialog.FileName;
                System.IO.StreamReader file = new System.IO.StreamReader(filename);
                while ((line = file.ReadLine()) != null)
                {
                    listBox1.Items.Add(line);
                    counter++;
                }
                file.Close();
                 
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                listBox2.Items.Clear();
                int counter = 0;
                string line;
                string filename = theDialog.FileName;
                System.IO.StreamReader file = new System.IO.StreamReader(filename);
                while ((line = file.ReadLine()) != null)
                {
                    listBox2.Items.Add(line);
                    counter++;
                }
                file.Close();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            workerThread.RunWorkerAsync();
        }


        private void WorkerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime startTime = DateTime.Now;

            _keepRunning = true;

            while (_keepRunning)
            {
                Thread.Sleep(500); 
                Chilkat.Http http = new Chilkat.Http();
                http.SocksVersion = 5;
                int count_item = listBox2.Items.Count;
                int count = 0;
                for (int i = 0; i < listBox2.Items.Count; i++)
                {
                    
                    count++;
                    this.Invoke(new MethodInvoker(delegate ()
                        {
                            listBox2.SelectedIndex = i;
                        }
                    ));

                    string line = listBox2.Items[i].ToString(); 
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        label3.Text = "Đang từ khóa: " + line;
                    }
                    ));

                    /** 
                     * Bắt đầu chạy Sock 
                     * */
                    for (int z = 0; z < listBox1.Items.Count; z++)
                    {
                        if (browser != null && browser.IsBrowserInitialized)
                        {
                            browser.Dispose();
                        }
                        //listBox1.SelectedIndex = z;
                        string keyword = listBox2.Items[z].ToString();
                        InitBrowser(keyword, listBox1.Items[i].ToString());
                         
                    }
                     
                     
                }


                string timeElapsedInstring = (DateTime.Now - startTime).ToString(@"hh\:mm\:ss"); 
                workerThread.ReportProgress(0, timeElapsedInstring); 
                if (workerThread.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        private void InitBrowser(string keyword)
        {
            throw new NotImplementedException();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _keepRunning = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            workerThread.CancelAsync();
        }


        public void InitBrowser(string url, string sock5)
        {

           
            CefSettings cfsettings = new CefSettings();
            cfsettings.CefCommandLineArgs.Add("proxy-server", "socks5://" + sock5);
            cfsettings.UserAgent = "My/Custom/User-Agent-AndStuff";
            Cef.Initialize(cfsettings);

            browser = new ChromiumWebBrowser(url);
            this.Invoke(new MethodInvoker(delegate ()
            {
                groupBox1.Controls.Add((Control)browser);
                browser.Dock = DockStyle.Fill;
            }
            ));
            this.Invoke(new MethodInvoker(delegate ()
            {
                listBox3.Items.Add(url + "" + sock5);
            }
            ));
            Thread.Sleep(1000);
            try
            {
                browser.Dispose();
                Cef.Shutdown();
            }
            catch (Exception ex)
            {
                listBox3.Items.Add("Unable to reinitialize" + ex);

            }


        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Cef.Shutdown();
        }
    }
}
