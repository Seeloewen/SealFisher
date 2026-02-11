using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SealFisher.Source.Menus
{
    /// <summary>
    /// Interaktionslogik für wndArchive.xaml
    /// </summary>
    public partial class wndArchive : Window
    {
        wndGame wndGame = (wndGame)Application.Current.MainWindow;
        public wndArchive()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
