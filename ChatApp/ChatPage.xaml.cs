using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace ChatApp
{
    public partial class ChatPage : ContentPage
    {
        private UdpService udpService;
        private DatabaseService databaseService;
        private string username;
        public ObservableCollection<Message> Messages { get; set; }

        // Конструктор, который принимает имя пользователя и IP-адрес другого устройства
        public ChatPage(string username, string targetIpAddress)
        {
            InitializeComponent();
            this.username = username;

            // Создаем сервис для общения с другим устройством по его IP-адресу
            udpService = new UdpService(targetIpAddress);
            udpService.OnMessageReceived += (message) => DisplayReceivedMessage(message);

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "chat.db");
            databaseService = new DatabaseService(dbPath);

            var savedMessages = databaseService.GetMessages();
            Messages = new ObservableCollection<Message>(savedMessages);

            BindingContext = this;

            // Запускаем слушание входящих сообщений
            Task.Run(() => udpService.ListenForMessages());
        }

        private async void OnSendMessageClicked(object sender, EventArgs e)
        {
            string messageText = MessageEntry.Text;
            if (!string.IsNullOrEmpty(messageText))
            {
                var message = new Message
                {
                    Username = username,
                    Text = messageText,
                    Timestamp = DateTime.Now
                };

                // Сохраняем в базе данных
                databaseService.SaveMessage(message);

                // Добавляем сообщение в коллекцию
                Messages.Add(message);

                // Отправляем через UDP
                await udpService.SendMessage(messageText);

                // Очистить поле ввода
                MessageEntry.Text = string.Empty;
            }
        }

        private void DisplayReceivedMessage(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Messages.Add(new Message
                {
                    Username = "Другой пользователь",
                    Text = message,
                    Timestamp = DateTime.Now
                });
            });
        }
    }
}
