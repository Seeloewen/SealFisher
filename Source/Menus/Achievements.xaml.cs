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

namespace SealFisher
{
    /// <summary>
    /// Interaktionslogik für Achievements.xaml
    /// </summary>
    public partial class wndAchievements : Window
    {
        public wndAchievements()
        {
            InitializeComponent();
            Achievementhandler();
        }

        public void Achievementhandler()
        {
            if(Player.Ach1) cbAchievement1.IsChecked = true;
            if(Player.Ach2) cbAchievement2.IsChecked = true;
            if(Player.Ach3) cbAchievement3.IsChecked = true;
            if(Player.Ach4) cbAchievement4.IsChecked = true;
            if(Player.Ach5) cbAchievement5.IsChecked = true;
            if(Player.Ach6) cbAchievement6.IsChecked = true;
            if(Player.Ach7) cbAchievement7.IsChecked = true;
            if(Player.Ach8) cbAchievement8.IsChecked = true;
            if(Player.Ach9) cbAchievement9.IsChecked = true;
        }
    }
}
