using StarsectorLTool.Src.Global;
using StarsectorLTool.Src.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StarsectorLTool.Windows
{
    /// <summary>
    /// ModManagerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ModManagerWindow : Window
    {
        private ModManager manager = ModManager.Instance;
        private Dictionary<string, ModConfigData> nameToConfigs = new Dictionary<string, ModConfigData>();

        public ModManagerWindow()
        {
            InitializeComponent();
            cb_Configs.DropDownClosed += Cb_Configs_DropDownClosed;
            ModManager.LoadAllBuiltInConfigPath();
            var configs = ModManager.CurrentConfigDatas;
            
            Thread.Sleep(50);
        }

        private void Cb_Configs_DropDownClosed(object sender, EventArgs e)
        {
            //tb_ConfigRMTextBloc.Text = 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
