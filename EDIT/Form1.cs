using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDIT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Exit()
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                notifyIcon1.ShowBalloonTip(0);
                Hide();
            }
        }

        private void Maximize()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Maximize();
        }

        private void maximizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Maximize();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(Path);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.IO.Directory.CreateDirectory(Path);

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Path;
            watcher.Filter = "*.bmp";

            watcher.Created += new FileSystemEventHandler(OnCreated);

            watcher.EnableRaisingEvents = true;
        }

        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            FileStream stream = null;
            bool FileReady = false;
            while (!FileReady)
            {
                try
                {
                    using (stream = File.Open(e.FullPath,FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        FileReady = true;

                        var bmp = Image.FromStream(stream);
                        var encoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.MimeType == "image/jpeg");
                        var parameters = new EncoderParameters();
                        parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);

                        var name = System.IO.Path.ChangeExtension(e.Name, ".jpg");
                        System.IO.Directory.CreateDirectory(Output);

                        bmp.Save(
                            Output + @"\" + name,
                            encoder,
                            parameters);

                        bmp.Dispose();
                    }
                }
                catch (IOException)
                {
                    //File isn't ready yet, so we need to keep on waiting until it is.
                }
                //We'll want to wait a bit between polls, if the file isn't ready.
                if (!FileReady) Thread.Sleep(1000);
            }
        }

        private static string Path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\Frontier Developments\Elite Dangerous\";
        private static string Output = Path + @"\Converted";
    }
}
