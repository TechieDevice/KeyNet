using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using KeyNetClient;
using System.Numerics;

namespace KeyNetServer
{
    class Program
    {
        static ServerObject server; // сервер
        static Thread listenThread; // поток для прослушивания
         
        static void Main(string[] args)
        {
            try
            {
                Crypto.RsaKeyGen();
                //DB.Connect();
                Crypto.DsaGen();

                BigInteger p = Crypto.DSAdata[0];
                BigInteger q = Crypto.DSAdata[1];
                BigInteger g = Crypto.DSAdata[2];
                BigInteger j = Crypto.FindInverse(g,p);
                BigInteger w = Crypto.DSAdata[4];
                BigInteger y1 = BigInteger.ModPow(g,w,p);
                BigInteger y2 = BigInteger.ModPow(j,w,p);
                BigInteger r = 274;
                    BigInteger x = BigInteger.ModPow(g,r,p);
                BigInteger e = 129;
                BigInteger s = (r+w*e)%q;
                    BigInteger z1 = (BigInteger.ModPow(g,s,p)*BigInteger.ModPow(y1,e,p))%p;
                    BigInteger z2 = (BigInteger.ModPow(g,s,p)*BigInteger.ModPow(y2,e,p))%p;


                //BigInteger w = BigInteger.ModPow(Crypto.DSAdata[2], Crypto.DSAdata[0] - Crypto.DSAdata[4], Crypto.DSAdata[0]);
                //Console.WriteLine(w);
                Console.WriteLine(x);
                Console.WriteLine();
                Console.WriteLine(y1);
                Console.WriteLine(y2);
                Console.WriteLine();
                Console.WriteLine(z1);
                Console.WriteLine(z2);

                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); //старт потока
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        public List<ClientObject> clients = new List<ClientObject>(); // все подключения

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string userName)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.userName == userName);
            // и удаляем его из списка подключений
            if (client != null)
                clients.Remove(client);
        }
        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // трансляция сообщения подключенным клиентам
        protected internal void SendMessage(byte[] data, string userName, int type)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if ((clients[i].userName == userName) && (type == 0)) //отправить сообщение целевому пользователю
                {
                    byte[] edata = Crypto.ForeignRsaEncrypt(data, clients[i].key);
                    clients[i].stream.Write(edata, 0, edata.Length); //передача данных
                }
                if ((clients[i].userName != userName) && (type == 1)) //отправить сообщение всем кроме целевого пользователя
                {
                    clients[i].stream.Write(data, 0, data.Length); //передача данных
                }
                if ((clients[i].userName == userName) && (type == 2)) //отправить сообщение всем кроме целевого пользователя
                {
                    clients[i].stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }
        // отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            DB.Close();
            Environment.Exit(0); //завершение процесса
        }
    }

    public class ClientObject
    {
        protected internal string key { get; private set; }
        protected internal NetworkStream stream { get; private set; }
        protected internal string userName { get; private set; }
        TcpClient client;
        ServerObject server; //объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                stream = client.GetStream();
                //получаем имя пользователя
                byte[] message = GetMessage();

                userName = Encoding.Unicode.GetString(message).Substring(1, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(0,1)));
                key = Encoding.Unicode.GetString(message).Substring(userName.Length + 1);

                string sendMessage = $"OK{Crypto.PublicKey.Length}{Crypto.PublicKey}";
                if(server.clients.Count > 0)
                {
                    sendMessage += server.clients[0].userName.ToString();
                    for (int i = 1; i < server.clients.Count; i++)
                    {
                        sendMessage += "$" + server.clients[i].userName.ToString();
                    }
                    server.SendMessage(Encoding.Unicode.GetBytes(sendMessage), this.userName, 2);
                    sendMessage = "";
                    sendMessage += server.clients[0].userName.ToString();
                    for (int i = 1; i < server.clients.Count; i++)
                    {
                        sendMessage += "$" + server.clients[i].userName.ToString();
                    }
                    server.SendMessage(Encoding.Unicode.GetBytes($"us{sendMessage}"), this.userName, 1);                  
                }

                Console.WriteLine($"{userName} вошел в сеть");

                //в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        StringBuilder builder = new StringBuilder();
                        builder.Append(Encoding.Unicode.GetString(message));
                        string encMessage = builder.ToString();

                        switch (encMessage.Substring(0, 2))
                        {
                            case "ms": //message
                                {
                                    string targetUser = encMessage.Substring(3, Convert.ToInt32(encMessage.Substring(2, 1)));

                                    Console.WriteLine($"{this.userName} отправил {targetUser}: {Encoding.Unicode.GetString(message).Substring(targetUser.Length + 3)}");
                                    server.SendMessage(Encoding.Unicode.GetBytes($"ms{this.userName.Length}{this.userName}{Encoding.Unicode.GetString(message).Substring(targetUser.Length + 3)}"), targetUser, 2);
                                    break;
                                }
                            case "pk": //public key
                                {
                                    string targetUser = encMessage.Substring(3, Convert.ToInt32(encMessage.Substring(2, 1)));
                                    string key = server.clients.Find(x => x.userName == targetUser).key;
                                    Console.WriteLine($"Сервер отправил открытый ключ {targetUser} для {this.userName}");
                                    server.SendMessage(Encoding.Unicode.GetBytes($"pk{targetUser.Length}{targetUser}{server.clients.Find(x => x.userName == targetUser).key}"), this.userName, 2);
                                    break;
                                }
                            case "pc": //private create
                                {
                                    string targetUser = encMessage.Substring(3, Convert.ToInt32(encMessage.Substring(2, 1)));
                                    byte[] keyPart = new byte[344];
                                    for (int i = 0; i < 344; i++)
                                    {
                                        keyPart[i] = message[6 + targetUser.Length * 2 + i];
                                    }
                                    byte[] mes = Encoding.Unicode.GetBytes($"pc{this.userName.Length}{this.userName}").Concat(keyPart).ToArray();
                                    Console.WriteLine($"Server send public {targetUser}'s key for {this.userName}");
                                    server.SendMessage(mes, targetUser, 2);
                                    break;
                                }
                            case "kp": //key part
                                {
                                    string targetUser = Encoding.Unicode.GetString(message).Substring(3, Convert.ToInt32(Encoding.Unicode.GetString(message).Substring(2, 1)));
                                    byte[] keyPart = new byte[344];
                                    for (int i = 0; i < 344; i++)
                                    {
                                        keyPart[i] = message[6 + targetUser.Length * 2 + i];
                                    }
                                    string key = this.key;
                                    byte[] mes = (Encoding.Unicode.GetBytes($"kp{this.userName.Length}{this.userName}").Concat(keyPart).ToArray()).Concat(Encoding.Unicode.GetBytes(this.key)).ToArray();
                                    server.SendMessage(mes, targetUser, 2);
                                    break;
                                }
                            //case "as":
                            //    {
                            //        string targetUser = encMessage.Substring(3, Convert.ToInt32(encMessage.Substring(2, 1)));
                            //        string key = server.clients.Find(x => x.userName == targetUser).key;
                            //        server.SendMessage(Encoding.Unicode.GetBytes($"pk{targetUser.Length}{targetUser}{server.clients.Find(x => x.userName == targetUser).key}"), this.userName, 2);
                            //        break;
                            //    }
                            case "sx": //send X and public key (p,g,y)
                                {
                                    string targetUser = encMessage.Substring(3, Convert.ToInt32(encMessage.Substring(2, 1)));
                                    byte[] x = Encoding.Unicode.GetBytes(encMessage.Substring(3 + targetUser.Length));
                                    Console.WriteLine($"Сервер отправил открытый ключ [P,G,Y] и сгенерированное число Х {this.userName} для {targetUser}");
                                    server.SendMessage(Encoding.Unicode.GetBytes($"sx{this.userName.Length}{this.userName}").Concat(x).ToArray(), targetUser, 2);
                                    break;
                                }
                            case "ss": //send S
                                {
                                    string targetUser = encMessage.Substring(3, Convert.ToInt32(encMessage.Substring(2, 1)));
                                    byte[] s = Encoding.Unicode.GetBytes(encMessage.Substring(3 + targetUser.Length));
                                    Console.WriteLine($"Сервер отправил подсчитанный {this.userName} S для {targetUser}");
                                    server.SendMessage(Encoding.Unicode.GetBytes($"ss{this.userName.Length}{this.userName}").Concat(s).ToArray(), targetUser, 2);
                                    break;
                                }
                            case "st": //send T (E)
                                {
                                    string targetUser = encMessage.Substring(3, Convert.ToInt32(encMessage.Substring(2, 1)));
                                    byte[] t = Encoding.Unicode.GetBytes(encMessage.Substring(3 + targetUser.Length));
                                    Console.WriteLine($"Сервер отправил сгенерированный {this.userName} E для {targetUser}");
                                    server.SendMessage(Encoding.Unicode.GetBytes($"st{this.userName.Length}{this.userName}").Concat(t).ToArray(), targetUser, 2);
                                    break;
                                }
                            case "ok": //ok or fail
                                {
                                    string targetUser = encMessage.Substring(3, Convert.ToInt32(encMessage.Substring(2, 1)));
                                    byte[] ok = Encoding.Unicode.GetBytes(encMessage.Substring(3 + targetUser.Length));
                                    Console.WriteLine($"Сервер отправил ответ {this.userName} для {targetUser}. Ответ: [{Encoding.Unicode.GetString(ok)}]");
                                    server.SendMessage(Encoding.Unicode.GetBytes($"ok{this.userName.Length}{this.userName}").Concat(ok).ToArray(), targetUser, 2);
                                    break;
                                }
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"{this.userName} покинул чат");

                        //server.SendMessage(Encoding.Unicode.GetBytes($"ms{this.userName.Length}{this.userName} покинул чат"), this.userName, 1);
                        sendMessage = "";
                        for (int i = 0; i < server.clients.Count; i++)
                        {
                            if (server.clients[i].userName.ToString() != userName)
                            {
                                if (sendMessage == "")
                                {
                                    sendMessage = server.clients[i].userName.ToString();
                                }
                                else
                                {
                                    sendMessage += "$" + server.clients[i].userName.ToString();
                                }
                            }
                        }
                        server.SendMessage(Encoding.Unicode.GetBytes($"us{sendMessage}"), this.userName, 1);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.userName);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
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

        // закрытие подключения
        protected internal void Close()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
