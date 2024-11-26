using Microsoft.Maui.Controls;

namespace ChatApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnStartChatClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string targetIpAddress = TargetIpAddressEntry.Text;  // IP адрес другого устройства

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(targetIpAddress))
            {
                var chatPage = new ChatPage(username, targetIpAddress);
                await Navigation.PushAsync(chatPage);
            }
            else
            {
                await DisplayAlert("Ошибка", "Введите имя пользователя и IP-адрес", "OK");
            }
        }
    }
}
