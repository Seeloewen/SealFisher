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

	public partial class wndShop : Window
	{

		MainWindow wndGame = (MainWindow)Application.Current.MainWindow;

		int rodPrice;
		int baitPrice;
		int storagePrice;

		public wndShop()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//Set money display
			tblMoney.Text = string.Format("Money: {0}", publicVariables.playerStats[1]);

			//Calculate the prices for the upgrades
			CalculatePrices();

			//Display levels
			tblRodLevel.Text = string.Format("Rod - Level {0}", Convert.ToInt32(publicVariables.playerStats[2]) + 1);
			tblBaitLevel.Text = string.Format("Bait - Level {0}", Convert.ToInt32(publicVariables.playerStats[3]) + 1);
			tblStorageLevel.Text = string.Format("Storage - Level {0}", (Convert.ToInt32(publicVariables.playerStats[4]) + 10) / 10);
		}

		private void btnUpgradeRod_Click(object sender, RoutedEventArgs e)
		{
			if (Convert.ToInt32(publicVariables.playerStats[1]) >= rodPrice)
			{
				//Remove money from player
				wndGame.AddMoney(-Convert.ToDouble(rodPrice));

				//Upgrade level and display new level
				publicVariables.playerStats[2] = Convert.ToString(Convert.ToInt32(publicVariables.playerStats[2]) + 1);
				tblRodLevel.Text = string.Format("Rod - Level {0}", Convert.ToInt32(publicVariables.playerStats[2]) + 1);

				//Update and display new price
				CalculatePrices();

				//Update money display
				tblMoney.Text = string.Format("Money: {0}", publicVariables.playerStats[1]);
			}
		}

		private void CalculatePrices()
		{
			//Calculate prices for the next level
			rodPrice = (Convert.ToInt32(publicVariables.playerStats[2]) + 1) * 120;
			baitPrice = (Convert.ToInt32(publicVariables.playerStats[3]) + 1) * 550;
			storagePrice = ((Convert.ToInt32(publicVariables.playerStats[4]) + 10) / 10) * 1100;

			//Show prices
			tblRodCost.Text = string.Format("Cost: {0}", Convert.ToString(rodPrice));
			tblBaitCost.Text = string.Format("Cost: {0}", Convert.ToString(baitPrice));
			tblStorageCost.Text = string.Format("Cost: {0}", Convert.ToString(storagePrice));
		}

		private void btnUpgradeBait_Click(object sender, RoutedEventArgs e)
		{
			if (Convert.ToInt32(publicVariables.playerStats[1]) >= baitPrice)
			{
				//Remove money from player
				wndGame.AddMoney(-Convert.ToDouble(baitPrice));

				//Upgrade level and display new level
				publicVariables.playerStats[3] = Convert.ToString(Convert.ToInt32(publicVariables.playerStats[3]) + 1);
				tblBaitLevel.Text = string.Format("Bait - Level {0}", Convert.ToInt32(publicVariables.playerStats[3]) + 1);

				//Update and display new price
				CalculatePrices();

				//Update money display
				tblMoney.Text = string.Format("Money: {0}", publicVariables.playerStats[1]);
			}
		}

		private void btnUpgradeStorage_Click(object sender, RoutedEventArgs e)
		{
			if (Convert.ToInt32(publicVariables.playerStats[1]) >= storagePrice)
			{
				//Remove money from player
				wndGame.AddMoney(-Convert.ToDouble(storagePrice));

				//Upgrade level and display new level
				publicVariables.playerStats[4] = Convert.ToString(Convert.ToInt32(publicVariables.playerStats[4]) + 10);
				tblStorageLevel.Text = string.Format("Storage - Level {0}", (Convert.ToInt32(publicVariables.playerStats[4]) + 10) / 10);

				//Update and display new price
				CalculatePrices();

				//Update money display
				tblMoney.Text = string.Format("Money: {0}", publicVariables.playerStats[1]);
			}
		}
	}
}
