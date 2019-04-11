using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZBase;
using ZBase.Common;
using ZBase.Network;

namespace Gui {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
            Logger.LogItemAdded += LoggerOnLogItemAdded;
        }

        private void LoggerOnLogItemAdded(LogItem logItem) {
            if (InvokeRequired) {
                Invoke(new LogEventArgs(LoggerOnLogItemAdded), logItem);
                return;
            }

            if (logItem.Type == LogType.Info || logItem.Type == LogType.Chat || logItem.Type == LogType.Command)
                AddMainLog(logItem);
            else
                AddErrorLog(logItem);
        }

        private void AddMainLog(LogItem item) {
            var formatted = $"{item.Time.ToShortTimeString()} > {item.Message}\r\n";
            infoLog.AppendText(formatted);
            infoLog.Select(infoLog.Text.Length, infoLog.Text.Length);
            infoLog.ScrollToCaret();
        }

        private void AddErrorLog(LogItem item) {
            var formatted = $"{item.Time.ToShortTimeString()} > [{item.Type.ToString()}] {item.Message}\r\n";
            errorLog.AppendText(formatted);
            errorLog.Select(errorLog.Text.Length, errorLog.Text.Length);
            errorLog.ScrollToCaret();
        }

        private void startButton_Click(object sender, EventArgs e) {
            infoLog.Clear();
            errorLog.Clear();
            Main.Start();

            Task.Run(() => UpdateBandwidth());
        }

        private void UpdateBandwidth() {
            while (Main.Running) {
                long one = Interlocked.Read(ref Server.BytesReceived);
                long two = Interlocked.Read(ref Server.BytesSent);
                Update(one, two);
                Task.Delay(500).Wait();
            }
        }

        private delegate void dualintargs(long one, long two);

        private void Update(long uno, long dos) {
            if (InvokeRequired) {
                Invoke(new dualintargs(Update), uno, dos);
                return;
            }

            lblRecv.Text = "Recv: " + uno + " KB/s";
            lblSent.Text = "Sent: " + dos + " KB/s";
        }

        private void tabPage1_Click(object sender, EventArgs e) {

        }

        private void btnStop_Click(object sender, EventArgs e) {
            Main.Stop();
        }
    }
}
