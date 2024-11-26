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

        // �����������, ������� ��������� ��� ������������ � IP-����� ������� ����������
        public ChatPage(string username, string targetIpAddress)
        {
            InitializeComponent();
            this.username = username;

            // ������� ������ ��� ������� � ������ ����������� �� ��� IP-������
            udpService = new UdpService(targetIpAddress);
            udpService.OnMessageReceived += (message) => DisplayReceivedMessage(message);

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "chat.db");
            databaseService = new DatabaseService(dbPath);

            var savedMessages = databaseService.GetMessages();
            Messages = new ObservableCollection<Message>(savedMessages);

            BindingContext = this;

            // ��������� �������� �������� ���������
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

                // ��������� � ���� ������
                databaseService.SaveMessage(message);

                // ��������� ��������� � ���������
                Messages.Add(message);

                // ���������� ����� UDP
                await udpService.SendMessage(messageText);

                // �������� ���� �����
                MessageEntry.Text = string.Empty;
            }
        }

        private void DisplayReceivedMessage(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Messages.Add(new Message
                {
                    Username = "������ ������������",
                    Text = message,
                    Timestamp = DateTime.Now
                });
            });
        }
    }
}
