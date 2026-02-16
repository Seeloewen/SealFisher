using System;
using System.Collections.Generic;

namespace SealFisher
{
	public static class Player
	{
        //Player stats
        public static List<Fish> inventory = new List<Fish>();
        public static List<Fish> archive = new List<Fish>();
        public static string startDate = DateTime.Now.ToString();
		public static int money = 0;
		public static int rodPower = 1;
		public static int baitPower = 1;
		public static int inventorySlots = 10;
        public static int archiveSlots = 10;
		public static int Location = 1;
        //Player Achievements
		public static bool Ach1 = false;
		public static bool Ach2 = false;
		public static bool Ach3 = false;
		public static bool Ach4 = false;
		public static bool Ach5 = false;
		public static bool Ach6 = false;
		public static bool Ach7 = false;
		public static bool Ach8 = false;
		public static bool Ach9 = false;
        public static void CalculateArchiveSlots()
        {
            if (Ach1 == true)
            {
                archiveSlots = archiveSlots + 1;
            }
            if (Ach2 == true) 
            {
                archiveSlots = archiveSlots + 1;
            }
            if (Ach3 == true)
            {
                archiveSlots = archiveSlots + 10;
            }
            if (Ach4 == true)
            {
                archiveSlots = archiveSlots + 5;
            }
            if (Ach5 == true)
            {
                archiveSlots = archiveSlots + 10;
            }
            if (Ach6 == true)
            {
                archiveSlots = archiveSlots + 20;
            }
            if (Ach7 == true)
            {
                archiveSlots = archiveSlots + 50;
            }
            if (Ach8 == true)
            {
                archiveSlots = archiveSlots + 5;
            }
            if (Ach9 == true)
            {
                archiveSlots = archiveSlots + 100;
            }
        }

        //Player fish Stats
        public static int FishStat;
        public static int TroutStat;
        public static int SalmonStat;
        public static int TunaStat;
        public static int PikeStat;
        public static int HerringStat;
        public static int ZanderStat;
        public static int EelStat;
        public static int CodStat;
        public static int CatfishStat;
        public static int CarpStat;
        public static int PlasticBottleStat;
        public static int MetalCanStat;
        public static int CigarreteStat;
        public static int BrokenPhoneStat;
        public static int PlasticBagStat;
        public static int CoelacanthStat;
        public static int ArapaimaStat;
        public static int MegamouthSharkStat;
        public static int MantaRayStat;
        public static int OlmStat;
        public static int BlacksnakeCuskEelStat;
        public static int HainanDogfishStat;
        public static int DenisonBarbStat;
        public static int HarlequinGhostPipefishStat;
        public static int DevilsHolePupfishStat;
        public static int SalamanderfishStat;
        public static int RedMouthedCichlidStat;
        public static int LongfinTriplefinStat;
        public static int GreatWhiteSharkStat;
        public static int ChineseSturgeonStat;
        public static int AnchorStat;
        public static int SealStat;
        public static int DiamondSwordStat;
        public static int NuclearWasteStat;
        public static int BombStat;
        public static int CommonFishStat;
        public static int TrashFishStat;
        public static int RareFishStat;
        public static int SuperRareFishStat;
        public static int LegendaryFishStat;
        public static int SpecialFishStat;
    }
}
