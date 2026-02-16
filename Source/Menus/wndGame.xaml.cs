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
    public enum Location
    {
        Location1,
        Location2
    }
    public enum BuildingProgress
    {
        none,
        start,
        middle,
        Finished
    }

    public partial class wndGame : Window
    {

        //-- Variables --//

        //General - Using Windows Forms Timers here as they have a slightly better acurracy on low tickrates
        private System.Windows.Forms.Timer fishTimer = new System.Windows.Forms.Timer();
        public System.Windows.Forms.Timer caughtNotifcationTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer fishWarningDisappearTimer = new System.Windows.Forms.Timer();
        private Random random = new Random();
        private List<FishType> fishTypes;
        private RodState rodState = RodState.idle;
        private BuildingProgress buildingProgress = BuildingProgress.none;
        private Location location = Location.Location1;
        private double fishCatchTime;
        public int Locationmultiplier = 1;

        //Images
        private static Uri uriBackground = new Uri("pack://application:,,,/SealFisher;component/Resources/imgBackground.png");
        private BitmapImage imgBackground = new BitmapImage(uriBackground);
        private static Uri uriBackgroundCasted = new Uri("pack://application:,,,/SealFisher;component/Resources/imgBackgroundCasted.png");
        private BitmapImage imgBackgroundCasted = new BitmapImage(uriBackgroundCasted);
        private static Uri uriBackgroundIdle = new Uri("pack://application:,,,/SealFisher;component/Resources/imgBackgroundIdle.png");
        private BitmapImage imgBackgroundIdle = new BitmapImage(uriBackgroundIdle);
        private static Uri uriBackground1Boat = new Uri("pack://application:,,,/SealFisher;component/Resources/background1 boat.png");
        private BitmapImage imgBackground1Boat = new BitmapImage(uriBackground1Boat);
        private static Uri uriBackground1Boat2 = new Uri("pack://application:,,,/SealFisher;component/Resources/background1 boat2.png");
        private BitmapImage imgBackground1Boat2 = new BitmapImage(uriBackground1Boat2);
        private static Uri uriBackground1BoatFinished = new Uri("pack://application:,,,/SealFisher;component/Resources/background1 boat Finish.png");
        private BitmapImage imgBackgroundIdle1BoatFinished = new BitmapImage(uriBackground1BoatFinished);
        private static Uri uriBackground2Idle = new Uri("pack://application:,,,/SealFisher;component/Resources/Background 2 idle.png");
        private BitmapImage imgBackground2Idle = new BitmapImage(uriBackground2Idle);
        private static Uri uriBackground2Casted = new Uri("pack://application:,,,/SealFisher;component/Resources/Background 2 rodcasted.png");
        private BitmapImage imgBackground2Casted = new BitmapImage(uriBackground2Casted);
        //Menus
        private wndInventory wndInventory;
        private wndShop wndShop;
        private wndAchievements wndAchievements;
        private wndStatistics wndStatistics;

        //Directories and files
        static string appData = GetFolderPath(SpecialFolder.ApplicationData);
        static string gameDirectory = $"{appData}/SealFisher";
        string statsSaveFile = $"{gameDirectory}/statistics.txt";
        string inventorySaveFile = $"{gameDirectory}/inventory.txt";

        //-- Constructor --//

        public wndGame()
        {
            InitializeComponent();

            Game.Run();

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
            Buildable();
            SetLocationMultiplier();
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
        private void btnAchievements_Click(object sender, RoutedEventArgs e)
        {
            wndAchievements = new wndAchievements() { Owner = this };
            wndAchievements.ShowDialog();
        }
        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            wndStatistics = new wndStatistics() { Owner = this };
            wndStatistics.ShowDialog();
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
                        GenerateFishigTime();
                        fishTimer.Interval = (int)(fishCatchTime * 1000);
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
                    UpdateFishAchievements();
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
                        file.WriteLine(Player.Location);
                        file.WriteLine(buildingProgress);
                        file.WriteLine(Player.Ach1);
                        file.WriteLine(Player.Ach2);
                        file.WriteLine(Player.Ach3);
                        file.WriteLine(Player.Ach4);
                        file.WriteLine(Player.Ach5);
                        file.WriteLine(Player.Ach6);
                        file.WriteLine(Player.Ach7);
                        file.WriteLine(Player.Ach8);
                        file.WriteLine(Player.Ach9);
                        file.WriteLine(Player.FishStat);
                        file.WriteLine(Player.TrashFishStat);
                        file.WriteLine(Player.CommonFishStat);
                        file.WriteLine(Player.RareFishStat);
                        file.WriteLine(Player.SuperRareFishStat);
                        file.WriteLine(Player.LegendaryFishStat);
                        file.WriteLine(Player.SpecialFishStat);
                        file.WriteLine(Player.SealStat);
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
                    Player.Location = int.Parse(loadedStats[5]);
                    string enumString = loadedStats[6].ToString();
                    Player.Ach1 = bool.Parse(loadedStats[7]);
                    Player.Ach2 = bool.Parse(loadedStats[8]);
                    Player.Ach3 = bool.Parse(loadedStats[9]);
                    Player.Ach4 = bool.Parse(loadedStats[10]);
                    Player.Ach5 = bool.Parse(loadedStats[11]);
                    Player.Ach6 = bool.Parse(loadedStats[12]);
                    Player.Ach7 = bool.Parse(loadedStats[13]);
                    Player.Ach8 = bool.Parse(loadedStats[14]);
                    Player.Ach9 = bool.Parse(loadedStats[15]);
                    Player.FishStat = int.Parse(loadedStats[16]);
                    Player.TrashFishStat = int.Parse(loadedStats[17]);
                    Player.CommonFishStat = int.Parse(loadedStats[18]);
                    Player.RareFishStat = int.Parse(loadedStats[19]);
                    Player.SuperRareFishStat = int.Parse(loadedStats[20]);
                    Player.LegendaryFishStat = int.Parse(loadedStats[21]);
                    Player.SpecialFishStat = int.Parse(loadedStats[22]);
                    Player.SealStat = int.Parse(loadedStats[23]);
                    GetBuildingProgress(enumString);
                    if (buildingProgress == BuildingProgress.Finished) Player.Ach8 = true;

                    //Update UI elements
                    SetMoney(Player.money);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not load your game progress: {ex.Message}", "Error loading game", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            if (Player.Location == 2)
            {
                btnSwitchLocationForward.Visibility = Visibility.Visible;
            }
        }
        public BuildingProgress GetBuildingProgress(string enumString)
        {
            switch (enumString)
            {
                //updates the buildingprogress
                case "none":
                    btnSwitchLocationForward.Content = "Build";
                    return buildingProgress = BuildingProgress.none;
                case "start":
                    btnSwitchLocationForward.Content = "Build";
                    return buildingProgress = BuildingProgress.start;
                case "middle":
                    btnSwitchLocationForward.Content = "Build";
                    return buildingProgress = BuildingProgress.middle;
                case "Finished":
                    btnSwitchLocationForward.Content = "Location 2";
                    return buildingProgress = BuildingProgress.Finished;
                default:
                    return buildingProgress = BuildingProgress.none;
            }
        }
        public void UpdateFishAchievements()
        {
            //Updates Achievements related to fish
            if (Player.FishStat >= 1) Player.Ach1 = true;
            if (Player.SealStat >= 1) Player.Ach3 = true;
            if (Player.FishStat >= 100) Player.Ach4 = true;
            if (Player.FishStat >= 1000) Player.Ach5 = true;
            if (Player.FishStat >= 10000) Player.Ach6 = true;
            if (Player.FishStat >= 100000) Player.Ach7 = true;
            if (Player.SealStat >= 100) Player.Ach9 = true;
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
            double commonChance = 0.6 - 0.006 * Player.rodPower * Locationmultiplier;
            double trashChance = 0.35 - 0.005 * Player.rodPower * Locationmultiplier;
            double rareChance = 0.05 + 0.003 * Player.rodPower * Locationmultiplier;
            double superRareChance = 0.03 + 0.001 * Player.rodPower * Locationmultiplier;
            double legendaryChance = 0.01 + 0.0005 * Player.rodPower * Locationmultiplier;
            double specialChance = 0.005 + 0.0001 * Player.rodPower * Locationmultiplier;

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
                if (location == Location.Location1)
                {
                    Background = new ImageBrush() { ImageSource = GetImageSource(uriBackgroundIdle) };
                }
                else if (location == Location.Location2)
                {
                    Background = new ImageBrush() { ImageSource = GetImageSource(uriBackground2Idle) };
                }

            }
            else if (state == RodState.catchable)
            {
                //Rod is thrown out and there is a catchable fish
                btnCast.Content = "Catch fish";
                rodState = RodState.catchable;
                if (location == Location.Location1)
                {
                    Background = new ImageBrush() { ImageSource = GetImageSource(uriBackgroundCasted) };
                }
                else if (location == Location.Location2)
                {
                    Background = new ImageBrush() { ImageSource = GetImageSource(uriBackground2Casted) };
                }
            }
            else if (state == RodState.casted)
            {
                //rod is thrown out but there is no fish
                btnCast.Content = "Reel rod in";
                rodState = RodState.casted;
                if (location == Location.Location1)
                {
                    Background = new ImageBrush() { ImageSource = GetImageSource(uriBackgroundCasted) };
                }
                else if (location == Location.Location2)
                {
                    Background = new ImageBrush() { ImageSource = GetImageSource(uriBackground2Casted) };
                }
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
            Player.FishStat++;

            switch (rarity)
            {
                case Rarity.Common:
                    fishNum = random.Next(1, 11);
                    Player.CommonFishStat++;

                    switch (fishNum)
                    {
                        case 1:
                            fishName = "Trout";
                            Player.TroutStat++;
                            break;
                        case 2:
                            fishName = "Salmon";
                            Player.SalmonStat++;
                            break;
                        case 3:
                            fishName = "Tuna";
                            Player.TunaStat++;
                            break;
                        case 4:
                            fishName = "Pike";
                            Player.PikeStat++;
                            break;
                        case 5:
                            fishName = "Herring";
                            Player.HerringStat++;
                            break;
                        case 6:
                            fishName = "Zander";
                            Player.ZanderStat++;
                            break;
                        case 7:
                            fishName = "Eel";
                            Player.EelStat++;
                            break;
                        case 8:
                            fishName = "Cod";
                            Player.CodStat++;
                            break;
                        case 9:
                            fishName = "Catfish";
                            Player.CatfishStat++;
                            break;
                        case 10:
                            fishName = "Carp";
                            Player.CarpStat++;
                            break;
                    }
                    break;
                case Rarity.Trash:
                    fishNum = random.Next(1, 6);
                    Player.TrashFishStat++;

                    switch (fishNum)
                    {
                        case 1:
                            fishName = "Plastic bottle";
                            Player.PlasticBottleStat++;
                            break;
                        case 2:
                            fishName = "Metal can";
                            Player.MetalCanStat++;
                            break;
                        case 3:
                            fishName = "Cigarrete";
                            Player.CigarreteStat++;
                            break;
                        case 4:
                            fishName = "Broken phone";
                            Player.BrokenPhoneStat++;
                            break;
                        case 5:
                            fishName = "Plastic bag";
                            Player.PlasticBagStat++;
                            break;
                    }
                    break;
                case Rarity.Rare:
                    fishNum = random.Next(1, 6);
                    Player.RareFishStat++;

                    switch (fishNum)
                    {
                        case 1:
                            fishName = "Coelacanth";
                            Player.CoelacanthStat++;
                            break;
                        case 2:
                            fishName = "Arapaima";
                            Player.ArapaimaStat++;
                            break;
                        case 3:
                            fishName = "Megamouth shark";
                            Player.MegamouthSharkStat++;
                            break;
                        case 4:
                            fishName = "Manta ray";
                            Player.MantaRayStat++;
                            break;
                        case 5:
                            fishName = "Olm";
                            Player.OlmStat++;
                            break;
                    }
                    break;
                case Rarity.SuperRare:
                    fishNum = random.Next(1, 6);
                    Player.SuperRareFishStat++;

                    switch (fishNum)
                    {
                        case 1:
                            fishName = "Blacksnake cusk-eel";
                            Player.BlacksnakeCuskEelStat++;
                            break;
                        case 2:
                            fishName = "Hainan dogfish";
                            Player.HainanDogfishStat++;
                            break;
                        case 3:
                            fishName = "Denison barb";
                            Player.DenisonBarbStat++;
                            break;
                        case 4:
                            fishName = "Harlequin ghost pipefish";
                            Player.HarlequinGhostPipefishStat++;
                            break;
                        case 5:
                            fishName = "Devil's Hole pupfish";
                            Player.DevilsHolePupfishStat++;
                            break;
                    }
                    break;
                case Rarity.Legendary:
                    fishNum = random.Next(1, 6);
                    Player.LegendaryFishStat++;

                    switch (fishNum)
                    {
                        case 1:
                            fishName = "Salamanderfish";
                            Player.SalamanderfishStat++;
                            break;
                        case 2:
                            fishName = "Red-mouthed cichlid";
                            Player.RedMouthedCichlidStat++;
                            break;
                        case 3:
                            fishName = "Longfin triplefin";
                            Player.LongfinTriplefinStat++;
                            break;
                        case 4:
                            fishName = "Great White Shark";
                            Player.GreatWhiteSharkStat++;
                            break;
                        case 5:
                            fishName = "Chinese sturgeon";
                            Player.ChineseSturgeonStat++;
                            break;
                    }
                    break;
                case Rarity.Special:
                    fishNum = random.Next(1, 6);
                    Player.SpecialFishStat++;

                    switch (fishNum)
                    {
                        case 1:
                            fishName = "Anchor";
                            Player.AnchorStat++;
                            break;
                        case 2:
                            fishName = "Seal";
                            Player.SealStat++;
                            break;
                        case 3:
                            fishName = "Diamond Sword";
                            Player.DiamondSwordStat++;
                            break;
                        case 4:
                            fishName = "Nuclear Waste";
                            Player.NuclearWasteStat++;
                            break;
                        case 5:
                            fishName = "Bomb";
                            Player.BombStat++;
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
        public void GenerateFishigTime()
        {
            int BaseValue = 13;
            fishCatchTime = BaseValue / Player.baitPower * Locationmultiplier + random.Next(0, 4);
        }

        private void btnSwitchLocationForward_Click(object sender, RoutedEventArgs e)
        {
            if (buildingProgress == BuildingProgress.none)
            {
                Background = new ImageBrush() { ImageSource = GetImageSource(uriBackground1Boat) };
                buildingProgress = BuildingProgress.start;
            }
            else if (buildingProgress == BuildingProgress.start)
            {
                Background = new ImageBrush() { ImageSource = GetImageSource(uriBackground1Boat2) };
                buildingProgress = BuildingProgress.middle;
            }
            else if (buildingProgress == BuildingProgress.middle)
            {
                Background = new ImageBrush() { ImageSource = GetImageSource(uriBackground1BoatFinished) };
                buildingProgress = BuildingProgress.Finished;
                btnSwitchLocationForward.Content = "Location 2";
                if (!Player.Ach8) Player.Ach8 = true;
            }
            else if (buildingProgress == BuildingProgress.Finished && location == Location.Location1)
            {
                Background = new ImageBrush() { ImageSource = GetImageSource(uriBackground2Idle) };
                location = Location.Location2;
                btnSwitchLocationForward.Visibility = Visibility.Hidden;
                btnSwitchLocationBackward.Visibility = Visibility.Visible;
                btnCast.Visibility = Visibility.Visible;
                StopAllTimers();
                SetRodState(RodState.idle);
                SetLocationMultiplier();
            }
        }

        private void btnSwitchLocationBackward_Click(object sender, RoutedEventArgs e)
        {
            if (location == Location.Location2)
            {
                Background = new ImageBrush() { ImageSource = GetImageSource(uriBackgroundIdle) };
                location = Location.Location1;
                btnSwitchLocationBackward.Visibility = Visibility.Hidden;
                btnSwitchLocationForward.Visibility = Visibility.Visible;
                btnCast.Visibility = Visibility.Visible;
                StopAllTimers();
                SetRodState(RodState.idle);
                SetLocationMultiplier();
            }
        }
        public void StopAllTimers()
        {
            fishTimer.Stop();
            fishWarningDisappearTimer.Stop();
            caughtNotifcationTimer.Stop();
        }
        public void Buildable()
        {
            if (Player.Location == 2 && (buildingProgress == BuildingProgress.none || buildingProgress == BuildingProgress.start || buildingProgress == BuildingProgress.middle))
            {
                btnCast.Visibility = Visibility.Hidden;
            }
        }
        public void SetLocationMultiplier()
        {
            if (location == Location.Location1)
            {
                Locationmultiplier = 1;
            }
            else if (location == Location.Location2)
            {
                Locationmultiplier = 2;
            }
        }


    }

    //Fish rarity, used to calculate the chances for getting specific fish types
    public struct FishType
    {
        public Rarity Rarity { get; set; }
        public double Chance { get; set; }
    }
}

