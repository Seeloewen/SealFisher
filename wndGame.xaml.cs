using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
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
				SetRodState("casted");

				//Start timer for fish spawn
				fishCatchTime = random.Next(3, 10);
				fishTimer.Interval = new TimeSpan(0, 0, fishCatchTime);
				fishTimer.Start();
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
			int fishNumber = random.Next(1, 675 - (rodpower * 15));

			//Common fish
			if (fishNumber >= 1 && fishNumber <= (50 - rodpower))
			{
				CatchFish("Common", "Carp");
			}
			else if (fishNumber >= (51 - rodpower) && fishNumber <= (100 - (rodpower * 2)))
			{
				CatchFish("Common", "Trout");
			}
			else if (fishNumber >= (101 - (rodpower * 2)) && fishNumber <= (150 - (rodpower * 3)))
			{
				CatchFish("Common", "Salmon");
			}
			else if (fishNumber >= (151 - (rodpower * 3)) && fishNumber <= (200 - (rodpower * 4)))
			{
				CatchFish("Common", "Tuna");
			}
			else if (fishNumber >= (201 - (rodpower * 4)) && fishNumber <= (250 - (rodpower * 5)))
			{
				CatchFish("Common", "Pike");
			}
			else if (fishNumber >= (251 - (rodpower * 5)) && fishNumber <= (300 - (rodpower * 6)))
			{
				CatchFish("Common", "Herring");
			}
			else if (fishNumber >= (301 - (rodpower * 6)) && fishNumber <= (350 - (rodpower * 7)))
			{
				CatchFish("Common", "Zander");
			}
			else if (fishNumber >= (351 - (rodpower * 7)) && fishNumber <= (400 - (rodpower * 8)))
			{
				CatchFish("Common", "Eel");
			}
			else if (fishNumber >= (401 - (rodpower * 8)) && fishNumber <= (450 - (rodpower * 9)))
			{
				CatchFish("Common", "Cod");
			}
			else if (fishNumber >= (451 - (rodpower * 9)) && fishNumber <= (500 - (rodpower * 10)))
			{
				CatchFish("Common", "Catfish");
			}
			//Trash
			else if (fishNumber >= (501 - (rodpower * 10)) && fishNumber <= (525 - (rodpower * 11)))
			{
				CatchFish("Trash", "Plastic bottle");
			}
			else if (fishNumber >= (526 - (rodpower * 11)) && fishNumber <= (550 - (rodpower * 12)))
			{
				CatchFish("Trash", "Metal can");
			}
			else if (fishNumber >= (551 - (rodpower * 12)) && fishNumber <= (575 - (rodpower * 13)))
			{
				CatchFish("Trash", "Cigarrete");
			}
			else if (fishNumber >= (576 - (rodpower * 13)) && fishNumber <= (600 - (rodpower * 14)))
			{
				CatchFish("Trash", "Broken phone");
			}
			else if (fishNumber >= (601 - (rodpower * 14)) && fishNumber <= (625 - (rodpower * 15)))
			{
				CatchFish("Trash", "Plastic bag");
			}
			//Rare fish
			else if (fishNumber >= (626 - (rodpower * 15)) && fishNumber <= (630) - (rodpower * 15))
			{
				CatchFish("Rare", "Coelacanth");
			}
			else if (fishNumber >= (631 - (rodpower * 15)) && fishNumber <= (635) - (rodpower * 15))
			{
				CatchFish("Rare", "Arapaima");
			}
			else if (fishNumber >= (636 - (rodpower * 15)) && fishNumber <= (640) - (rodpower * 15))
			{
				CatchFish("Rare", "Megamouth shark");
			}
			else if (fishNumber >= (641 - (rodpower * 15)) && fishNumber <= (645) - (rodpower * 15))
			{
				CatchFish("Rare", "Manta ray");
			}
			else if (fishNumber >= (646 - (rodpower * 15)) && fishNumber <= (650) - (rodpower * 15))
			{
				CatchFish("Rare", "Olm");
			}
			//Super rare fish
			else if (fishNumber >= (651 - (rodpower * 15)) && fishNumber <= (653 - (rodpower * 15)))
			{
				CatchFish("Super rare", "Blacksnake cusk-eel");
			}
			else if (fishNumber >= (654 - (rodpower * 15)) && fishNumber <= (656 - (rodpower * 15)))
			{
				CatchFish("Super rare", "Hainan dogfish");
			}
			else if (fishNumber >= (657 - (rodpower * 15)) && fishNumber <= (659 - (rodpower * 15)))
			{
				CatchFish("Super rare", "Denison barb");
			}
			else if (fishNumber >= (660 - (rodpower * 15)) && fishNumber <= (662 - (rodpower * 15)))
			{
				CatchFish("Super rare", "Harlequin ghost pipefish");
			}
			else if (fishNumber >= (663 - (rodpower * 15)) && fishNumber <= (665 - (rodpower * 15)))
			{
				CatchFish("Super rare", "Devil's Hole pupfish");
			}
			//Legendary fish
			else if (fishNumber == (666 - (rodpower * 15)))
			{
				CatchFish("Legendary", "Salamanderfish");
			}
			else if (fishNumber == (667 - (rodpower * 15)))
			{
				CatchFish("Legendary", "Red-mouthed cichlid");
			}
			else if (fishNumber == (668 - (rodpower * 15)))
			{
				CatchFish("Legendary", "Longfin triplefin");
			}
			else if (fishNumber == (669 - (rodpower * 15)))
			{
				CatchFish("Legendary", "Great White Shark");
			}
			else if (fishNumber == (670 - (rodpower * 15)))
			{
				CatchFish("Legendary", "Chinese sturgeon");
			}
			//Special items
			else if (fishNumber == (671 - (rodpower * 15)))
			{
				CatchFish("Special", "Anchor");
			}
			else if (fishNumber == (672 - (rodpower * 15)))
			{
				CatchFish("Special", "Seal");
			}
			else if (fishNumber == (673 - (rodpower * 15)))
			{
				CatchFish("Special", "Diamond Sword");
			}
			else if (fishNumber == (674 - (rodpower * 15)))
			{
				CatchFish("Special", "Nuclear Waste");
			}
			else if (fishNumber == (675 - (rodpower * 15)))
			{
				CatchFish("Special", "???");
			}

		}

		private void CatchFish(string rarity, string fishname)
		{
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
			publicVariables.inventory.Add(String.Format("{0};{1};{2};{3}", fishname, weight, rarity, DateTime.Now));

			//Show 'caught fish' notification
			tblCaughtFish.Text = String.Format("You caught a {0} ({1}) weighing {2} KG", fishname, rarity, weight);
			tblCaughtFish.Visibility = Visibility.Visible;
			caughtNotifcationTimer.Stop();
			caughtNotifcationTimer.Interval = new TimeSpan(0, 0, 5);
			caughtNotifcationTimer.Start();

		}
	}
}

