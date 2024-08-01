using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
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
using static System.Windows.Forms.LinkLabel;

namespace SealFisher
{

    public partial class MainWindow : Window
    {

        //-- Variables --//

        //General
        System.Windows.Threading.DispatcherTimer fishTimer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer caughtNotifcationTimer = new System.Windows.Threading.DispatcherTimer();
        DispatcherTimer FishwarningdisappearTimer = new DispatcherTimer();
        string rodState = "idle";
        int fishCatchTime;
        Random random = new Random();
        List<FishType> fishTypes;

        //Images
        static Uri imageUribackground_default = new Uri("pack://application:,,,/SealFisher;component/Resources/background_default.png");
        BitmapImage background_default = new BitmapImage(imageUribackground_default);
        static Uri imageUribackground_fisher_rodcasted = new Uri("pack://application:,,,/SealFisher;component/Resources/background_fisher_rodcasted.png");
        BitmapImage background_fisher_rodcasted = new BitmapImage(imageUribackground_fisher_rodcasted);
        static Uri imageUribackground_fisher_idle = new Uri("pack://application:,,,/SealFisher;component/Resources/background_fisher_idle.png");
        BitmapImage background_fisher_idle = new BitmapImage(imageUribackground_fisher_idle);

        //Windows
        wndInventory wndInventory;
        wndShop wndShop;

        //Directories and files
        static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string gameDirectory = appData + "/SealFisher";
        string statsSaveFile = gameDirectory + "/statistics.txt";
        string inventorySaveFile = gameDirectory + "/inventory.txt";

        //-- Constructor --//

        public MainWindow()
        {
            InitializeComponent();
            fishTimer.Tick += new EventHandler(fishTimer_Tick);
            FishwarningdisappearTimer.Tick += new EventHandler(FishwarningdisappearTimer_Tick);
            caughtNotifcationTimer.Tick += new EventHandler(caughtNotificationTimer_Tick);
            FishwarningdisappearTimer.Interval = new TimeSpan(0, 0, 3);
        }

        //-- Event Handlers --//

        private void wndGame_Loaded(object sender, RoutedEventArgs e)
        {
            CreateGameDirectory();
            LoadGame();
            InitializeStats();
            InitializeFish();
        }

        private void fishTimer_Tick(object sender, EventArgs e)
        {
            //Make fish catchable
            SetRodState("catchable");
            tblFishWarning.Visibility = Visibility.Visible;
            fishTimer.Stop();
            FishwarningdisappearTimer.Start();
        }
        private void FishwarningdisappearTimer_Tick(object sender, EventArgs e)
        {
            //MAKE FISH DISAPPEAR AFTER 3 SECONDS OF NOT CLICKING ON THE BUTTON!
            SetRodState("casted");
            tblFishWarning.Visibility = Visibility.Hidden;
            FishwarningdisappearTimer.Stop();
            fishTimer.Start();
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
                    fishCatchTime = random.Next(1, 12);
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
                FishwarningdisappearTimer.Stop();
                tblFishWarning.Visibility = Visibility.Hidden;
                GenerateFish();
            }

        }


        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            //Open inventory window
            wndInventory = new wndInventory() { Owner = this };
            wndInventory.Owner = Application.Current.MainWindow;
            if (wndInventory.IsOpen == false)
            {
                wndInventory.Show();
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //DEBUG CHEAT! 
            int i = 0;
            while (i < 10)
            {
                GenerateFish();
                i++;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Save the game
            SaveGame(true);
        }

        private void btnShop_Click(object sender, RoutedEventArgs e)
        {
            //Open shop window
            wndShop = new wndShop() { Owner = this };
            wndShop.Owner = Application.Current.MainWindow;
            wndShop.ShowDialog();
        }

        //-- Custom Methods --//

        public void CreateGameDirectory()
        {
            //Checks if all necessary directories are there and creates them if not
            try
            {
                if (Directory.Exists(appData))
                {
                    if (!Directory.Exists(gameDirectory))
                    {
                        Directory.CreateDirectory(gameDirectory);
                    }
                }
                else
                {
                    MessageBox.Show("Could not create game directory, because your Appdata directory is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Could not create game directory: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool NecessaryDirectoriesExist()
        {
            //Checks if all directories, that the game needs, exist
            if (Directory.Exists(appData) && Directory.Exists(gameDirectory))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void SaveGame(bool showConfirmation)
        {
            //Check if necessary directories for saving the files exist
            if (NecessaryDirectoriesExist() == true)
            {
                try
                {
                    //Save inventory
                    using (StreamWriter file = new StreamWriter(inventorySaveFile))
                    {
                        foreach (string item in publicVariables.inventory)
                            file.WriteLine(item);
                    }

                    //Save user progress/stats
                    using (StreamWriter file = new StreamWriter(statsSaveFile))
                    {
                        foreach (string statistic in publicVariables.playerStats)
                            file.WriteLine(statistic);
                    }

                    //Show save confirmation if enabled
                    if (showConfirmation == true)
                    {
                        MessageBox.Show("Your inventory and player stats have been saved successfully.", "Saved game", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Could not save your game: {0}", ex.Message), "Error when saving game", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Could not save your game because of issues with necessary directories. Try restarting the app. Note that you will loose all unsaved progress.", "Error when saving game", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void LoadGame()
        {
            if (NecessaryDirectoriesExist() == true)
            {
                try
                {
                    //Load inventory
                    var loadedItems = File.ReadLines(inventorySaveFile);
                    foreach (var item in loadedItems)
                    {
                        publicVariables.inventory.Add(item);
                    }

                    //Load player stats
                    string[] loadedStats = File.ReadAllLines(statsSaveFile);
                    publicVariables.playerStats[0] = loadedStats[0]; //Started playing date
                    publicVariables.playerStats[1] = loadedStats[1]; //Money
                    publicVariables.playerStats[2] = loadedStats[2]; //Rodpower
                    publicVariables.playerStats[3] = loadedStats[3]; //Baitpower
                    publicVariables.playerStats[4] = loadedStats[4]; //Fish inventory slots

                    //Update UI elements
                    tblMoney.Text = String.Format("Money: {0}", publicVariables.playerStats[1]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Could not load your game progress: {0}", ex.Message), "Error loading game", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void AddMoney(double money)
        {
            //Add money and round to 0 decimals
            publicVariables.playerStats[1] = Convert.ToString(Math.Round(Convert.ToInt32(publicVariables.playerStats[1]) + money, 0));
            tblMoney.Text = string.Format("Money: {0}", publicVariables.playerStats[1].ToString());
        }

        public void InitializeStats()
        {
            //Set play started date if not already defined
            if (String.IsNullOrEmpty(publicVariables.playerStats[0]))
            {
                publicVariables.playerStats[0] = DateTime.Now.ToString();
            }

            //Set money
            if (string.IsNullOrEmpty(publicVariables.playerStats[1]))
            {
                publicVariables.playerStats[1] = "0";
            }

            //Set rodpower
            if (string.IsNullOrEmpty(publicVariables.playerStats[2]))
            {
                publicVariables.playerStats[2] = "1";
            }

            //Set baitpower
            if (string.IsNullOrEmpty(publicVariables.playerStats[3]))
            {
                publicVariables.playerStats[3] = "1";

            }
            //Set fish inventory slots
            if (string.IsNullOrEmpty(publicVariables.playerStats[4]))
            {
                publicVariables.playerStats[4] = "10";
            }


        }

        private void InitializeFish()
        {

            //Calculate raw chances for rarities based on rod power
            double commonChance = 0.6 - 0.006 * Convert.ToDouble(publicVariables.playerStats[2]);
            double trashChance = 0.35 - 0.005 * Convert.ToDouble(publicVariables.playerStats[2]);
            double rareChance = 0.05 + 0.003 * Convert.ToDouble(publicVariables.playerStats[2]);
            double superRareChance = 0.03 + 0.001 * Convert.ToDouble(publicVariables.playerStats[2]);
            double legendaryChance = 0.01 + 0.0005 * Convert.ToDouble(publicVariables.playerStats[2]);
            double specialChance = 0.005 + 0.0001 * Convert.ToDouble(publicVariables.playerStats[2]);

            //Calculate sum of the chances
            double totalChance = commonChance + trashChance + rareChance + superRareChance + legendaryChance + specialChance;

            //Normalize chances
            commonChance /= totalChance;
            trashChance /= totalChance;
            rareChance /= totalChance;
            superRareChance /= totalChance;
            legendaryChance /= totalChance;
            specialChance /= totalChance;

            //Create the fish types in a list
            fishTypes = new List<FishType>
            {
                  new FishType { Rarity = "Common", Chance = commonChance },
                  new FishType { Rarity = "Trash", Chance = trashChance },
                  new FishType { Rarity = "Rare", Chance = rareChance },
                  new FishType { Rarity = "Super rare", Chance = superRareChance },
                  new FishType { Rarity = "Legendary", Chance = legendaryChance },
                  new FishType { Rarity = "Special", Chance = specialChance }
            };
        }

        public bool InventoryFull()
        {
            //Check if the inventory list has as many entries as fishInventorySlots is big
            if (publicVariables.inventory.Count >= Convert.ToInt32(publicVariables.playerStats[4]))
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
                btnCast.Content = "Cast rod";
                imgBackground.Source = background_fisher_idle;
            }
            else if (state == "catchable")
            {
                //Rod is thrown out and there is a catchable fish
                btnCast.Content = "Catch fish";
                rodState = "catchable";
                imgBackground.Source = background_fisher_rodcasted;
            }
            else if (state == "casted")
            {
                //rod is thrown out but there is no fish
                rodState = "casted";
                imgBackground.Source = background_fisher_rodcasted;
                btnCast.Content = "Reel rod in";
            }
        }

        private void GenerateFish()
        {
            //Get the total chance for fish
            double totalChance = 0.0;
            foreach (var fishType in fishTypes)
            {
                totalChance += fishType.Chance;
            }

            //Generate a random number based on the total chance
            double randomValue = random.NextDouble() * totalChance;

            //Reset the accumulatedChance
            double accumulatedChance = 0;

            //Go through each fishtype and check if the random number is smaller than the accumulated chance
            //How does this work? In begins with the accumulated chance for common and compares that with the random number.
            //If the random number is not smaller, it goes to the next rarity and adds that rarity's chance to the accumulated chance.
            //Now, it once again compares the new accumulated chance to the random number, the chances now being higher that it hits.
            //That way it goes through each fishtype and the chances of getting that fishtype get higher for each fishtype.
            foreach (FishType fishType in fishTypes)
            {
                accumulatedChance += fishType.Chance;
                if (randomValue <= accumulatedChance)
                {
                    CatchFish(fishType.Rarity);
                    break;
                }
            }

        }

        private void CatchFish(string rarity)
        {
            wndInventory = new wndInventory() { Owner = this };

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
                weight = random.Next(1, (50 + Convert.ToInt32(publicVariables.playerStats[3])));
            }
            if (rarity == "Trash")
            {
                weight = random.Next(1, (10 + Convert.ToInt32(publicVariables.playerStats[3])));
            }
            else if (rarity == "Rare")
            {
                weight = random.Next(50, (100 + Convert.ToInt32(publicVariables.playerStats[3])));
            }
            else if (rarity == "Super rare")
            {
                weight = random.Next(100, (150 + Convert.ToInt32(publicVariables.playerStats[3])));
            }
            else if (rarity == "Legendary")
            {
                weight = random.Next(150, (200 + Convert.ToInt32(publicVariables.playerStats[3])));
            }
            else if (rarity == "Special")
            {
                weight = 69;
            }

            //Add fish to inventory
            publicVariables.inventory.Add(String.Format("{0};{1};{2};{3}", fishName, weight, rarity, DateTime.Now));
            wndInventory.RefreshInventory();

            //Show 'caught fish' notification
            tblCaughtFish.Text = String.Format("You caught a {0} ({1}) weighing {2} KG", fishName, rarity, weight);
            tblCaughtFish.Visibility = Visibility.Visible;
            caughtNotifcationTimer.Stop();
            caughtNotifcationTimer.Interval = new TimeSpan(0, 0, 5);
            caughtNotifcationTimer.Start();

        }
    }

    //Fish rarity, used to calculate the chances for getting specific fish types
    public class FishType
    {
        public string Rarity { get; set; }
        public double Chance { get; set; }
    }
}

