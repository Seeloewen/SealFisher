using System;
using System.Collections.Generic;

namespace SealFisher
{
	public static class Player
	{
        //Player stats
        public static List<Fish> inventory = new List<Fish>();
        public static string startDate = DateTime.Now.ToString();
		public static int money = 0;
		public static int rodPower = 1;
		public static int baitPower = 1;
		public static int inventorySlots = 10;
		public static int Location = 1;
	}
}
