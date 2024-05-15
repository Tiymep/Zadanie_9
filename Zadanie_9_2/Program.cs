using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpChat
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
            Console.WriteLine();

            // Запуск задач для приема и отправки сообщений
            Task receiveTask = Task.Run(() => ReceiveMessageAsync(localPort));
            Task sendTask = SendMessageAsync(localAddress, remotePort, username);

            await Task.WhenAll(receiveTask, sendTask);
        }

        private static async Task SendMessageAsync(IPAddress localAddress, int remotePort, string username)
        {
            using UdpClient sender = new UdpClient();
            Console.WriteLine("Для отправки сообщения введите текст и нажмите Enter.");

            while (true)
            {
                var message = Console.ReadLine();

                // Если введена пустая строка, завершаем цикл и приложение
                if (string.IsNullOrWhiteSpace(message)) break;

                message = $"{username}: {message}";
                byte[] data = Encoding.UTF8.GetBytes(message);

                await sender.SendAsync(data, data.Length, new IPEndPoint(localAddress, remotePort));
            }
        }

        private static async Task ReceiveMessageAsync(int localPort)
        {
            using UdpClient receiver = new UdpClient(localPort);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                var result = await receiver.ReceiveAsync();
                var message = Encoding.UTF8.GetString(result.Buffer);

                Console.WriteLine(message);
            }
        }
    }
}
