using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Numerics;

namespace KeyNetClient
{

    public partial class MainForm : Form
    {
        static string userName;
        private LoginForm loginForm;
        static string host;
        static int port;
        static TcpClient client;
        static NetworkStream stream;
        static string serverKey;
        List<user> users = new List<user>();
        ListBox activeChat;
        Thread receiveThread;
        int keyType;
        const int size = 344;


        public MainForm(string uName, LoginForm lForm, TcpClient Client, NetworkStream Stream, string[] Users, string ServerKey)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            loginForm = lForm;
            loginForm.Hide();
            userName = uName;
            nameLabel.Text = userName;
            //host = Host;
            //port = Port;
            client = Client;
            stream = Stream;
            serverKey = ServerKey;
            usersBox.Items.AddRange(Users);
            for (int i = 0; i < Users.Length; i++)
            {
                users.Add(new user(Users[i]));
                groupBox.Controls.Add(users[i].messageTextBox);
            }
            this.Text = userName;
            sendButton.Enabled = false;

            receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (receiveThread != null) receiveThread.Abort();
            Disconnect();
            loginForm.Close();
        }

        // отправка сообщений
        private void sendButton_Click(object sender, EventArgs e)
        {
            if ((sendTextBox.Text != "") && (activeChat != null))
            {
                byte[] key = new byte[32];
                for (int i = 0; i < 32; i++)
                {
                    key[i] = users.Find(x => x.messageTextBox == activeChat).key[i];
                }
                byte[] iv = new byte[16];
                for (int i = 0; i < 16; i++)
                {
                    iv[i] = users.Find(x => x.messageTextBox == activeChat).key[32 + i];
                }
                activeChat.Items.Add($"{userName}: {sendTextBox.Text}");
                //byte[] data = Encoding.Unicode.GetBytes("ms" +
                //    users.Find(x => x.messageTextBox == activeChat).userName.Length + 
                //    users.Find(x => x.messageTextBox == activeChat).userName).Concat(Crypto.EncryptAes(sendTextBox.Text, key, iv)).ToArray();
                byte[] dannie = Crypto.EncryptAes(sendTextBox.Text, key, iv);
                string da = Crypto.DecryptAes(dannie, key, iv);
                //stream.Write(data, 0, data.Length);

                activeChat.Items.Add(da);
            }
        }

        // получение сообщений
        private void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] message = GetMessage();

                    switch (Encoding.Unicode.GetString(message).Substring(0, 2)) 
                    {
                        case"ms": //message
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    string target = Encoding.Unicode.GetString(message).Substring(3, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(2, 1)));
                                    byte[] key = new byte[32];
                                    for (int i = 0; i < 32; i++)
                                    {
                                        key[i] = users.Find(x => x.messageTextBox == activeChat).key[i];
                                    }
                                    byte[] iv = new byte[16];
                                    for (int i = 0; i < 16; i++)
                                    {
                                        iv[i] = users.Find(x => x.messageTextBox == activeChat).key[32 + i];
                                    }
                                    byte[] data = new byte[message.Length - (6 + target.Length * 2)];
                                    for (int i = 0; i < (message.Length - (6 + target.Length * 2)); i++)
                                    {
                                        data[i] = message[6 + target.Length * 2 + i];
                                    }
                                    string text = $"{target}: {Crypto.DecryptAes(data, key, iv)}";
                                    users.Find(x => x.userName == target).messageTextBox.Items.Add(text); //вывод сообщения
                                });
                                break;
                            }
                        case"pk": //public key
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    byte[] byteKey = Crypto.AesKeyPartGen();
                                    string target = Encoding.Unicode.GetString(message).Substring(3, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(2, 1)));
                                    users.Find(x => x.userName == target).key = byteKey;
                                    string key = Encoding.Unicode.GetString(message).Substring(3 + target.Length);
                                    string myKey = Crypto.PublicKey;
                                    byte[] byteKeyE = Crypto.ForeignRsaEncrypt(byteKey, Encoding.Unicode.GetString(message).Substring(3 + target.Length));
                                    string base64Key = Convert.ToBase64String(byteKeyE);
                                    byte[] byteUnicode = Encoding.Unicode.GetBytes(base64Key);
                                    //base64Key = Encoding.Unicode.GetString(byteUnicode);
                                    //byteKeyE = Convert.FromBase64String(base64Key);
                                    //byteKey = Crypto.RsaDecrypt(byteKeyE);
                                    byte[] data = Encoding.Unicode.GetBytes($"kp{target.Length}{target}").Concat(byteUnicode).ToArray();
                                    stream.Write(data, 0, data.Length);
                                });
                                break;
                            }
                        case "kp": //key part
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    byte[] byteKey = Crypto.AesKeyPartGen();
                                    string target = Encoding.Unicode.GetString(message).Substring(3, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(2, 1)));
                                    byte[] byteUnicode = new byte[344];
                                    for (int i = 0; i < 344; i++)
                                    {
                                        byteUnicode[i] = message[6 + target.Length * 2 + i];
                                    }
                                    string base64KeyPart = Encoding.Unicode.GetString(byteUnicode);
                                    byte[] byteKeyPartE = Convert.FromBase64String(base64KeyPart);
                                    byte[] keyPart = Crypto.RsaDecrypt(byteKeyPartE);

                                    string key = Encoding.Unicode.GetString(message).Substring(3 + target.Length + 172);
                                    string myKey = Crypto.PublicKey;

                                    byte[] byteKeyE = Crypto.ForeignRsaEncrypt(byteKey, Encoding.Unicode.GetString(message).Substring(3 + target.Length + 172));
                                    string base64Key = Convert.ToBase64String(byteKeyE);
                                    byteUnicode = Encoding.Unicode.GetBytes(base64Key);
                                    byte[] data = Encoding.Unicode.GetBytes($"pc{target.Length}{target}").Concat(byteUnicode).ToArray();
                                    stream.Write(data, 0, data.Length);
                                    users.Find(x => x.userName == target).key = Crypto.UniteAesKey(keyPart, byteKey);
                                    if (activeChat != null)
                                    {
                                        if (users.Find(x => x.userName == target).messageTextBox == activeChat)
                                        {
                                            keyBox.Text = Encoding.Unicode.GetString(users.Find(x => x.userName == target).key);
                                            //keyButton.Enabled = false;
                                            sendButton.Enabled = true;
                                        }
                                    }
                                });
                                break;
                            }
                        case "pc": //private create
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    string target = Encoding.Unicode.GetString(message).Substring(3, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(2, 1)));
                                    byte[] byteUnicode = new byte[344];
                                    for (int i = 0; i < 344; i++)
                                    {
                                        byteUnicode[i] = message[6 + target.Length * 2 + i];
                                    }
                                    string base64KeyPart = Encoding.Unicode.GetString(byteUnicode);
                                    byte[] byteKeyPartE = Convert.FromBase64String(base64KeyPart);
                                    byte[] keyPart = Crypto.RsaDecrypt(byteKeyPartE);
                                    users.Find(x => x.userName == target).key = Crypto.UniteAesKey(users.Find(x => x.userName == target).key, keyPart);
                                    keyBox.Text = Encoding.Unicode.GetString(users.Find(x => x.userName == target).key);
                                    //keyButton.Enabled = false;
                                    sendButton.Enabled = true;
                                });
                                break;
                            }
                        case "us": //user send
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    usersBox.Items.Clear();
                                    string[] Users = Encoding.Unicode.GetString(message).Substring(2).Split('$');
                                    usersBox.Items.AddRange(Users);   //обновление списка юзеров в сети
                                    users.Clear();
                                    for (int i = 0; i < Users.Length; i++)
                                    {
                                        users.Add(new user(Users[i]));
                                        groupBox.Controls.Add(users[i].messageTextBox);
                                    }
                                });
                                break;
                            }
                        case "sx": //send X and public key (p,g,y)
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    string target = Encoding.Unicode.GetString(message).Substring(3, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(2, 1)));
                                    byte[] g = new byte[size];
                                    for (int i = 0; i < size; i++)
                                    {
                                        g[i] = message[6 + target.Length * 2 + i];
                                    }
                                    string base64g = Encoding.Unicode.GetString(g);
                                    byte[] byteg = Convert.FromBase64String(base64g);
                                    users.Find(x => x.userName == target).bg = new BigInteger(byteg);

                                    byte[] p = new byte[size];
                                    for (int i = 0; i < size; i++)
                                    {
                                        p[i] = message[size + 6 + target.Length * 2 + i];
                                    }
                                    string base64p = Encoding.Unicode.GetString(p);
                                    byte[] bytep = Convert.FromBase64String(base64p);
                                    users.Find(x => x.userName == target).bp = new BigInteger(bytep);

                                    byte[] y = new byte[size];
                                    for (int i = 0; i < size; i++)
                                    {
                                        y[i] = message[2*size + 6 + target.Length * 2 + i];
                                    }
                                    string base64y = Encoding.Unicode.GetString(y);
                                    byte[] bytey = Convert.FromBase64String(base64y);
                                    users.Find(x => x.userName == target).by = new BigInteger(bytey);

                                    byte[] e = new byte[message.Length - (3*size + 6 + target.Length * 2)];
                                    for (int i = 0; i < (message.Length - (3*size + 6 + target.Length * 2)); i++)
                                    {
                                        e[i] = message[3*size + 6 + target.Length * 2 + i];
                                    }
                                    string base64e = Encoding.Unicode.GetString(e);
                                    byte[] bytee = Convert.FromBase64String(base64e);
                                    users.Find(x => x.userName == target).bx = new BigInteger(bytee);

                                    users.Find(x => x.userName == target).bt = Crypto.GetRND(BitConverter.GetBytes(Math.Pow(2, 72)-1).Length);
                                    //users.Find(x => x.userName == target).bt = 129;
                                    byte[] t = users.Find(x => x.userName == target).bt.ToByteArray();
                                    if (t[t.Length - 1] != 0)
                                    {
                                        byte[] tempt = new byte[t.Length];
                                        Array.Copy(t, tempt, t.Length);
                                        t = new byte[tempt.Length + 1];
                                        Array.Copy(tempt, t, tempt.Length);
                                    }
                                    string base64t = Convert.ToBase64String(t);
                                    byte[] tUnicode = Encoding.Unicode.GetBytes(base64t);

                                    byte[] data = Encoding.Unicode.GetBytes("st" + usersBox.SelectedItem.ToString().Length + usersBox.SelectedItem.ToString()).Concat(tUnicode).ToArray();
                                    stream.Write(data, 0, data.Length);
                                });
                                break;
                            }
                        case "ss": //send S
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    string target = Encoding.Unicode.GetString(message).Substring(3, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(2, 1)));
                                    byte[] s;
                                    s = new byte[message.Length - (6 + target.Length * 2)];
                                    for (int i = 0; i < (message.Length - (6 + target.Length * 2)); i++)
                                    {
                                        s[i] = message[6 + target.Length * 2 + i];
                                    }
                                    string base64s = Encoding.Unicode.GetString(s);
                                    byte[] bytes = Convert.FromBase64String(base64s);
                                    users.Find(x => x.userName == target).bs = new BigInteger(bytes);

                                    BigInteger check =
                                        (BigInteger.ModPow(
                                            users.Find(x => x.userName == target).bg,
                                            users.Find(x => x.userName == target).bs,
                                            users.Find(x => x.userName == target).bp)
                                        * BigInteger.ModPow(
                                            users.Find(x => x.userName == target).by,
                                            users.Find(x => x.userName == target).bt,
                                            users.Find(x => x.userName == target).bp)) 
                                        % users.Find(x => x.userName == target).bp;

                                    byte[] data;
                                    if (check == users.Find(x => x.userName == target).bx)
                                    {
                                        data = Encoding.Unicode.GetBytes("ok" + usersBox.SelectedItem.ToString().Length + usersBox.SelectedItem.ToString() + "ok").ToArray();
                                        keyBox.Text = "ok";
                                    }
                                    else
                                    {
                                        data = Encoding.Unicode.GetBytes("ok" + usersBox.SelectedItem.ToString().Length + usersBox.SelectedItem.ToString() + "fail").ToArray();
                                        keyBox.Text = "fail";                             
                                    }
                                    stream.Write(data, 0, data.Length);
                                });
                                break;
                            }
                        case "st": //send T (E)
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    string target = Encoding.Unicode.GetString(message).Substring(3, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(2, 1)));
                                    byte[] t;
                                    t = new byte[message.Length - (6 + target.Length * 2)];
                                    for (int i = 0; i < (message.Length - (6 + target.Length * 2)); i++)
                                    {
                                        t[i] = message[6 + target.Length * 2 + i];
                                    }
                                    string base64t = Encoding.Unicode.GetString(t);
                                    byte[] bytet = Convert.FromBase64String(base64t);
                                    users.Find(x => x.userName == target).t = new BigInteger(bytet);

                                    users.Find(x => x.userName == target).s = (users.Find(x => x.userName == target).r + (users.Find(x => x.userName == target).t * Crypto.DSAdata[4])) % Crypto.DSAdata[1];
                                    //users.Find(x => x.userName == target).s = (users.Find(x => x.userName == target).r + (users.Find(x => x.userName == target).t * 357)) % 443;
                                    byte[] s = users.Find(x => x.userName == target).s.ToByteArray();
                                    if (s[s.Length - 1] != 0)
                                    {
                                        byte[] temps = new byte[s.Length];
                                        Array.Copy(s, temps, s.Length);
                                        s = new byte[temps.Length + 1];
                                        Array.Copy(temps, s, temps.Length);
                                    }
                                    string base64s = Convert.ToBase64String(s);
                                    byte[] sUnicode = Encoding.Unicode.GetBytes(base64s);

                                    byte[] data = Encoding.Unicode.GetBytes("ss" + usersBox.SelectedItem.ToString().Length + usersBox.SelectedItem.ToString()).Concat(sUnicode).ToArray();
                                    stream.Write(data, 0, data.Length);
                                });
                                break;
                            }
                        case "ok": //ok or fail
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    string target = Encoding.Unicode.GetString(message).Substring(3, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(2, 1)));                                   
                                    keyBox.Text = Encoding.Unicode.GetString(message).Substring(3 + target.Length);
                                });
                                break;
                            }
                        case "ts": //time mark send
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    string target = Encoding.Unicode.GetString(message).Substring(3, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(2, 1)));
                                    string mark = Encoding.Unicode.GetString(message).Substring(5 + target.Length, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(3 + target.Length, 2)));
                                    DateTime date = new DateTime(1, 1, 1, 0, 0, 00);
                                    DateTime now = DateTime.Now;
                                    TimeSpan interval = now - date;
                                    string timeMark = Convert.ToInt32(interval.TotalMinutes).ToString();
                                    if (timeMark == mark)
                                    {
                                        byte[] key = Encoding.Unicode.GetBytes(Encoding.Unicode.GetString(message).Substring(4 + target.Length + mark.Length));
                                        byte[] byteUnicode = new byte[128];
                                        for (int i = 0; i < 128; i++)
                                        {
                                            byteUnicode[i] = message[10 + target.Length * 2 + mark.Length * 2 + i];
                                        }
                                        string base64Key = Encoding.Unicode.GetString(byteUnicode);
                                        byte[] byteKey = Convert.FromBase64String(base64Key);
                                        users.Find(x => x.userName == target).key = byteKey;
                                        if (activeChat != null)
                                        {
                                            if (users.Find(x => x.userName == target).messageTextBox == activeChat)
                                            {
                                                keyBox.Text = Encoding.Unicode.GetString(users.Find(x => x.userName == target).key);
                                                //keyButton.Enabled = false;
                                                sendButton.Enabled = true;
                                            }
                                        }
                                        byte[] data = Encoding.Unicode.GetBytes("of" + usersBox.SelectedItem.ToString().Length + usersBox.SelectedItem.ToString()).ToArray();
                                        stream.Write(data, 0, data.Length);
                                    }
                                });
                                break;
                            }
                        case "of": //ok or fail
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    string target = Encoding.Unicode.GetString(message).Substring(3, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(2, 1)));
                                    if (activeChat != null)
                                    {
                                        if (users.Find(x => x.userName == target).messageTextBox == activeChat)
                                        {
                                            keyBox.Text = Encoding.Unicode.GetString(users.Find(x => x.userName == target).key);
                                            //keyButton.Enabled = false;
                                            sendButton.Enabled = true;
                                        }
                                    }
                                });
                                break;
                            }
                        default:
                            this.BeginInvoke((MethodInvoker)delegate ()
                            {
                                users.Find(x => x.userName == Encoding.Unicode.GetString(message).Substring(3, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(2, 1))))
                                    .messageTextBox.Items.Add("ReceiveMessageError"); //сообщение получено с ошибкой
                            });
                            break;                           
                    }
                }
                catch (Exception e)
                {
                    this.BeginInvoke((MethodInvoker)delegate ()
                    {
                        if (activeChat != null)
                        {
                            activeChat.Items.Add("Соединение разорвано!"); //соединение было прервано
                        }
                    });
                    Disconnect();
                    return;
                }
            }
        }

        private byte[] GetMessage()
        {
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

            return Encoding.Unicode.GetBytes(message);
        }

        private void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
        }

        private void chatButton_Click(object sender, EventArgs e)
        {
            if (usersBox.SelectedItem != null)
            {
                if (activeChat != null) activeChat.Visible = false;
                keyBox.Text = Encoding.Unicode.GetString(users.Find(x => x.userName == usersBox.SelectedItem.ToString()).key);
                activeChat = users.Find(x => x.userName == usersBox.SelectedItem.ToString()).messageTextBox;
                activeChat.Visible = true;
                if (keyBox.Text == "")
                {
                    keyButton.Enabled = true;
                    sendButton.Enabled = false;
                }
                else
                {
                    //keyButton.Enabled = false;
                    sendButton.Enabled = true;
                }
            }
        }

        private void keyButton_Click(object sender, EventArgs e)
        {
            if (usersBox.SelectedItem != null)
            {
                if (keyType == 0)
                //if (Encoding.Unicode.GetString(users.Find(x => x.userName == usersBox.SelectedItem.ToString()).key) == "")
                {
                    byte[] nameSend = Encoding.Unicode.GetBytes("pk" + usersBox.SelectedItem.ToString().Length + usersBox.SelectedItem.ToString());
                    stream.Write(nameSend, 0, nameSend.Length);
                }
                else if (keyType == 1)
                //if (Encoding.Unicode.GetString(users.Find(x => x.userName == usersBox.SelectedItem.ToString()).key) == "")
                {
                    byte[] key = Crypto.AesKeyGen();
                    string base64Key = Convert.ToBase64String(key);
                    byte[] keyUnicode = Encoding.Unicode.GetBytes(base64Key);
                    users.Find(x => x.userName == usersBox.SelectedItem.ToString()).key = key;
                    

                    DateTime date = new DateTime(1, 1, 1, 0, 0, 00);
                    DateTime now = DateTime.Now;
                    TimeSpan interval = now - date;
                    string timeMark = Convert.ToInt32(interval.TotalMinutes).ToString();

                    byte[] dataSend = Encoding.Unicode.GetBytes(
                        "ts" + usersBox.SelectedItem.ToString().Length + 
                        usersBox.SelectedItem.ToString() + 
                        timeMark.Length + 
                        timeMark).Concat(
                        keyUnicode).ToArray();
                    stream.Write(dataSend, 0, dataSend.Length);
                }
                else if (keyType == 2)
                //if (Encoding.Unicode.GetString(users.Find(x => x.userName == usersBox.SelectedItem.ToString()).key) == "")
                {
                    Crypto.DsaGen();
                    //users.Find(i => i.userName == usersBox.SelectedItem.ToString()).g = Crypto.DSAdata[2];
                    //byte[] g = Crypto.DSAdata[2].ToByteArray();
                    BigInteger gbi = Crypto.DSAdata[2];
                    users.Find(i => i.userName == usersBox.SelectedItem.ToString()).g = gbi;
                    byte[] g = gbi.ToByteArray();
                    if (g.Length == 128)
                    {
                        byte[] temp = new byte[128];
                        Array.Copy( g, temp,128);
                        g = new byte[129];
                        Array.Copy(temp, g, 128);
                    }
                    string base64g = Convert.ToBase64String(g);
                    byte[] gUnicode = Encoding.Unicode.GetBytes(base64g);

                    //users.Find(i => i.userName == usersBox.SelectedItem.ToString()).p = Crypto.DSAdata[0];
                    BigInteger pbi = Crypto.DSAdata[0];
                    users.Find(i => i.userName == usersBox.SelectedItem.ToString()).p = pbi;
                    byte[] p = pbi.ToByteArray();
                    //byte[] p = Crypto.DSAdata[0].ToByteArray();
                    if (p.Length == 128)
                    {
                        byte[] temp = new byte[128];
                        Array.Copy(p, temp, 128);
                        p = new byte[129];
                        Array.Copy(temp, p, 128);
                    }
                    string base64p = Convert.ToBase64String(p);
                    byte[] pUnicode = Encoding.Unicode.GetBytes(base64p);

                    BigInteger w = Crypto.DSAdata[4];
                    BigInteger pravoslavnoeG = Crypto.FindInverse(Crypto.DSAdata[2], pbi);
                    BigInteger ybi = BigInteger.ModPow(pravoslavnoeG, w, pbi);
                    users.Find(i => i.userName == usersBox.SelectedItem.ToString()).y = ybi;

                    byte[] y = ybi.ToByteArray();
                    if (y.Length == 128)
                    {
                        byte[] temp = new byte[128];
                        Array.Copy(y, temp, 128);
                        y = new byte[129];
                        Array.Copy(temp, y, 128);
                    }
                    string base64y = Convert.ToBase64String(y);
                    byte[] yUnicode = Encoding.Unicode.GetBytes(base64y);

                    BigInteger rbi = Crypto.GetRND(Crypto.DSAdata[1].ToByteArray().Length-1);
                    //BigInteger rbi = 274;
                    users.Find(i => i.userName == usersBox.SelectedItem.ToString()).r = rbi;
                    byte[] r = rbi.ToByteArray();
                    BigInteger xbi = BigInteger.ModPow(Crypto.DSAdata[2], rbi, pbi);
                    //BigInteger xbi = 37123;
                    users.Find(i => i.userName == usersBox.SelectedItem.ToString()).x = xbi;
                    byte[] x = xbi.ToByteArray();
                    users.Find(i => i.userName == usersBox.SelectedItem.ToString()).bytex = x;
                    if (x[x.Length-1] != 0)
                    {
                        byte[] tempx = new byte[x.Length];
                        Array.Copy(x, tempx, x.Length);
                        x = new byte[tempx.Length + 1];
                        Array.Copy(tempx, x, tempx.Length);
                    }
                    string base64x = Convert.ToBase64String(x);
                    byte[] xUnicode = Encoding.Unicode.GetBytes(base64x);

                    byte[] send = Encoding.Unicode.GetBytes(
                        "sx" + usersBox.SelectedItem.ToString().Length + usersBox.SelectedItem.ToString()).Concat(
                        gUnicode).Concat(
                        pUnicode).Concat(
                        yUnicode).Concat(
                        xUnicode).ToArray();
                    stream.Write(send, 0, send.Length);
                }
            }
        }

        private void aesButton_CheckedChanged(object sender, EventArgs e)
        {
            keyType = 0;
        }

        private void frogButton_CheckedChanged(object sender, EventArgs e)
        {
            keyType = 1;
        }

        private void dsaButton_CheckedChanged(object sender, EventArgs e)
        {
            keyType = 2;
        }
    }

    public class user
    {
        public string userName;
        public ListBox messageTextBox;
        public byte[] key;
        public BigInteger r;
        public BigInteger p;
        public BigInteger g;
        public BigInteger x;
        public BigInteger y;
        public BigInteger t;
        public BigInteger s;
        public BigInteger bp;
        public BigInteger bg;
        public BigInteger bx;
        public BigInteger by;
        public BigInteger bt;
        public BigInteger bs;
        public byte[] bytex;
        public user(string UserName)
        {
            userName = UserName;
            key = new byte[0];
            messageTextBox = new ListBox();
            messageTextBox.Visible = false;
            messageTextBox.Name = $"{UserName}TextBox";
            messageTextBox.Location = new System.Drawing.Point(6, 19);
            messageTextBox.Size = new System.Drawing.Size(539, 277);
            messageTextBox.ScrollAlwaysVisible = true;
            messageTextBox.SelectionMode = SelectionMode.None;
        }
        public void KeySet(byte[] Key)
        {
            key = Key;
        }
    }
}


//byte[] resData = new byte[0];
//int bytes = 0;
//do
//{
//    byte[] buf = new byte[64]; // буфер для получаемых данных
//    bytes += stream.Read(buf, 0, buf.Length);
//    resData = resData.Concat(buf).ToArray();
//    if (bytes == 0)
//    {
//        throw new Exception();
//    }
//}
//while (stream.DataAvailable);

//return resData;
