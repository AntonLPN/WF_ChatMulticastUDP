using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientUDP_Chat
{
    public partial class Form1 : Form
    {

       int remotePort; // Порт для отправки сообщений
         IPAddress ipAddress; // IP адрес сервера
      Socket listeningSocket; // Сокет
        public Form1()
        {
            InitializeComponent();
        }


        private void AddText(string str)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(str);
           // textBoxMessages.Clear();
            textBoxMessages.Text = builder.ToString();

        }

        // Поток для приема подключений
        private  void Listen()
        {
            try
            {
               // IPEndPoint localIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0); // Прослушиваем по адресу
                //listeningSocket.Bind(localIP);

                while (true)
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    byte[] data = new byte[1024];

                    EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

                    do
                    {
                        bytes = listeningSocket.ReceiveFrom(data, ref remoteIp);
                        builder.AppendLine(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (listeningSocket.Available > 0);

                    IPEndPoint remoteFullIp = remoteIp as IPEndPoint;

                    textBoxMessages.BeginInvoke(new Action<string>(AddText), builder.ToString());


                    //Console.WriteLine("{0}:{1} - {2}", remoteFullIp.Address.ToString(), remoteFullIp.Port, builder.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                CloseConnect();
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










        private void buttonStart_Click(object sender, EventArgs e)
        {
            
            remotePort =Convert.ToInt32(textBoxPort.Text);
            ipAddress = IPAddress.Parse(textBoxIP.Text);


           

            try
            {
                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // Создание сокета
               // Task listeningTask = new Task(Listen); // Создание потока
               // listeningTask.Start(); // Запуск потока

                    string message ="К нам подключился клиент :" + textBoxNameUser.Text;

                    byte[] data = Encoding.Unicode.GetBytes(message);
                    EndPoint remotePoint = new IPEndPoint(ipAddress, remotePort);
                    listeningSocket.SendTo(data, remotePoint);

                Task.Run(() =>
                {
                    Listen();


                });

                buttonStart.Enabled = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
         
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            CloseConnect();
            textBoxMessages.Clear();
            textBoxMessages.Text = "Подклчение с сервером разорвано";
            buttonStart.Enabled = true;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {

            string mess = $"{DateTime.Now}  \r\n  From :  {textBoxNameUser.Text} \r\n {textBox1.Text}";

            byte[] data = Encoding.Unicode.GetBytes(mess);
            EndPoint remotePoint = new IPEndPoint(ipAddress, remotePort);
            listeningSocket.SendTo(data, remotePoint);
        }
    }
}
