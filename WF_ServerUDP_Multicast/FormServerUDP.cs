using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



//Создайте оконное приложение «Чат». Для входа в чат пользователи 
//указывают логин. Каждый пользователь видит все сообщения чата. Сообщения в 
//чате могут быть только текстовыми.
//Реализуйте отправку и прием сообщений с использованием схемы 
//маршрутизации multicast. Для реализации задания используйте возможности 
//класса UdpClient
namespace WF_ServerUDP_Multicast
{
    public partial class FormServerUDP : Form
    {

       int localPort; // порт приема сообщений
         Socket listeningSocket; // Сокет
        string ip;

        static List<IPEndPoint> clients = new List<IPEndPoint>(); // Список "подключенных" клиентов
        public FormServerUDP()
        {
            InitializeComponent();
            localPort = 0;
        }

        //Запрет ввода букв
        private void textBoxPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar) ||
            (!string.IsNullOrEmpty(textBoxPort.Text) && e.KeyChar == ',' || e.KeyChar == (char)Keys.Back))
            {
                return;
            }

            e.Handled = true;
        }

        private void AddText(string str)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(str);
         
            textBoxMessages.Text += builder.ToString();

        }

        




        //}
        /// <summary>
        /// Метод отправки сообщения клиентам
        /// </summary>
        /// <param name="message"></param>
        /// <param name="port"></param>
        private void SendMessagesAllUsers(string message, string port)
        {

            byte[] data = Encoding.Unicode.GetBytes(message); // Формируем байты из текста

            for (int i = 0; i < clients.Count; i++) // Циклом перебераем всех клиентов
            {
                ///  int remotePort;
                IPAddress remoteAddress2 = IPAddress.Parse(textBoxIP.Text);
                ip = textBoxIP.Text;
                UdpClient client = null;
                try
                {
                    client = new UdpClient();
                    //client.Connect(IPEndPoint(ad))
                    byte[] buff = Encoding.Unicode.GetBytes(textBoxMessages.Text);
                   // IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), clients[i].Port);
                    Thread.Sleep(1000);
                    client.Send(buff, buff.Length, clients[i]);
                }
                catch (SocketException se)
                {
                   // MessageBox.Show(se.StackTrace);
                }
                finally
                {
                    client.Close();
                }


            }
               

           
        }



        // закрытие сокета
        private  void CloseConnect()
        {
            if (listeningSocket != null)
            {
                listeningSocket.Shutdown(SocketShutdown.Both);
                listeningSocket.Close();
                listeningSocket = null;
            }
            MessageBox.Show("Сервер остановлен!");
        }



        Task receiver;


        private void ListenerV2()
        {

            ip = textBoxIP.Text;
         
              
                if (int.TryParse(textBoxPort.Text, out localPort))
                {

                    UdpClient listener = new UdpClient(new IPEndPoint(IPAddress.Parse(ip), localPort));
                    IPEndPoint remoteEP = null;
                    while (true)
                    {
                        byte[] buff = listener.Receive(ref remoteEP);
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine(Encoding.Unicode.GetString(buff));
                  
                        textBoxMessages.BeginInvoke(new Action<string>(AddText), builder.ToString());

                    bool addClient = true; // Переменная для определения нового пользователя

                    for (int i = 0; i < clients.Count; i++) // Циклом перебераем всех пользователей которые отправляли сообщения на сервер
                        if (clients[i].Port.ToString() == remoteEP.Port.ToString()) // Если аддресс отправителя данного сообщения совпадает с аддрессом в списке
                            addClient = false; // Не добавляем клиента в историю

                    if (addClient == true) // Если этого отправителя не было обноруженно в истории
                        clients.Add(remoteEP); // Добавляем клиента в исторю


                    SendMessagesAllUsers("text", remoteEP.Port.ToString());

                }



              

                
            }
          
        }



        private void buttonStart_Click(object sender, EventArgs e)
        {


            if (receiver != null)
                return;
            receiver = Task.Run(() =>
            {

                ListenerV2();
            
            });

                


                //Console.WriteLine("UDP CHAT SERVER VERSION 3");
                //Console.Write("Введите порт для приема сообщений: ");
                //localPort = Int32.Parse(Console.ReadLine());
                //ip = textBoxIP.Text;
                //if (receiver != null)
                //    return;
                //receiver = Task.Run(() =>
                //{
                //    int localPort;
                //    if (int.TryParse(textBoxPort.Text, out localPort))
                //    {

                //        UdpClient listener = new UdpClient(new IPEndPoint(IPAddress.Parse(ip), localPort));
                //        IPEndPoint remoteEP = null;
                //        while (true)
                //        {
                //            byte[] buff = listener.Receive(ref remoteEP);
                //            StringBuilder builder = new StringBuilder();
                //            builder.AppendLine($"К нам подключился клиент: {Encoding.Unicode.GetString(buff)}");
                //            builder.AppendLine(Encoding.Default.GetString(buff));
                //            textBoxMessages.BeginInvoke(new Action<string>(AddText), builder.ToString());
                //        }
                //    }
                //});





                //localPort = Convert.ToInt32(textBoxPort.Text);
                //ip = textBoxIP.Text;


                //try
                //{
                //    listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // Создание сокета
                //    Task listeningTask = new Task(Listen); // Создание потока для получения сообщений
                //    listeningTask.Start(); // Запуск потока
                //    listeningTask.Wait(); // Не идем дальше пока поток не будет остановлен
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.StackTrace);
                //}
                //finally
                //{
                //    CloseConnect(); // Закрываем сокет
                //}
            }
    }
}
