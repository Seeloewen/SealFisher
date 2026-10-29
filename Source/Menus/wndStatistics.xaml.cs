using System.Windows;

namespace SealFisher
{
    /// <summary>
    /// Interaktionslogik für wndStatistics.xaml
    /// </summary>
    public partial class wndStatistics : Window
    {

        //-- Constructor --//

        public wndStatistics()
        {
            InitializeComponent();
            RefreshStats();
        }
        public void RefreshStats()
        {
            //Refreshs the general stats
            tblMoneyStat.Text = $"{Player.money}";
            tblRodPowerStat.Text = $"{Player.rodPower}";
            tblBaitPowerStat.Text = $"{Player.baitPower}";
            tblInvSlotsStat.Text = $"{Player.inventorySlots}";
            tblLocationStat.Text = $"{Player.Location}";
            //Refreshs the general fish stats
            tblFishStat.Text = $"{Player.FishStat}";
            tblTrashFishStat.Text = $"{Player.TrashFishStat}";
            tblCommonFishStat.Text = $"{Player.CommonFishStat}";
            tblRareFishStat.Text = $"{Player.RareFishStat}";
            tblSuperRareFishStat.Text = $"{Player.SuperRareFishStat}";
            tblLegendaryFishStat.Text = $"{Player.LegendaryFishStat}";
            tblSpecialFishStat.Text = $"{Player.SpecialFishStat}";
        }
    }
}
