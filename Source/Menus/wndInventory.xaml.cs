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
        public wndGame wndGame = (wndGame)Application.Current.MainWindow;
        public static StackPanel inventoryStackPanel = new StackPanel();
        public static List<ItemSlot> slotList = new List<ItemSlot>();
        public static List<Fish> soldFish = new List<Fish>();
        public static bool IsOpen;
        private int soldFishAmount;
        private double earnedMoney;

        //-- Constructor --//

        public wndInventory()
        {
            InitializeComponent();
        }

        //-- Event Handlers --//

        void btnSellAll_Click(object sender, RoutedEventArgs e)
        {
            if (Player.inventory.Count > 0)
            {
                if (MessageBox.Show("Are you sure that you want to sell all your fish?", "Sell all fish", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    //Reset sell stats
                    soldFishAmount = 0;
                    earnedMoney = 0;

                    //Sell all fish for money
                    foreach (Fish fish in Player.inventory)
                    {
                        //Sell the fish
                        SellFish(fish);
                    }
                    //Refresh Inventory
                    RefreshInventory();

                    //Show confirmation message with sell stats
                    MessageBox.Show($"Sold {soldFishAmount} fish for {earnedMoney} total money.", "Sold fish", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            else
            {
                MessageBox.Show("You don't have any fish to sell!", "Sell all fish", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        void btnSellSelected_Click(object sender, RoutedEventArgs e)
        {
            int selectedFishAmount = 0;

            foreach (ItemSlot slot in slotList)
            {
                if (slot.slotCheckBox.IsChecked == true)
                {
                    selectedFishAmount++;
                }
            }

            if (selectedFishAmount > 0)
            {
                //Reset sell stats
                soldFishAmount = 0;
                earnedMoney = 0;

                //Only sell selected fish for money
                foreach (ItemSlot slot in slotList)
                {
                    if (slot.slotCheckBox.IsChecked == true)
                    {
                        //Sell the fish
                        SellFish(Player.inventory[slot.slotNumber]);
                    }
                }

                //Refresh Inventory
                RefreshInventory();

                //Show confirmation message with sell stats
                MessageBox.Show(string.Format("Sold {0} fish for {1} total money.", soldFishAmount, earnedMoney), "Sold fish", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("You don't have any fish selected to sell", "Sell selected fish", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Load the inventory
            LoadInventory();
            IsOpen = true;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
            wndGame.Activate();
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
            if (Player.inventory.Count == 0)
            {
                inventoryStackPanel.Children.Add(tblNoFish);
            }

            //Create inventory slots
            slotList.Clear();
            {
                for (int i = 0; i < Player.inventory.Count; i++)
                {
                    slotList.Add(new ItemSlot(i));
                }
            }

            //Add the StackPanel as the lone child of the ScrollViewer
            inventoryScrollViewer.Content = inventoryStackPanel;

            //Add the ScrollViewer as the Content of the parent Window object
            Content = inventoryScrollViewer;

            //Set header text depending on fish amount
            tblHeader.Text = $"Inventory ({slotList.Count}/{Player.inventorySlots} fish)";
        }

        private void SellFish(Fish fish)
        {
            double rarityMultiplier = 1;

            //Get multiplier based on rarity
            switch (fish.rarity)
            {
                case Rarity.Common:
                    rarityMultiplier = 0.5;
                    break;
                case Rarity.Trash:
                    rarityMultiplier = 0.25;
                    break;
                case Rarity.Rare:
                    rarityMultiplier = 0.75;
                    break;
                case Rarity.SuperRare:
                    rarityMultiplier = 1;
                    break;
                case Rarity.Legendary:
                    rarityMultiplier = 1.5;
                    break;
                case Rarity.Special:
                    rarityMultiplier = 2;
                    break;
            }

            //Add money
            wndGame.AddMoney(fish.weight * rarityMultiplier);
            soldFishAmount++;
            earnedMoney = Math.Round(earnedMoney + fish.weight * rarityMultiplier);
            soldFish.Add(fish);
        }

        public void RefreshInventory()
        {
            //Remove sold fish from the inventory
            foreach (Fish entry in soldFish)
            {
                Player.inventory.Remove(entry);
            }
            soldFish.Clear();

            //Clear the window and load it again
            inventoryStackPanel.Children.Clear();
            LoadInventory();
        }
    }

    //Item slot
    public class ItemSlot
    {
        //Attributes of an item slot
        public Border slotBorder = new Border();
        public Canvas slotCanvas = new Canvas();
        public TextBlock slotTextblock = new TextBlock();
        public CheckBox slotCheckBox = new CheckBox();
        public Fish fish;
        public int slotNumber;

        public ItemSlot(int num)
        {
            slotNumber = num;

            //Get item string and split it up
            fish = Player.inventory[slotNumber];

            //Add textblock to canvas
            slotTextblock.Text = $"Fish #{slotNumber + 1}: {fish.name} ({fish.rarity})\nWeight: {fish.weight}KG - Caught on: {fish.caughtDate}";
            slotTextblock.FontSize = 20;
            slotTextblock.Margin = new Thickness(10, 0, 0, 0);
            slotCanvas.Children.Add(slotTextblock);

            //Add checkbox to canvas
            slotCheckBox.Margin = new Thickness(415, 5, 0, 0);
            slotCanvas.Children.Add(slotCheckBox);

            //Set backcolor of slot depending on rarity
            switch (fish.rarity)
            {
                case Rarity.Common:
                    slotCanvas.Background = Brushes.LightGray;
                    break;
                case Rarity.Trash:
                    slotCanvas.Background = Brushes.DarkGray;
                    break;
                case Rarity.Rare:
                    slotCanvas.Background = Brushes.LightBlue;
                    break;
                case Rarity.SuperRare:
                    slotCanvas.Background = Brushes.Violet;
                    break;
                case Rarity.Legendary:
                    slotCanvas.Background = Brushes.Orange;
                    break;
                case Rarity.Special:
                    slotCanvas.Background = Brushes.IndianRed;
                    break;
            }

            //Set up border of slot
            Border border = new Border();

            border.Margin = slotNumber == 0 ? new Thickness(25, 110, 0, 0) : new Thickness(25, 25, 0, 0);

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

