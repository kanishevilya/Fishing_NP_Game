using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;

namespace Server
{
    internal class Program
    {
        private static Socket srvSocket = null;  //сокет сервака


        private static List<ClientInfo> players = new List<ClientInfo>();  //массив клиентов (сокет и инфа краткая)

        //private static ManualResetEvent resetEvent = new ManualResetEvent(false); 

        private static int port = 12345;
        //private static byte[] buffer = new byte[1024];
        static void Main(string[] args)
        {
            Console.WriteLine("Server");
            ThreadPool.SetMinThreads(1, 1); //пулл потоков
            ThreadPool.SetMaxThreads(int.MaxValue, int.MaxValue);

            serverProcedure(); //процедура сервака
        }
        private static void serverProcedure()
        {
            try
            {
                IPEndPoint srvEP = new IPEndPoint(IPAddress.Any, port); //настройка сервера
                srvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //
                srvSocket.Bind(srvEP); //Подключение IP Port к сокету
                srvSocket.Listen(100); //прослушивание 
                                       //ThreadPool.QueueUserWorkItem(checkWinProc);

                while (true)
                {
                    Socket client = srvSocket.Accept(); //подключение клиента
                    ClientInfo info = new ClientInfo();
                    info.client = client;
                    lock (players)
                    { //добавление клиента в массив

                        players.Add(info);
                    }

                    //info.client.Send(Encoding.UTF8.GetBytes(forSend));
                    getAllPlayersInfo(info); //получить инфу от всех игроков
                    sendForAllPlayersInfo(info); //разослать всем сообщение о себе


                    Console.WriteLine($"Client {client.RemoteEndPoint} is connected"); //лог для сервака
                    ThreadPool.QueueUserWorkItem(ClientProc, info); //начало процедуры клиента
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
        private static void getAllPlayersInfo(ClientInfo clientInfo)
        {
            //Отправка подключенному игроку информации обо всех игроках.
            //На стороне клиента флаг NEW говорит о том что подключился новый клиент
            foreach (ClientInfo pl in players)
            {
                if (pl.client != clientInfo.client)
                {
                    try
                    {

                        string forSend = "NEW" + pl.client.RemoteEndPoint.ToString();
                        clientInfo.client.Send(Encoding.UTF8.GetBytes(forSend));
                        Thread.Sleep(150);
                    }
                    catch (Exception ex) { }
                }
            }
        }
        private static void sendForAllPlayersInfo(ClientInfo clientInfo)
        {
            //Всем клиентам передаем информации о себе
            string forSend = "NEW" + clientInfo.client.RemoteEndPoint.ToString();
            foreach (ClientInfo pl in players)
            {
                if (clientInfo.client != pl.client)
                {
                    try
                    {

                        pl.client.Send(Encoding.UTF8.GetBytes(forSend));
                    }
                    catch (Exception ex) { }
                }
            }
        }
        private static void ClientProc(object par)
        {
            ClientInfo info = par as ClientInfo;
            try
            {

                Thread thread = new Thread(RecieveCycle);
                thread.Start(info);
                while (true)
                {
                    if (info.UserStep.WaitOne(100) == true)
                    {
                        Console.Write($"Client {info.client.RemoteEndPoint} coords: {info.coords} coordsOffset: {info.coordsOffset}  background: {info.backgroundX}    |   imgPath: {info.imgPath}  isMirror: {info.isMirror}   \n");
                        string forSend = "INFO" + info.client.RemoteEndPoint + " " + info.coords + " " + info.backgroundX + " " + info.coordsOffset + " " + info.imgPath + " " + info.isMirror + " " + info.Name + "$";

                        //if (info.client == firstPlayer.client) {
                        //	//secondPlayer.client.Send(Encoding.UTF8.GetBytes(forSend));
                        //	//Thread.Sleep(500);<3
                        //	firstPlayer.client.Send(Encoding.UTF8.GetBytes(forSend));
                        //} else {
                        //	firstPlayer.client.Send(Encoding.UTF8.GetBytes(forSend));

                        //}

                        foreach (ClientInfo client in players)
                        {
                            if (client.client != info.client)
                            {
                                if (client.client != null)
                                {
                                    client.client.Send(Encoding.UTF8.GetBytes(forSend)); //отправляем инфу клиентам кроме себя
                                }

                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                lock (info)
                {
                    if (info.client != null)
                    {

                        sendToAllPlayers(info, "END" + info.client.RemoteEndPoint + "$");
                    }
                    DisconnectClient(info);
                }
            }
        }
        private static void sendToAllPlayers(ClientInfo info, string forSend)
        {
            foreach (ClientInfo client in players)
            {
                if (client.client != info.client)
                {
                    if (client.client != null)
                    {
                        try
                        {

                            client.client.Send(Encoding.UTF8.GetBytes(forSend)); //отправляем инфу клиентам кроме себя
                        }
                        catch (Exception ex) { }
                    }

                }
            }
        }
        //private static Mutex mutex = new Mutex();
        private static void RecieveCycle(object par)
        {
            ClientInfo info = par as ClientInfo;
            try
            {
                while (true)
                {

                    int size = info.client.Receive(info.buffer); //получаю только я (если это Координаты и инфа об анимации)

                    string str = Encoding.UTF8.GetString(info.buffer, 0, size);
                    //info.client.Send(Encoding.UTF8.GetBytes("GOOD"));
                    bool isCanSendFullInfo = false;
                    string[] strs = str.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries).First().Split('|');

                    if (strs.Length < 1) { continue; }
                    if (strs[0].IndexOf("COORD") != -1)
                    {

                        strs[0] = strs[0].Substring(5);
                        string[] coord = strs[0].Split(' ');
                        info.coords.X = int.Parse(coord[0]);
                        info.coords.Y = int.Parse(coord[1]);
                        info.backgroundX = int.Parse(coord[2]);
                        info.coordsOffset.X = int.Parse(coord[3]);
                        info.coordsOffset.Y = int.Parse(coord[4]);
                        if (info.coordsOffset.X != 0)
                        {

                        }
                    }
                    if (strs.Length < 2) { continue; }
                    if (strs[1].IndexOf("ANIM") != -1)
                    {

                        strs[1] = strs[1].Substring(4);
                        string[] anims = strs[1].Split(' ');
                        info.imgPath = anims[0];
                        info.isMirror = anims[1] == "True" ? true : false;
                        info.Name = anims[2];
                        isCanSendFullInfo = true;
                    }


                    if (isCanSendFullInfo)
                    {
                        info.UserStep.Set(); //я могу отправить другим игрокам (INFO)
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                lock (info)
                {
                    if (info.client != null)
                    {

                        sendToAllPlayers(info, "END" + info.client.RemoteEndPoint);
                    }
                    DisconnectClient(info);
                }
            }

        }
        private static void DisconnectClient(ClientInfo info)
        {
            if (info.client != null)
            {
                Console.WriteLine($"Client {info.client.RemoteEndPoint} is disconnected");
                lock (players)
                {

                    players.Remove(info);
                }
                info.client.Close();
                info.client.Dispose();
                info.client = null;
            }
        }
        public class ClientInfo
        {
            public Socket client;
            public AutoResetEvent UserStep = new AutoResetEvent(false);

            public string Name = "Name";

            public Point coords = new Point();
            public Point coordsOffset = new Point(0, 0);
            public bool isGoLeft = false, isGoRight = false;
            public int backgroundX = 0;
            public string imgPath = "";
            public bool isMirror = false;
            public bool theMoveWas = false;
            public byte[] buffer = new byte[1024];
        }
    }
}
