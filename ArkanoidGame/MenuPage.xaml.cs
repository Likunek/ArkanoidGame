using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ArkanoidGame
{
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var message = MessageBox.Show("Вы уверены, что хотите выйти из игры?", "Выход", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (message == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HelpPage());
        }

        private void Game_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GamePage());
        }
    }
}
