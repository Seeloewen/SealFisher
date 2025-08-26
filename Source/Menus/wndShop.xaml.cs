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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace SealFisher
{

    public partial class wndShop : Window
    {

        wndGame wndGame = (wndGame)Application.Current.MainWindow;

        int rodPrice;
        int baitPrice;
        int storagePrice;
        int LocationPrice;

        //-- Constructor --//

        public wndShop()
        {
            InitializeComponent();
        }

        //-- Event Handlers --//

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Set money display
            tblMoney.Text = $"Money: {Player.money}";

            //Display levels
            tblRodLevel.Text = $"Rod - Level {Player.rodPower + 1}";
            tblBaitLevel.Text = $"Bait - Level {Player.baitPower + 1}";
            tblStorageLevel.Text = $"Storage - Level {(Player.inventorySlots + 10) / 10}";
            tblLocationLevel.Text = $"Location - Level {Player.Location + 1}";

            //Calculate the prices for the upgrades
            CalculatePrices();
        }

        private void btnUpgradeRod_Click(object sender, RoutedEventArgs e)
        {
            if (Player.money >= rodPrice)
            {
                //Remove money from player
                wndGame.RemoveMoney(Convert.ToDouble(rodPrice));

                //Upgrade level and display new level
                Player.rodPower++;
                tblRodLevel.Text = $"Rod - Level {Player.rodPower + 1}";

                //Update and display new price
                CalculatePrices();

                //Update money display
                tblMoney.Text = $"Money: {Player.money}";

                //Check if the rod is max level and don't allow further upgrades
                if(Player.rodPower >= 100)
                {
                    btnUpgradeRod.IsEnabled = false;
                    btnUpgradeRod.Content = "Maxed";
                    tblRodLevel.Text = "Bait - Level 100";
                    tblRodCost.Text = "Cost: ---";
                }
            }
        }

        private void btnUpgradeBait_Click(object sender, RoutedEventArgs e)
        {
            if (Player.money >= baitPrice)
            {
                //Remove money from player
                wndGame.RemoveMoney(baitPrice);

                //Upgrade level and display new level
                Player.baitPower++;
                tblBaitLevel.Text = $"Bait - Level {Player.baitPower + 1}";

                //Update and display new price
                CalculatePrices();

                //Update money display
                tblMoney.Text = $"Money: {Player.money}";

                //Check if the bait is max level and don't allow further upgrades
                if (Player.baitPower >= 10)
                {
                    btnUpgradeBait.IsEnabled = false;
                    btnUpgradeBait.Content = "Maxed";
                    tblBaitLevel.Text = "Bait - Level 10";
                    tblBaitCost.Text = "Cost: ---";
                }
            }
        }

        private void btnUpgradeStorage_Click(object sender, RoutedEventArgs e)
        {
            if (Player.money >= storagePrice)
            {
                //Remove money from player
                wndGame.RemoveMoney(storagePrice);

                //Upgrade level and display new level
                Player.inventorySlots += 10;
                tblStorageLevel.Text = $"Storage - Level {Player.inventorySlots + 10}";

                //Update and display new price
                CalculatePrices();

                //Update money display
                tblMoney.Text = $"Money: {Player.money}";
            }
        }
        private void btnUpgradeLocation_Click(object sender, RoutedEventArgs e)
        {
            if(Player.money >= LocationPrice)
            {
                //Remove money from player
                wndGame.RemoveMoney(LocationPrice);

                //Upgrade level and display new level
                Player.Location++;
                tblLocationLevel.Text = $"Location - Level {Player.Location + 1}";

                //Update and display new price
                CalculatePrices();

                //Update money display
                tblMoney.Text = $"Money: {Player.money}";


                wndGame.btnSwitchLocationForward.Visibility = Visibility.Visible;
                wndGame.btnCast.Visibility = Visibility.Hidden;
                wndGame.tblCaughtFish.Text = "You have to build several times";
                wndGame.tblCaughtFish.Visibility = Visibility.Visible;
                wndGame.StopAllTimers();
                wndGame.caughtNotifcationTimer.Interval = 5000;
                wndGame.caughtNotifcationTimer.Start();

                if (Player.Location >= 2)
                {
                    btnUpgradeLocation.IsEnabled = false;
                    btnUpgradeLocation.Content = "Coming Soon";
                    tblLocationCost.Text = "Cost: ???";

                }
            }
        }

        //-- Custom Methods --//

        private void CalculatePrices()
        {
            //Calculate prices for the next level
            rodPrice = (Player.rodPower + 1) * 120;            
            baitPrice = (Player.baitPower + 1) * 300;
            storagePrice = (Player.inventorySlots + 10) / 10 * 1100;
            LocationPrice = Player.Location * 10000;

            //Show prices
            tblRodCost.Text = $"Cost: {rodPrice}";
            if (Player.baitPower <= 9)
            {
                tblBaitCost.Text = $"Cost: {baitPrice}";
            }
            if (Player.baitPower >= 10)
            {
                btnUpgradeBait.IsEnabled = false;
                btnUpgradeBait.Content = "Maxed";
                tblBaitLevel.Text = "Bait - Level 10";
                tblBaitCost.Text = "Cost: ---";
            }
            tblStorageCost.Text = $"Cost: {storagePrice}";
            if (Player.Location == 1)
            {
                tblLocationCost.Text = $"Cost: {LocationPrice}";
            }
            if (Player.Location >= 2)
            {
                btnUpgradeLocation.IsEnabled = false;
                btnUpgradeLocation.Content = "Coming Soon";
                tblLocationCost.Text = "Cost: ???";

            }
        }
    }
}
