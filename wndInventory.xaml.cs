using SealFisher;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
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
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace SealFisher
{

	//Window
	public partial class wndInventory : Window
	{

		//-- Variables --//

		public ScrollViewer inventoryScrollViewer = new ScrollViewer();
		public static StackPanel inventoryStackPanel = new StackPanel();
		public static string[] itemList = new string[3];
		public static string[] sellItemList = new string[3];
		public static List<itemSlot> slotList = new List<itemSlot>();
		public static List<string> soldFish = new List<string>();
		MainWindow wndGame = (MainWindow)Application.Current.MainWindow;

		//-- Constructor --//

		public wndInventory()
		{
			InitializeComponent();
		}

		//-- Event Handlers --//

		void btnSellAll_Click(object sender, RoutedEventArgs e)
		{
			//Sell all fish for money
			foreach (string fish in publicVariables.inventory)
			{
				//Sell the fish
				SellFish(fish);
			}

			//Refresh Inventory
			RefreshInventory();
		}

		void btnSellSelected_Click(object sender, RoutedEventArgs e)
		{
			//Only sell selected fish for money
			foreach (itemSlot slot in slotList)
			{
				if (slot.slotCheckBox.IsChecked == true)
				{
					//Sell the fish
					SellFish(publicVariables.inventory[slot.slotNumber]);
				}
			}

			//Refresh Inventory
			RefreshInventory();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//Load the inventory
			LoadInventory();
		}

		//-- Custom Methods --//

		public void LoadInventory()
		{
			//Create ScrollViewer and StackPanel to enable scrollability in the window
			inventoryScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
			inventoryStackPanel.HorizontalAlignment = HorizontalAlignment.Left;
			inventoryStackPanel.VerticalAlignment = VerticalAlignment.Top;
			inventoryStackPanel.Children.Clear();

			//Create Header text
			TextBlock tblHeader = new TextBlock();
			tblHeader.Margin = new Thickness(20, 5, 0, 0);
			tblHeader.FontSize = 30;
			tblHeader.FontWeight = FontWeights.Bold;

			//Create 'sell all' button
			Button btnSellAll = new Button();
			btnSellAll.Width = 220;
			btnSellAll.Height = 30;
			btnSellAll.Margin = new Thickness(20, 60, 0, 0);
			btnSellAll.FontSize = 15;
			btnSellAll.Content = "Sell all fish";
			btnSellAll.FontWeight = FontWeights.Bold;
			btnSellAll.Click += btnSellAll_Click;

			//Create 'sell selected' button
			Button btnSellSelected = new Button();
			btnSellSelected.Width = 220;
			btnSellSelected.Height = 30;
			btnSellSelected.Margin = new Thickness(250, 60, 0, 0);
			btnSellSelected.FontSize = 15;
			btnSellSelected.Content = "Sell only selected fish";
			btnSellSelected.FontWeight = FontWeights.Bold;
			btnSellSelected.Click += btnSellSelected_Click;

			//Place all top objects into header canvas
			Canvas cvsHeader = new Canvas();
			cvsHeader.Children.Add(tblHeader);
			cvsHeader.Children.Add(btnSellAll);
			cvsHeader.Children.Add(btnSellSelected);
			inventoryStackPanel.Children.Add(cvsHeader);

			//Create textblock for empty inventory
			TextBlock tblNoFish = new TextBlock();
			tblNoFish.Margin = new Thickness(100, 400, 0, 0);
			tblNoFish.FontSize = 30;
			tblNoFish.Text = "You don't have any fish!";
			if (publicVariables.inventory.Count == 0)
			{
				inventoryStackPanel.Children.Add(tblNoFish);
			}

			//Create inventory slots
			slotList.Clear();
			{
				for (int i = 0; i < publicVariables.inventory.Count; i++)
				{
					slotList.Add(new itemSlot(i));
				}
			}

			//Add the StackPanel as the lone child of the ScrollViewer
			inventoryScrollViewer.Content = inventoryStackPanel;

			//Add the ScrollViewer as the Content of the parent Window object
			Content = inventoryScrollViewer;

			//Set header text depending on fish amount
			tblHeader.Text = String.Format("Inventory ({0}/{1} fish)", slotList.Count, publicVariables.fishInventorySlots);
		}

		private void SellFish(string fish)
		{
			double rarityMultiplier = 1;

			//Sell fish - WIP
			sellItemList = fish.Split(';');
			if (sellItemList[2] == "Common")
			{
				rarityMultiplier = 0.5;
			}
			else if (sellItemList[2] == "Trash")
			{
				rarityMultiplier = 0.25;
			}
			else if (sellItemList[2] == "Rare")
			{
				rarityMultiplier = 0.75;
			}
			else if (sellItemList[2] == "Super rare")
			{
				rarityMultiplier = 1;
			}
			else if (sellItemList[2] == "Legendary")
			{
				rarityMultiplier = 1.5;
			}
			else if (sellItemList[2] == "Special")
			{
				rarityMultiplier = 2;
			}

			//Add money
			wndGame.AddMoney(Convert.ToInt32(sellItemList[1]) * rarityMultiplier);
			soldFish.Add(fish);
		}

		private void RefreshInventory()
		{
			//Remove sold fish from the inventory
			foreach (string entry in soldFish)
			{
				publicVariables.inventory.Remove(entry);
			}
			soldFish.Clear();

			//Clear the window and load it again
			inventoryStackPanel.Children.Clear();
			LoadInventory();
		}

	}

	//Item slot
	public class itemSlot
	{
		//Attributes of an item slot
		public Border slotBorder = new Border();
		public Canvas slotCanvas = new Canvas();
		public TextBlock slotTextblock = new TextBlock();
		public CheckBox slotCheckBox = new CheckBox();
		public int slotNumber;

		public itemSlot(int num)
		{
			slotNumber = num;

			//Get item string and split it up
			wndInventory.itemList = publicVariables.inventory[slotNumber].Split(';');

			//Add textblock to canvas
			slotTextblock.Text = string.Format("Fish #{0}: {1} ({2})\nWeight: {3}KG - Caught on: {4}", slotNumber + 1, wndInventory.itemList[0], wndInventory.itemList[2], wndInventory.itemList[1], wndInventory.itemList[3]);
			slotTextblock.FontSize = 20;
			slotTextblock.Margin = new Thickness(10, 0, 0, 0);
			slotCanvas.Children.Add(slotTextblock);

			//Add checkbox to canvas
			slotCheckBox.Margin = new Thickness(415, 5, 0, 0);
			slotCanvas.Children.Add(slotCheckBox);

			//Set backcolor of slot depending on rarity
			if (wndInventory.itemList[2] == "Common")
			{
				slotCanvas.Background = Brushes.LightGray;
			}
			else if (wndInventory.itemList[2] == "Trash")
			{
				slotCanvas.Background = Brushes.DarkGray;
			}
			else if (wndInventory.itemList[2] == "Rare")
			{
				slotCanvas.Background = Brushes.LightBlue;
			}
			else if (wndInventory.itemList[2] == "Super rare")
			{
				slotCanvas.Background = Brushes.Violet;
			}
			else if (wndInventory.itemList[2] == "Legendary")
			{
				slotCanvas.Background = Brushes.Orange;
			}
			else if (wndInventory.itemList[2] == "Special")
			{
				slotCanvas.Background = Brushes.IndianRed;
			}

			//Set up border of slot
			Border border = new Border();
			if (slotNumber == 0)
			{
				border.Margin = new Thickness(25, 110, 0, 0);
			}
			else
			{
				border.Margin = new Thickness(25, 25, 0, 0);
			}
			border.Height = 70;
			border.Width = 440;
			border.BorderBrush = Brushes.Black;
			border.BorderThickness = new Thickness(2);
			border.Child = slotCanvas;

			//Add slot to inventory
			wndInventory.inventoryStackPanel.Children.Add(border);
		}
	}
}

