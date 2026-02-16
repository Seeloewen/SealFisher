using System.Windows;

namespace SealFisher
{
    /// <summary>
    /// Interaktionslogik für Achievements.xaml
    /// </summary>
    public partial class wndAchievements : Window
    {

        //-- Constructor --//

        public wndAchievements()
        {
            InitializeComponent();
            Achievementhandler();
        }

        public void Achievementhandler()
        {
            //Checks Achievement boxes if reached
            if (Player.Ach1) cbAchievement1.IsChecked = true;
            if (Player.Ach2) cbAchievement2.IsChecked = true;
            if (Player.Ach3) cbAchievement3.IsChecked = true;
            if (Player.Ach4) cbAchievement4.IsChecked = true;
            if (Player.Ach5) cbAchievement5.IsChecked = true;
            if (Player.Ach6) cbAchievement6.IsChecked = true;
            if (Player.Ach7) cbAchievement7.IsChecked = true;
            if (Player.Ach8) cbAchievement8.IsChecked = true;
            if (Player.Ach9) cbAchievement9.IsChecked = true;
        }
    }
}
