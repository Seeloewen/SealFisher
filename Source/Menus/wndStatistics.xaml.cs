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

namespace SealFisher
{
    /// <summary>
    /// Interaktionslogik für wndStatistics.xaml
    /// </summary>
    public partial class wndStatistics : Window
    {
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
            tblLegandaryFishStat.Text = $"{Player.LegendaryFishStat}";
            tblSpecialFishStat.Text = $"{Player.SpecialFishStat}";
        }
    }
}
