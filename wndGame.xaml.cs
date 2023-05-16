using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SealFisher
{

	public partial class MainWindow : Window
	{

		//-- Variables --//

		//General
		System.Windows.Threading.DispatcherTimer fishTimer = new System.Windows.Threading.DispatcherTimer();
		System.Windows.Threading.DispatcherTimer caughtNotifcationTimer = new System.Windows.Threading.DispatcherTimer();
		string rodState = "idle";
		int rodpower;
		int baitpower;
		int fishCatchTime;
		Random random = new Random();

		//Images
		static Uri imageUribackground_default = new Uri("pack://application:,,,/SealFisher;component/Resources/background_default.png");
		BitmapImage background_default = new BitmapImage(imageUribackground_default);
		static Uri imageUribackground_fisher_rodcasted = new Uri("pack://application:,,,/SealFisher;component/Resources/background_fisher_rodcasted.png");
		BitmapImage background_fisher_rodcasted = new BitmapImage(imageUribackground_fisher_rodcasted);
		static Uri imageUribackground_fisher_idle = new Uri("pack://application:,,,/SealFisher;component/Resources/background_fisher_idle.png");
		BitmapImage background_fisher_idle = new BitmapImage(imageUribackground_fisher_idle);
		wndInventory wndInventory;

		//-- Constructor --//

		public MainWindow()
		{
			InitializeComponent();
			fishTimer.Tick += new EventHandler(fishTimer_Tick);
			caughtNotifcationTimer.Tick += new EventHandler(caughtNotificationTimer_Tick);
		}

		//-- Event Handlers --//

		private void fishTimer_Tick(object sender, EventArgs e)
		{
			//Make fish catchable
			SetRodState("catchable");
			tblFishWarning.Visibility = Visibility.Visible;
			fishTimer.Stop();
			//WIP - MAKE FISH DISAPPEAR AFTER 3 SECONDS OF NOT CLICKING ON THE BUTTON!
		}

		private void caughtNotificationTimer_Tick(object sender, EventArgs e)
		{
			//Hide 'fish caught' notification
			tblCaughtFish.Visibility = Visibility.Hidden;
			caughtNotifcationTimer.Stop();
		}

		private void btnCast_Click(object sender, RoutedEventArgs e)
		{
			//Check rodstate, then execute actions depending on rodstate
			if (rodState == "idle")
			{
				//Check if inventory is full
				if (InventoryFull() == false)
				{
					SetRodState("casted");

					//Start timer for fish spawn
					fishCatchTime = random.Next(3, 10);
					fishTimer.Interval = new TimeSpan(0, 0, fishCatchTime);
					fishTimer.Start();
				}
				else
				{
					MessageBox.Show("Your inventory is full. Please sell some fish or upgrade your storage.", "Inventory full", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}

			}
			else if (rodState == "casted")
			{
				//Retreat rod without getting a fish
				SetRodState("idle");
				fishTimer.Stop();
			}
			else if (rodState == "catchable")
			{
				//Retreat rod and get fish
				SetRodState("idle");
				fishTimer.Stop();
				tblFishWarning.Visibility = Visibility.Hidden;
				GenerateFish();
			}

		}


		private void btnInventory_Click(object sender, RoutedEventArgs e)
		{
			//Open inventory window
			wndInventory = new wndInventory() { Owner = this };
			wndInventory.Owner = Application.Current.MainWindow;
			wndInventory.ShowDialog();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			//DEBUG CHEAT!
			int i = 0;
			while (i < 100)
			{
				GenerateFish();
				i++;
			}
		}

		//-- Custom Methods --//

		public void AddMoney(double money)
		{
			//Add money and round to 0 decimals
			publicVariables.money = Math.Round(publicVariables.money + money, 0);
			tblMoney.Text = string.Format("Money: {0}", publicVariables.money.ToString());
		}

		public bool InventoryFull()
		{
			if (publicVariables.inventory.Count >= publicVariables.fishInventorySlots)
			{
				return true;
			}

			else
			{
				return false;
			}
		}

		private void SetRodState(string state)
		{
			if (state == "idle")
			{
				//Rod is not thrown out
				rodState = "idle";
				btnCast.Content = "Cast";
				imgBackground.Source = background_fisher_idle;
			}
			else if (state == "catchable")
			{
				//Rod is thrown out and there is a catchable fish
				btnCast.Content = "Catch";
				rodState = "catchable";
				imgBackground.Source = background_fisher_rodcasted;
			}
			else if (state == "casted")
			{
				//rod is thrown out but there is no fish
				rodState = "casted";
				imgBackground.Source = background_fisher_rodcasted;
				btnCast.Content = "Reel in";
			}
		}

		private void GenerateFish()
		{
			//Generate fish based on a certain number
			int fishNumber = random.Next(1, 676 - (rodpower * 15));
			rodpower = 1;

			if (fishNumber >= 1 && fishNumber <= (500 - (rodpower * 10)))
			{
				CatchFish("Common");
			}
			else if (fishNumber >= (501 - (rodpower * 10)) && fishNumber <= (625 - (rodpower * 15)))
			{
				CatchFish("Trash");
			}
			else if (fishNumber >= (626 - (rodpower * 15)) && fishNumber <= (650) - (rodpower * 15))
			{
				CatchFish("Rare");
			}
			else if (fishNumber >= (651 - (rodpower * 15)) && fishNumber <= (665 - (rodpower * 15)))
			{
				CatchFish("Super rare");
			}
			else if (fishNumber >= (666 - (rodpower * 15)) && fishNumber <= (670 - (rodpower * 15)))
			{
				CatchFish("Legendary");
			}
			else if (fishNumber >= (671 - (rodpower * 15)) && fishNumber <= (675 - (rodpower * 15)))
			{
				CatchFish("Special");
			}

		}

		private void CatchFish(string rarity)
		{
			//Common fish
			int fishNum = 0;
			string fishName = "";

			if (rarity == "Common")
			{
				fishNum = random.Next(1, 11);
				if (fishNum == 1)
				{
					fishName = "Trout";
				}
				else if (fishNum == 2)
				{
					fishName = "Salmon";
				}
				else if (fishNum == 3)
				{
					fishName = "Tuna";
				}
				else if (fishNum == 4)
				{
					fishName = "Pike";
				}
				else if (fishNum == 5)
				{
					fishName = "Herring";
				}
				else if (fishNum == 6)
				{
					fishName = "Zander";
				}
				else if (fishNum == 7)
				{
					fishName = "Eel";
				}
				else if (fishNum == 8)
				{
					fishName = "Cod";
				}
				else if (fishNum == 9)
				{
					fishName = "Catfish";
				}
				else if (fishNum == 10)
				{
					fishName = "Carp";
				}
			}
			else if (rarity == "Trash")
			{
				fishNum = random.Next(1, 6);
				if (fishNum == 1)
				{
					fishName = "Plastic bottle";
				}
				else if (fishNum == 2)
				{
					fishName = "Metal can";
				}
				else if (fishNum == 3)
				{
					fishName = "Cigarrete";
				}
				else if (fishNum == 4)
				{
					fishName = "Broken phone";
				}
				else if (fishNum == 5)
				{
					fishName = "Plastic bag";
				}
			}
			else if (rarity == "Rare")
			{
				fishNum = random.Next(1, 6);
				if (fishNum == 1)
				{
					fishName = "Coelacanth";
				}
				else if (fishNum == 2)
				{
					fishName = "Arapaima";
				}
				else if (fishNum == 3)
				{
					fishName = "Megamouth shark";
				}
				else if (fishNum == 4)
				{
					fishName = "Manta ray";
				}
				else if (fishNum == 5)
				{
					fishName = "Olm";
				}
			}
			else if (rarity == "Super rare")
			{
				fishNum = random.Next(1, 6);
				if (fishNum == 1)
				{
					fishName = "Blacksnake cusk-eel";
				}
				else if (fishNum == 2)
				{
					fishName = "Hainan dogfis";
				}
				else if (fishNum == 3)
				{
					fishName = "Denison barb";
				}
				else if (fishNum == 4)
				{
					fishName = "Harlequin ghost pipefish";
				}
				else if (fishNum == 5)
				{
					fishName = "Devil's Hole pupfish";
				}
			}
			else if (rarity == "Legendary")
			{
				fishNum = random.Next(1, 6);
				if (fishNum == 1)
				{
					fishName = "Salamanderfish";
				}
				else if (fishNum == 2)
				{
					fishName = "Red-mouthed cichlid";
				}
				else if (fishNum == 3)
				{
					fishName = "Longfin triplefin";
				}
				else if (fishNum == 4)
				{
					fishName = "Great White Shark";
				}
				else if (fishNum == 5)
				{
					fishName = "Chinese sturgeon";
				}
			}
			else if (rarity == "Special")
			{
				fishNum = random.Next(1, 6);
				if (fishNum == 1)
				{
					fishName = "Anchor";
				}
				else if (fishNum == 2)
				{
					fishName = "Seal";
				}
				else if (fishNum == 3)
				{
					fishName = "Diamond Sword";
				}
				else if (fishNum == 4)
				{
					fishName = "Nuclear Waste";
				}
				else if (fishNum == 5)
				{
					fishName = "Bomb";
				}
			}

			//Set weight based on fish rarity
			int weight = 0;
			if (rarity == "Common")
			{
				weight = random.Next(1, (50 + baitpower));
			}
			if (rarity == "Trash")
			{
				weight = random.Next(1, (10 + baitpower));
			}
			else if (rarity == "Rare")
			{
				weight = random.Next(50, (100 + baitpower));
			}
			else if (rarity == "Super rare")
			{
				weight = random.Next(100, (150 + baitpower));
			}
			else if (rarity == "Legendary")
			{
				weight = random.Next(150, (200 + baitpower));
			}
			else if (rarity == "Special")
			{
				weight = 69;
			}

			//Add fish to inventory
			publicVariables.inventory.Add(String.Format("{0};{1};{2};{3}", fishName, weight, rarity, DateTime.Now));

			//Show 'caught fish' notification
			tblCaughtFish.Text = String.Format("You caught a {0} ({1}) weighing {2} KG", fishName, rarity, weight);
			tblCaughtFish.Visibility = Visibility.Visible;
			caughtNotifcationTimer.Stop();
			caughtNotifcationTimer.Interval = new TimeSpan(0, 0, 5);
			caughtNotifcationTimer.Start();

		}
	}
}

