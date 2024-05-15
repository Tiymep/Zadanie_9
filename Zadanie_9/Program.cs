using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie_9

{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Ввод данных пользователя
            IPAddress localAddress = IPAddress.Parse("127.0.0.1");
            Console.Write("Введите свое имя: ");
            string? username = Console.ReadLine();

            Console.Write("Введите порт для приема сообщений: ");
            if (!int.TryParse(Console.ReadLine(), out var localPort)) return;

            Console.Write("Введите порт для отправки сообщений: ");
            if (!int.TryParse(Console.ReadLine(), out var remotePort)) return;

            // Запуск задач для приема и отправки сообщений
            Task receiveTask = Task.Run(() => ReceiveMessageAsync(localAddress, localPort));
            Task sendTask = SendMessageAsync(localAddress, remotePort, username);

            await Task.WhenAll(receiveTask, sendTask);
        }

        private static async Task SendMessageAsync(IPAddress localAddress, int remotePort, string username)
        {
            using Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Console.WriteLine("Для отправки сообщения введите текст и нажмите Enter.");

            while (true)
            {
                var message = Console.ReadLine();

                // Если введена пустая строка, завершаем цикл и приложение
                if (string.IsNullOrWhiteSpace(message)) break;

                message = $"{username}: {message}";
                byte[] data = Encoding.UTF8.GetBytes(message);

                await sender.SendToAsync(new ArraySegment<byte>(data), SocketFlags.None, new IPEndPoint(localAddress, remotePort));
            }
        }

        private static async Task ReceiveMessageAsync(IPAddress localAddress, int localPort)
        {
            byte[] data = new byte[65535];

            using Socket receiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            receiver.Bind(new IPEndPoint(localAddress, localPort));

            while (true)
            {
                var result = await receiver.ReceiveFromAsync(new ArraySegment<byte>(data), SocketFlags.None, new IPEndPoint(IPAddress.Any, 0));
                var message = Encoding.UTF8.GetString(data, 0, result.ReceivedBytes);

                Console.WriteLine(message);
            }
        }
    }
}
