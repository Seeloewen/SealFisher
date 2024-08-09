namespace SealFisher
{
    public enum Rarity
    {
        Trash,
        Common,
        Rare,
        SuperRare,
        Legendary,
        Special
    }

    public class Fish
    {
        public string name;
        public int weight;
        public Rarity rarity;
        public string caughtDate;

        public Fish(string name, int weight, Rarity rarity, string caughtDate)
        {
            this.name = name;
            this.weight = weight;
            this.rarity = rarity;
            this.caughtDate = caughtDate;
        }
    }
}
