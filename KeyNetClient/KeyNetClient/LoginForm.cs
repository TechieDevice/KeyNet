using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KeyNetClient
{
    public readonly struct address
    {
        public int number { get; }
        public string host { get; }
        public int port { get; }
        public string hostname { get; }

        public address(int Number, string Host, int Port, string Hostname)
        {
            number = Number;
            host = Host;
            port = Port;
            hostname = Hostname;
        }

        public override string ToString() => $"{number}. {host}:{port} - {hostname}";
    }

    public partial class LoginForm : Form
    {
        static TcpClient client;
        static NetworkStream stream;
        List<address> addressList = new List<address>();
        address address1 = new address(1, "127.0.0.1", 8888, "Travor");
        
        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            addressList.Add(address1);
            serverBox.Items.Add(address1);
            Crypto.RsaKeyGen();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if ((loginBox.Text != "") && (serverBox.SelectedItem != null))
            {
                loginButton.Enabled = false;
                string host = addressList[Convert.ToInt32(serverBox.SelectedItem.ToString().Substring(0, 1)) - 1].host;
                int port = addressList[Convert.ToInt32(serverBox.SelectedItem.ToString().Substring(0, 1)) - 1].port;
                client = new TcpClient();
                try
                {
                    client.Connect(host, port);     // подключение клиента
                    stream = client.GetStream();    // получаем поток

                    string data = $"{loginBox.Text.Length}{loginBox.Text}{Crypto.PublicKey}";
                    byte[] sendData = Encoding.Unicode.GetBytes(data);
                    stream.Write(sendData, 0, sendData.Length);  

                    byte[] resData = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(resData, 0, resData.Length);
                        builder.Append(Encoding.Unicode.GetString(resData, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();

                    if (message.Substring(0,2) == "OK")
                    {
                        string serverKey = message.Substring(5, Convert.ToInt32(message.Substring(2, 3)));
                        string[] users = message.Substring(5 + serverKey.Length).Split('$');
                        MainForm mainForm = new MainForm(loginBox.Text, this, client, stream, users, serverKey);
                        mainForm.Show();
                    }
                }
                catch (Exception ex)
                {
                    loginButton.Enabled = true;
                    errLabel.Text = ex.Message;
                }
            }
        }
    }
}
