using StarsectorLTool.Src.Global;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StarsectorLTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists(Global.TEST_VM_PATH + "\\" + "vmparams"))
            {
                lab_vmTips.Content = "-已检测到vmparams-";
                lab_vmTips.Foreground = Brushes.Green;
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());

        }
    }
}
