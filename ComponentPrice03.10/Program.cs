using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ComponentPrice03._10
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string[] details = { "Процесор", "Оперативна пам'ять", "Вiдеокарта",
                "Накопичувач SSD", "Блок живлення", "Система охолодження", "Корпус" };

            Console.WriteLine("Деталi: ");
            foreach (var detail in details)
            {
                Console.WriteLine(detail);
            }

            using (Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("1 - Write price");
                    Console.WriteLine("2 - Exit");
                    Console.Write("Choose: ");

                    string command = Console.ReadLine();

                    if (command == "1")
                    {
                        Console.WriteLine("Enter component: ");
                        string userComponent = Console.ReadLine();

                        byte[] data = Encoding.UTF8.GetBytes(userComponent);
                        await udpSocket.SendToAsync(data, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555));

                        byte[] response = new byte[256];

                        EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);


                        var result = await udpSocket.ReceiveFromAsync(response, remoteIp);
                        string price = Encoding.UTF8.GetString(response, 0, result.ReceivedBytes);

                        Console.WriteLine();
                        Console.WriteLine($"{price}");
                    }

                    if (command == "2")
                    {
                        byte[] data = Encoding.UTF8.GetBytes("EXIT");
                        await udpSocket.SendToAsync(data, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555));
                        break;
                    }
                }
            }
        }
    }
}
