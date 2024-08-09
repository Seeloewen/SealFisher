using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Environment;

namespace SealFisher
{
    public enum RodState
    {
        idle,
        casted,
        catchable
    }

    public partial class wndGame : Window
    {

        //-- Variables --//

        //General - Using Windows Forms Timers here as they have a slightly better acurracy on low tickrates
        private System.Windows.Forms.Timer fishTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer caughtNotifcationTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer fishWarningDisappearTimer = new System.Windows.Forms.Timer();
        private Random random = new Random();
        private List<FishType> fishTypes;
        private RodState rodState = RodState.idle;
        private int fishCatchTime;

        //Images
        private static Uri uriBackground = new Uri("pack://application:,,,/SealFisher;component/Resources/imgBackground.png");
        private BitmapImage imgBackground = new BitmapImage(uriBackground);
        private static Uri uriBackgroundCasted = new Uri("pack://application:,,,/SealFisher;component/Resources/imgBackgroundCasted.png");
        private BitmapImage imgBackgroundCasted = new BitmapImage(uriBackgroundCasted);
        private static Uri uriBackgroundIdle = new Uri("pack://application:,,,/SealFisher;component/Resources/imgBackgroundIdle.png");
        private BitmapImage imgBackgroundIdle = new BitmapImage(uriBackgroundIdle);

        //Menus
        private wndInventory wndInventory;
        private wndShop wndShop;

        //Directories and files
        static string appData = GetFolderPath(SpecialFolder.ApplicationData);
        static string gameDirectory = $"{appData}/SealFisher";
        string statsSaveFile = $"{gameDirectory}/statistics.txt";
        string inventorySaveFile = $"{gameDirectory}/inventory.txt";

        //-- Constructor --//

        public wndGame()
        {
            InitializeComponent();

            //Setup timers
            fishTimer.Tick += new EventHandler(fishTimer_Tick);
            fishWarningDisappearTimer.Tick += new EventHandler(fishWarningDisappearTimer_Tick);
            caughtNotifcationTimer.Tick += new EventHandler(caughtNotificationTimer_Tick);
            fishWarningDisappearTimer.Interval = 3000;
        }

        //-- Event Handlers --//

        private void wndGame_Loaded(object sender, RoutedEventArgs e)
        {
            CreateGameDirectory();
            LoadGame();
            InitializeFish();
        }

        private void fishTimer_Tick(object sender, EventArgs e)
        {
            //Make fish catchable
            SetRodState(RodState.catchable);
            tblFishWarning.Visibility = Visibility.Visible;
            fishTimer.Stop();
            fishWarningDisappearTimer.Start();
        }
        private void fishWarningDisappearTimer_Tick(object sender, EventArgs e)
        {
            //Make fish disappear when not caught
            SetRodState(RodState.casted);
            tblFishWarning.Visibility = Visibility.Hidden;
            fishWarningDisappearTimer.Stop();
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
            //Cast your rod
            CastRod();
        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            //Open inventory window
            wndInventory = new wndInventory() { Owner = this };
            if (wndInventory.IsOpen == false)
            {
                wndInventory.Show();
            }
        }

        private void btnDebugGiveFish(object sender, RoutedEventArgs e)
        {
            //Cheat for debugging
            for (int i = 0; i < 10; i++)
            {
                GenerateFish();
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
            wndShop.ShowDialog();
        }

        //-- Custom Methods --//

        public void CastRod()
        {
            //Check rodstate, then execute actions depending on rodstate
            switch (rodState)
            {
                case RodState.idle:
                    //Check if inventory is full
                    if (!IsInventoryFull())
                    {
                        SetRodState(RodState.casted);

                        //Start timer for fish spawn
                        fishCatchTime = random.Next(1, 12);
                        fishTimer.Interval = fishCatchTime * 1000;
                        fishTimer.Start();
                    }
                    else
                    {
                        MessageBox.Show("Your inventory is full. Please sell some fish or upgrade your storage.", "Inventory full", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    break;
                case RodState.casted:
                    //Retreat rod without getting a fish
                    SetRodState(RodState.idle);
                    fishTimer.Stop();
                    break;
                case RodState.catchable:
                    //Retreat rod and get fish
                    SetRodState(RodState.idle);
                    fishTimer.Stop();
                    fishWarningDisappearTimer.Stop();
                    tblFishWarning.Visibility = Visibility.Hidden;
                    GenerateFish();
                    break;
            }
        }

        public void CreateGameDirectory()
        {
            //Checks if all necessary directories are there and creates them if not
            try
            {
                if (!Directory.Exists(gameDirectory))
                {
                    Directory.CreateDirectory(gameDirectory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not create game directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool GameDirectoriesExist()
        {
            //Checks if all directories, that the game needs, exist
            return Directory.Exists(appData) && Directory.Exists(gameDirectory);
        }

        public void SaveGame(bool showConfirmation)
        {
            //Check if necessary directories for saving the files exist
            if (GameDirectoriesExist())
            {
                try
                {
                    //Save inventory
                    using (StreamWriter file = new StreamWriter(inventorySaveFile))
                    {
                        foreach (Fish fish in Player.inventory)
                        {
                            file.WriteLine($"{fish.name};{fish.weight};{fish.rarity};{fish.caughtDate}");
                        }
                    }

                    //Save user progress/stats
                    using (StreamWriter file = new StreamWriter(statsSaveFile))
                    {
                        file.WriteLine(Player.money);
                        file.WriteLine(Player.rodPower);
                        file.WriteLine(Player.baitPower);
                        file.WriteLine(Player.inventorySlots);
                        file.WriteLine(Player.startDate);
                    }

                    //Show save confirmation if enabled
                    if (showConfirmation)
                    {
                        MessageBox.Show("Your inventory and player stats have been saved successfully.", "Saved game", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not save your game: {ex.Message}", "Error when saving game", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Could not save your game because of issues with your game directories. Try restarting the app.\nNote that you will loose all unsaved progress.", "Error while saving game", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void LoadGame()
        {
            if (GameDirectoriesExist())
            {
                try
                {
                    //Load inventory
                    var loadedFish = File.ReadLines(inventorySaveFile);
                    foreach (string fish in loadedFish)
                    {
                        string[] split = fish.Split(';');

                        Player.inventory.Add(new Fish(split[0], Convert.ToInt32(split[1]), GetRarityFromString(split[2]), split[3]));
                    }

                    //Load player stats
                    string[] loadedStats = File.ReadAllLines(statsSaveFile);
                    Player.money = int.Parse(loadedStats[0]);
                    Player.rodPower = int.Parse(loadedStats[1]);
                    Player.baitPower = int.Parse(loadedStats[2]);
                    Player.inventorySlots = int.Parse(loadedStats[3]);
                    Player.startDate = loadedStats[4];

                    //Update UI elements
                    SetMoney(Player.money);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not load your game progress: {ex.Message}", "Error loading game", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public Rarity GetRarityFromString(string rarity)
        {
            switch (rarity)
            {
                case "Trash":
                    return Rarity.Trash;
                case "Common":
                    return Rarity.Common;
                case "Rare":
                    return Rarity.Rare;
                case "SuperRare":
                    return Rarity.SuperRare;
                case "Legendary":
                    return Rarity.Legendary;
                case "Special":
                    return Rarity.Special;
                default:
                    return Rarity.Common;
            }
        }

        public void AddMoney(double money)
        {
            //Add money to player
            Player.money += (int)money;
            tblMoney.Text = $"Money: {Player.money}";
        }

        public void RemoveMoney(double money)
        {
            //Remove money from player
            Player.money -= (int)money;
            tblMoney.Text = $"Money: {Player.money}";
        }

        public void SetMoney(double money)
        {
            //Add money and round to 0 decimals
            Player.money = (int)money;
            tblMoney.Text = $"Money: {Player.money}";
        }

        private void InitializeFish()
        {

            //Calculate raw chances for rarities based on rod power
            double commonChance = 0.6 - 0.006 * Player.rodPower;
            double trashChance = 0.35 - 0.005 * Player.rodPower;
            double rareChance = 0.05 + 0.003 * Player.rodPower;
            double superRareChance = 0.03 + 0.001 * Player.rodPower;
            double legendaryChance = 0.01 + 0.0005 * Player.rodPower;
            double specialChance = 0.005 + 0.0001 * Player.rodPower;

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
                  new FishType { Rarity = Rarity.Common, Chance = commonChance },
                  new FishType { Rarity = Rarity.Trash, Chance = trashChance },
                  new FishType { Rarity = Rarity.Rare, Chance = rareChance },
                  new FishType { Rarity = Rarity.SuperRare, Chance = superRareChance },
                  new FishType { Rarity = Rarity.Legendary, Chance = legendaryChance },
                  new FishType { Rarity = Rarity.Legendary, Chance = specialChance }
            };
        }

        public bool IsInventoryFull()
        {
            //Check if the inventory list has as many entries as the player inventory
            return Player.inventory.Count >= Player.inventorySlots;
        }

        private void SetRodState(RodState state)
        {
            if (state == RodState.idle)
            {
                //Rod is not thrown out
                btnCast.Content = "Cast rod";
                rodState = RodState.idle;
                Background = new ImageBrush() { ImageSource = GetImageSource(uriBackgroundIdle) };
            }
            else if (state == RodState.catchable)
            {
                //Rod is thrown out and there is a catchable fish
                btnCast.Content = "Catch fish";
                rodState = RodState.catchable;
                Background = new ImageBrush() { ImageSource = GetImageSource(uriBackgroundCasted) };
            }
            else if (state == RodState.casted)
            {
                //rod is thrown out but there is no fish
                btnCast.Content = "Reel rod in";
                rodState = RodState.casted;
                Background = new ImageBrush() { ImageSource = GetImageSource(uriBackgroundCasted)} ;
            }
        }

        private void GenerateFish()
        {
            //Get the total chance for fish
            double totalChance = 0.0;
            foreach (FishType fishType in fishTypes)
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

        private void CatchFish(Rarity rarity)
        {
            wndInventory = new wndInventory() { Owner = this };

            //Common fish
            int fishNum = 0;
            string fishName = "";

            switch (rarity)
            {
                case Rarity.Common:
                    fishNum = random.Next(1, 11);

                    switch (fishNum)
                    {
                        case 1:
                            fishName = "Trout";
                            break;
                        case 2:
                            fishName = "Salmon";
                            break;
                        case 3:
                            fishName = "Tuna";
                            break;
                        case 4:
                            fishName = "Pike";
                            break;
                        case 5:
                            fishName = "Herring";
                            break;
                        case 6:
                            fishName = "Zander";
                            break;
                        case 7:
                            fishName = "Eel";
                            break;
                        case 8:
                            fishName = "Cod";
                            break;
                        case 9:
                            fishName = "Catfish";
                            break;
                        case 10:
                            fishName = "Carp";
                            break;
                    }
                    break;
                case Rarity.Trash:
                    fishNum = random.Next(1, 6);

                    switch (fishNum)
                    {
                        case 1:
                            fishName = "Plastic bottle";
                            break;
                        case 2:
                            fishName = "Metal can";
                            break;
                        case 3:
                            fishName = "Cigarrete";
                            break;
                        case 4:
                            fishName = "Broken phone";
                            break;
                        case 5:
                            fishName = "Plastic bag";
                            break;
                    }
                    break;
                case Rarity.Rare:
                    fishNum = random.Next(1, 6);

                    switch(fishNum)
                    {
                        case 1:
                            fishName = "Coelacanth";
                            break;
                        case 2:
                            fishName = "Arapaima";
                            break;
                        case 3:
                            fishName = "Megamouth shark";
                            break;
                        case 4:
                            fishName = "Manta ray";
                            break;
                        case 5:
                            fishName = "Olm";
                            break;
                    }
                    break;
                case Rarity.SuperRare:
                    fishNum = random.Next(1, 6);

                    switch(fishNum)
                    {
                        case 1:
                            fishName = "Blacksnake cusk-eel";
                            break;
                        case 2:
                            fishName = "Hainan dogfish";
                            break;
                        case 3:
                            fishName = "Denison barb";
                            break;
                        case 4:
                            fishName = "Harlequin ghost pipefish";
                            break;
                        case 5:
                            fishName = "Devil's Hole pupfish";
                            break;
                    }
                    break;
                case Rarity.Legendary:
                    fishNum = random.Next(1, 6);

                    switch(fishNum)
                    {
                        case 1:
                            fishName = "Salamanderfish";
                            break;
                        case 2:
                            fishName = "Red-mouthed cichlid";
                            break;
                        case 3:
                            fishName = "Longfin triplefin";
                            break;
                        case 4:
                            fishName = "Great White Shark";
                            break;
                        case 5:
                            fishName = "Chinese sturgeon";
                            break;
                    }
                    break;
                case Rarity.Special:
                    fishNum = random.Next(1, 6);

                    switch(fishNum)
                    {
                        case 1:
                            fishName = "Anchor";
                            break;
                        case 2:
                            fishName = "Seal";
                            break;
                        case 3:
                            fishName = "Diamond Sword";
                            break;
                        case 4:
                            fishName = "Nuclear Waste";
                            break;
                        case 5:
                            fishName = "Bomb";
                            break;
                    }
                    break;
            }

            //Get random weight
            int weight = random.Next(0, 61);

            //Add fish to inventory
            Player.inventory.Add(new Fish(fishName, weight, rarity, DateTime.Now.ToString()));
            wndInventory.RefreshInventory();

            //Show 'caught fish' notification
            tblCaughtFish.Text = $"You caught a {fishName} ({rarity}) weighing {weight} KG";
            tblCaughtFish.Visibility = Visibility.Visible;
            caughtNotifcationTimer.Stop();
            caughtNotifcationTimer.Interval = 5000;
            caughtNotifcationTimer.Start();

        }
        public static ImageSource GetImageSource(Uri uri)
        {
            //Get an image from an imagesource from a uri
            return BitmapFrame.Create(uri);
        }
    }

    //Fish rarity, used to calculate the chances for getting specific fish types
    public struct FishType
    {
        public Rarity Rarity { get; set; }
        public double Chance { get; set; }
    }
}

