using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp
{
    public class UdpService
    {
        private UdpClient udpClient;
        private IPEndPoint endPoint;
        private const int Port = 12345; // Порт для обмена сообщениями

        // Конструктор для указания целевого IP-адреса
        public UdpService(string targetIpAddress)
        {
            udpClient = new UdpClient();
            endPoint = new IPEndPoint(IPAddress.Parse(targetIpAddress), Port); // Используем IP-адрес другого устройства
        }

        // Метод для отправки сообщений
        public async Task SendMessage(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            await udpClient.SendAsync(messageBytes, messageBytes.Length, endPoint);
            Console.WriteLine($"Message sent to {endPoint.Address}:{Port}");
        }

        // Метод для прослушивания входящих сообщений
        public async Task ListenForMessages()
        {
            UdpClient receiveClient = new UdpClient(Port); // Слушаем на порту 12345
            while (true)
            {
                try
                {
                    UdpReceiveResult result = await receiveClient.ReceiveAsync();
                    string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
                    OnMessageReceived(receivedMessage); // Обрабатываем полученное сообщение
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving message: {ex.Message}");
                }
            }
        }

        // Событие для получения сообщения
        public event Action<string> OnMessageReceived = delegate { };
    }
}
