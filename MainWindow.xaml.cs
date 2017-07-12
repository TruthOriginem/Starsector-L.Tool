using StarsectorLTool.Src.Global;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + "starsector.exe"))
            {
                lab_vmTips.Content = "-请将本应用放在远行星号根目录-";
                Apply.IsEnabled = false;
                co_xms.IsEnabled = false;
                co_xmx.IsEnabled = false;
                return;
            }
            string path = Directory.GetCurrentDirectory() + "\\" + "vmparams";
            if (File.Exists(Directory.GetCurrentDirectory() + "\\" + "vmparams"))
            {
                lab_vmTips.Content = "-已检测到vmparams-";
                lab_vmTips.Foreground = Brushes.Green;
                Global.VM_PATH = path;
                ReadVmparams();
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            WriteVmparams();
        }
        /// <summary>
        /// 读取Vmparams。
        /// </summary>
        void ReadVmparams()
        {
            try
            {
                string all = File.ReadAllText(Global.VM_PATH);
                var data = ParseStringToData(all);
                Global.VM_DATA = data;
                xms_tn.Content = data.xms;
                xmx_tn.Content = data.xmx;

                co_xms.Items.Clear();
                co_xmx.Items.Clear();
                for (int i = 1536; i <= 8192; i += 512)
                {
                    var item = new ComboBoxItem();
                    item.Content = i + "m ";
                    co_xms.Items.Add(item);
                    item = new ComboBoxItem();
                    item.Content = i + "m ";
                    co_xmx.Items.Add(item);
                }
                co_xms.SelectedIndex = 0;
                co_xmx.SelectedIndex = 0;
            }
            catch
            {
                Environment.Exit(0);
            }
        }

        void WriteVmparams()
        {
            var data = Global.VM_DATA;
            ComboBoxItem sitem = co_xms.SelectedItem as ComboBoxItem;
            ComboBoxItem mitem = co_xmx.SelectedItem as ComboBoxItem;
            StringBuilder sb = new StringBuilder();
            sb.Append(data.beforeXms);
            sb.Append("-Xms");
            sb.Append(sitem.Content);
            sb.Append("-Xmx");
            sb.Append(mitem.Content);
            sb.Append(data.afterAndContainXss);
            try
            {
                File.WriteAllText(Global.VM_PATH, sb.ToString());
                MessageBox.Show("设置成功！", "恭喜", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch
            {
                MessageBox.Show("无法写入设置！请检查你的用户权限。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ReadVmparams();
        }

        VmparamsData ParseStringToData(string text)
        {
            VmparamsData data = new VmparamsData();
            var x = text.Split(new string[] { "-Xms" }, StringSplitOptions.RemoveEmptyEntries);
            data.beforeXms = x[0];
            x = x[1].Split(new string[] { "-Xmx" }, StringSplitOptions.RemoveEmptyEntries);
            data.xms = x[0];
            x = x[1].Split(new string[] { "-Xss" }, StringSplitOptions.RemoveEmptyEntries);
            data.xmx = x[0];
            data.afterAndContainXss = "-Xss" + x[1];
            return data;
        }


    }
}
