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
        public ScrollViewer archiveScrollViewer = new ScrollViewer();
        public static StackPanel archiveStackPanel = new StackPanel();
        public static List<ItemSlot> slotList = new List<ItemSlot>();
        wndGame wndGame = (wndGame)Application.Current.MainWindow;
        public static List<ItemSlot> itemSlots = new List<ItemSlot>();
        public static List<Fish> unarchivedFish = new List<Fish>();
        private int unarchivedFishAmount;


        public wndArchive()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadArchive();
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        public void LoadArchive()
        {
            //Create ScrollViewer and StackPanel to enable scrollability in the window
            archiveScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            archiveStackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            archiveStackPanel.VerticalAlignment = VerticalAlignment.Top;
            archiveStackPanel.Children.Clear();

            //Create Header text
            TextBlock tblHeader = new TextBlock();
            tblHeader.Margin = new Thickness(20, 5, 0, 0);
            tblHeader.FontSize = 30;
            tblHeader.FontWeight = FontWeights.Bold;

            //Create 'Unarchive selected' button
            Button btnUnarchiveSelected = new Button();
            btnUnarchiveSelected.Width = 220;
            btnUnarchiveSelected.Height = 30;
            btnUnarchiveSelected.Margin = new Thickness(480, 60, 0, 0);
            btnUnarchiveSelected.FontSize = 15;
            btnUnarchiveSelected.Content = "Unarchive selected fish";
            btnUnarchiveSelected.FontWeight = FontWeights.Bold;
            btnUnarchiveSelected.Click += btnUnarchiveSelected_click;


            //Place all top objects into header canvas
            Canvas cvsHeader = new Canvas();
            cvsHeader.Children.Add(tblHeader);
            cvsHeader.Children.Add(btnUnarchiveSelected);
            archiveStackPanel.Children.Add(cvsHeader);


            //Create textblock for empty archive
            TextBlock tblNoFish = new TextBlock();
            tblNoFish.Margin = new Thickness(200, 400, 0, 0);
            tblNoFish.FontSize = 30;
            tblNoFish.Text = "You don't have any fish archived!";
            if (Player.archive.Count == 0)
            {
                archiveStackPanel.Children.Add(tblNoFish);
            }
            
            //Create archive slots
            slotList.Clear();
            {
                for (int i = 0; i < Player.archive.Count; i++)
                {
                    slotList.Add(new ItemSlot(i,false));
                }
            }
            //Add the StackPanel as the lone child of the ScrollViewer
            archiveScrollViewer.Content = archiveStackPanel;

            //Add the ScrollViewer as the Content of the parent Window object
            Content = archiveScrollViewer;

            //Set header text depending on fish amount
            tblHeader.Text = $"Archive ({slotList.Count}/{Player.archiveSlots} fish Archived)";
        }


        void btnUnarchiveSelected_click(object sender, RoutedEventArgs e)
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
                //Reset archive stats
                unarchivedFishAmount = 0;

                if ( selectedFishAmount > Player.inventorySlots - Player.inventory.Count)
                {
                    MessageBox.Show("You dont have enough space in your inventory", "Unachived fish", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    //Archive selected fish to save them
                    foreach (ItemSlot slot in slotList)
                    {
                        if (slot.slotCheckBox.IsChecked == true)
                        {
                            //Archive the fish
                            UnArchiveFish(Player.archive[slot.slotNumber]);
                        }
                    }
                    //Refresh Archive
                    RefreshArchive();

                    //Show confirmation message with unarchive stats
                    MessageBox.Show($"Archived {unarchivedFishAmount} fish.", "Unarchived fish", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("You don't have any fish selected to unarchive", "Unarchive selected fish", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        public void RefreshArchive()
        {
            //Remove sold fish from the inventory
            foreach (Fish entry in unarchivedFish)
            {
                Player.archive.Remove(entry);
            }
            unarchivedFish.Clear();

            archiveStackPanel.Children.Clear();
            LoadArchive();
        }
        public void UnArchiveFish(Fish fish)
        {
            unarchivedFish.Add(fish);
            Player.inventory.Add(fish);
            unarchivedFishAmount++;
            
        }
    }
}
