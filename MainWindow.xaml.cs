using StarsectorLTool.Src.Global;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
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
        private static SolidColorBrush TRUE_GAME_BRUSH = new SolidColorBrush(Color.FromRgb(185, 255, 174));
        private static SolidColorBrush FALSE_GAME_BRUSH = new SolidColorBrush(Color.FromRgb(255, 60, 60));
        public MainWindow()
        {
            InitializeComponent();
            Global.STARSECTOR_EXE_PATH = Directory.GetCurrentDirectory() + "\\" + "starsector.exe";
            checkUpdate();
            if (!File.Exists(Global.STARSECTOR_EXE_PATH))
            {
                lab_vmTips.Content = "-请将本应用放在远行星号根目录-";
                Apply.IsEnabled = false;
                combo_xm.IsEnabled = false;
                btn_StartGame.IsEnabled = false;
                return;
            }
            string path = Directory.GetCurrentDirectory() + "\\" + "vmparams";
            if (File.Exists(Directory.GetCurrentDirectory() + "\\" + "vmparams"))
            {
                lab_vmTips.Content = "-已检测到vmparams:感谢使用远行星号L.Tool工具-";
                lab_vmTips.Foreground = Brushes.Green;
                Global.VM_PATH = path;
                if (Global.InitRegisterData())
                {
                    lab_ModAmount.Content = Global.SS_REGISTER_DATA.modAmount;
                    expa_PaiedGameCheck.IsEnabled = true;
                    if (!Global.SS_REGISTER_DATA.isGameLegal)
                    {
                        expa_PaiedGameCheck.Header = "你的游戏可能是盗版。";
                        expa_PaiedGameCheck.ToolTip = "你的注册表中并没有记录游戏序列码。";
                        expa_PaiedGameCheck.Foreground = FALSE_GAME_BRUSH;
                    }
                    else
                    {
                        expa_PaiedGameCheck.Header = "注册表中存在序列码。";
                        expa_PaiedGameCheck.ToolTip = "请以管理员身份运行本程序以获得你在该电脑上存储的的序列码。";
                        expa_PaiedGameCheck.Foreground = TRUE_GAME_BRUSH;
                    }
                    lab_SerialKey.Content = Global.SS_REGISTER_DATA.serialKey;
                }
                ReadVmparams();
            }



        }
        /// <summary>
        /// 开启游戏。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartGame(object sender, RoutedEventArgs e)
        {
            if (Global.VM_PATH != null)
            {
                try
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo(Global.STARSECTOR_EXE_PATH);
                    Process.Start(processStartInfo);
                    if (ckb_ExitAfterGame.IsChecked ?? true)
                    {

                        Environment.Exit(0);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
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

                combo_xm.Items.Clear();
                int totalIndex = 0;
                int currIndex = 0;
                for (int i = 1536; i <= 8192; i += 512)
                {
                    var item = new ComboBoxItem();
                    var content = i + "m";
                    if (data.xms.StartsWith(content))
                    {
                        currIndex = totalIndex;
                    }
                    item.Content = content + " ";
                    combo_xm.Items.Add(item);
                    totalIndex++;
                }
                combo_xm.SelectedIndex = currIndex;
            }
            catch
            {
                Environment.Exit(0);
            }
        }

        void WriteVmparams()
        {
            var data = Global.VM_DATA;
            ComboBoxItem sitem = combo_xm.SelectedItem as ComboBoxItem;
            StringBuilder sb = new StringBuilder();
            sb.Append(data.beforeXms);
            sb.Append("-Xms");
            sb.Append(sitem.Content);
            sb.Append("-Xmx");
            sb.Append(sitem.Content);
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
            data.rawVm = text;
            return data;
        }
        /// <summary>
        /// 检查更新
        /// </summary>
        private void checkUpdate()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StarsectorLTool.sslt_autoUpdate.exe"))
            {
                Byte[] b = new Byte[stream.Length];
                stream.Read(b, 0, b.Length);
                string s = System.IO.Path.GetTempPath() + "sslt_autoUpdate.exe";
                if (File.Exists(s))
                    File.Delete(s);
                using (FileStream f = File.Create(s))
                {
                    f.Write(b, 0, b.Length);
                }
                string args = "https://raw.githubusercontent.com/TruthOriginem/Starsector-L.Tool/master/bin/release/ " +
                    Assembly.GetExecutingAssembly().GetName().Version.ToString() + " " +
                    "https://raw.githubusercontent.com/TruthOriginem/Starsector-L.Tool/master/Properties/verison.xml " +
                   Directory.GetCurrentDirectory();
                Process.Start(s, args);
            }
        }

        private void Btn_Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("遗憾的是，精确预测内存使用情况是不可能的。这里只有一些粗略估计：" +
                "\n\n1.3G内存：在32位系统上你能分配的最大数值，加载一大堆势力mod将会导致崩溃，特别是在存档的时候。不要使用DynaSector（势力开局）。" +
                "\n\n2G(2048m)内存：对于大概两三个中等势力来说很不错，（比如说黑石船坞，SCY），但是如果你加更多的势力的话，很容易存档崩溃。如果你的系统内存只有4G，这个就是你的极限。" +
                "\n\n3G(3072m)内存：足够应付几个大型势力（比如说星际帝国，暗影）和一些中等势力。尽管如此，不要太过火。推荐给那些有着6G系统内存的人。" +
                "\n\n4G(4096m)内存：大多数mod组合需要在4G内存下运行；只有一些最疯狂mod组合会超过这个（一打或更多的势力，加上Nexerelin大乱斗和DynaSector势力开局）。推荐给那些有着8G系统内存的人。" +
                "\n\n6G(6144m)内存：就算你同时启用了所有mod，6G应该还是足够的。并不推荐这个配置，除非你有成吨的mod，并且你也有12G以上的系统内存。", "我该分配多少内存？");
        }

    }
}
