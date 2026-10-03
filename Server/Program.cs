using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var components = new Dictionary<string, int> () 
            {
                { "Процесор", 6500 },
                { "Оперативна пам'ять", 2500 },
                { "Вiдеокарта", 6000 },
                { "Накопичувач SSD", 1800 },
                { "Блок живлення", 2500 },
                { "Система охолодження", 1200 },
                { "Корпус", 2000 },
            };


            using (Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                var localIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
                udpSocket.Bind(localIP);

                Console.WriteLine("Components server started.");

                byte[] data = new byte[256];

                int bytesRead = 10;

                while (true)
                {
                    EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

                    var result = await udpSocket.ReceiveFromAsync(data, remoteIp);
                    var message = Encoding.UTF8.GetString(data, 0, result.ReceivedBytes);
                    DateTime time = DateTime.Now;

                    Console.WriteLine($"Клiєнт {result.RemoteEndPoint} запитав цiну компонента {message}");
                    Console.WriteLine($"Received {result.ReceivedBytes} byte");
                    Console.WriteLine($"Remote address: {result.RemoteEndPoint}");
                    Console.WriteLine($"Time of request: {time.ToString("HH:mm:ss")}");
                    

                    if (message == "EXIT")
                    {
                        DateTime timeExit = DateTime.Now;
                        Console.WriteLine($"Client {result.RemoteEndPoint} disconnected.");
                        Console.WriteLine($"Time of disconnect: {timeExit.ToString("HH:mm:ss")}");
                        continue;
                    }

                    string response;

                    if (components.TryGetValue(message, out int price))
                    {
                        response = $"Цiна: {price} грн";
                    }
                    else
                    {
                        response = "Таку комплектуючу не знайдено.";
                    }
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    
                    await udpSocket.SendToAsync(responseData, result.RemoteEndPoint);
                    
                }
            }
        }
    }
}
