using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealFisher
{
	public static class publicVariables
	{
		public static List<string> inventory = new List<string>();
		/* Inventory is structured like this:
		 * [0] Fish name
		 * [1] Fish weight
		 * [2] Fish rarity
		 * [3] Fish caught date
		 */

		public static string[] playerStats = new string[5];
		/* Player stats is structured like this:
		 * [0] Started playing date
		 * [1] Money
		 * [2] Rodpower
		 * [3] Baitpower
		 * [4] Fish Inventory Slots
		 */
	}
}
