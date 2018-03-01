using cslabs_win.client.packet;
using cslabs_win.common;
using cslabs_win.common.packet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cslabs_win.client
{
    public partial class MiniWindow : Form
    {

        public MiniWindow()
        {
            InitializeComponent();

            Connection connection = new Connection()
            {
                ServerAddress = Properties.Settings.Default.ServerAddress,
                ServerPort = Properties.Settings.Default.ServerPort,
                BindHandler = (_) => new BindPacket("Client")
            };
            connection.KnownPackets["ClientInitialize"] = new ClientInitializePacket(this);
            connection.Connect();
        }

        delegate void UpdateWindowDelegate(string username, string fullName, ExaminationMode examination);

        internal void UpdateWindow(string username, string fullName, ExaminationMode examination)
        {
            if (this.InvokeRequired)
            {
                UpdateWindowDelegate d = new UpdateWindowDelegate(UpdateWindow);
                this.Invoke(d, new object[] { username, fullName, examination });
            }
            else
            {
                if (examination.Enabled && examination.BrowserMode)
                {
                    ExaminationBrowser browser = new ExaminationBrowser(username, fullName, examination);
                    browser.Show();
                    this.Hide();
                    return;
                }
                else if (examination.Enabled)
                {
                    machineNameLabel.Text = "Examination Mode";
                    examination.Start();
                }
                else
                {
                    machineNameLabel.Text = Environment.MachineName;
                }

                usernameLabel.Text = username;
                fullNameLabel.Text = fullName;
                Opacity = 100;
                alterSizeLocation();
            }
        }

        private void MiniWindow_Load(object sender, EventArgs e)
        {
            alterSizeLocation();
        }

        private void alterSizeLocation()
        {
            int padding = 16;
            int maxWidth = Math.Max(fullNameLabel.Size.Width, machineNameLabel.Size.Width) + 180;
            maxWidth = Math.Max(maxWidth, usernameLabel.Size.Width);

            Size = new Size(maxWidth, Size.Height);
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - maxWidth - padding, padding);
        }

        private void MiniWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = e.CloseReason != CloseReason.WindowsShutDown && e.CloseReason != CloseReason.ApplicationExitCall;
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            Process.Start("shutdown", "/l /f");
            Application.Exit();
        }
    }
}
