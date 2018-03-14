using StarsectorLTool.Src.Global;
using StarsectorLTool.Windows;
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
        //能检测到序列码-颜色
        private static SolidColorBrush TRUE_GAME_BRUSH = new SolidColorBrush(Color.FromRgb(185, 255, 174));
        //不能检测序列码-颜色
        private static SolidColorBrush FALSE_GAME_BRUSH = new SolidColorBrush(Color.FromRgb(255, 60, 60));
        private Process autoUpdateProcess;
        private int recordComboItemIndex;
        public string Version { get => "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(); }

        public MainWindow()
        {
            InitializeComponent();
            //获取根目录的starsector.exe
            Global.Starsector_exe_path = Directory.GetCurrentDirectory() + "\\" + "starsector.exe";
//            Console.WriteLine(GetType().Assembly.Location);

            //检查更新
            checkUpdate();
            //如果根目录不存在starsector.exe，关闭功能
            if (!File.Exists(Global.Starsector_exe_path))
            {
                lab_vmTips.Content = "-请将本应用放在远行星号根目录-";
                combo_xm.IsEnabled = false;
                btn_StartGame.IsEnabled = false;
                return;
            }
            string path = Directory.GetCurrentDirectory() + "\\" + "vmparams";
            if (File.Exists(Directory.GetCurrentDirectory() + "\\" + "vmparams"))
            {
                lab_vmTips.Content = "-已检测到vmparams:感谢使用远行星号L.Tool工具-";
                lab_vmTips.Foreground = Brushes.Green;
                Global.Vm_path = path;
                if (Global.InitRegisterData())
                {
                    lab_ModAmount.Content = Global.Ss_registerData.modAmount;
                    expa_PaiedGameCheck.IsEnabled = true;
                    if (!Global.Ss_registerData.isGameLegal)
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
                    lab_SerialKey.Content = Global.Ss_registerData.serialKey;
                }
                ReadVmparams();
            }

            btn_Apply.IsEnabled = false;


        }
        /// <summary>
        /// 开启游戏。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartGame(object sender, RoutedEventArgs e)
        {
            if (Global.Vm_path != null)
            {
                try
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo(Global.Starsector_exe_path);
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
        /// <summary>
        /// 点击“应用”按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                string all = File.ReadAllText(Global.Vm_path);
                var data = VmparamsData.ParseStringToData(all);
                Global.Vm_data = data;
                xms_tn.Content = data.xmsx;
                xmx_tn.Content = data.xmsx;

                combo_xm.Items.Clear();
                int totalIndex = 0;
                int currIndex = 0;
                for (int i = 1536; i <= 8192; i += 512)
                {
                    var item = new ComboBoxItem();
                    var content = i + "m";
                    if (data.xmsx.StartsWith(content))
                    {
                        currIndex = totalIndex;
                    }
                    item.Content = content + " ";
                    combo_xm.Items.Add(item);
                    totalIndex++;
                }
                combo_xm.SelectedIndex = currIndex;
                recordComboItemIndex = currIndex;
            }
            catch
            {
                Environment.Exit(0);
            }
        }
        /// <summary>
        /// 写入Vmparams。
        /// </summary>
        void WriteVmparams()
        {
            var data = Global.Vm_data;
            //获取当前选择的内存条目的Content
            ComboBoxItem sitem = combo_xm.SelectedItem as ComboBoxItem;
            //去除多余空格来替换对应条目
            string targetSetting = ((string)sitem.Content).Trim();
            try
            {
                File.WriteAllText(Global.Vm_path, VmparamsData.ReplaceXMSetting(data, targetSetting));
                MessageBox.Show("设置成功！", "恭喜", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch
            {
                MessageBox.Show("无法写入设置！请检查你的用户权限。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ReadVmparams();
        }


        /// <summary>
        /// 检查更新
        /// </summary>
        private void checkUpdate()
        {
            //先把内嵌的自动更新exe释放出来
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StarsectorLTool.sslt_autoUpdate.exe"))
            {
                Byte[] b = new Byte[stream.Length];
                stream.Read(b, 0, b.Length);
                string s = System.IO.Path.GetTempPath() + "sslt_autoUpdate.exe";
                if (File.Exists(s))
                    File.Delete(s);
                //创建文件并用文件流往里写入相关数据，之后释放文件流
                using (FileStream f = File.Create(s))
                {
                    f.Write(b, 0, b.Length);
                }
                string fileLocation = GetType().Assembly.Location.Replace(' ','+');
                string args = Assembly.GetExecutingAssembly().GetName().Version.ToString() + "|" + fileLocation;
                
                    //打开自动更新exe
                autoUpdateProcess = Process.Start(s, args);
            }
        }

        //protected override void OnClosed(EventArgs e)
        //{
        //    if(autoUpdateProcess!=null && !autoUpdateProcess.HasExited)
        //    {
        //        autoUpdateProcess.Kill();
        //    }
        //}

        private void Btn_Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("遗憾的是，精确预测内存使用情况是不可能的。这里只有一些粗略估计：" +
                "\n\n1.3G内存：在32位系统上你能分配的最大数值，加载一大堆势力mod将会导致崩溃，特别是在存档的时候。不要使用DynaSector（势力开局）。" +
                "\n\n2G(2048m)内存：对于大概两三个中等势力来说很不错，（比如说黑石船坞，SCY），但是如果你加更多的势力的话，很容易存档崩溃。如果你的系统内存只有4G，这个就是你的极限。" +
                "\n\n3G(3072m)内存：足够应付几个大型势力（比如说星际帝国，暗影）和一些中等势力。尽管如此，不要太过火。推荐给那些有着6G系统内存的人。" +
                "\n\n4G(4096m)内存：大多数mod组合需要在4G内存下运行；只有一些最疯狂mod组合会超过这个（一打或更多的势力，加上Nexerelin大乱斗和DynaSector势力开局）。推荐给那些有着8G系统内存的人。" +
                "\n\n6G(6144m)内存：就算你同时启用了所有mod，6G应该还是足够的。并不推荐这个配置，除非你有成吨的mod，并且你也有12G以上的系统内存。", "我该分配多少内存？");
        }
        /// <summary>
        /// 一旦更改设置判断应用按钮是否需要启用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combo_xm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btn_Apply.IsEnabled = combo_xm.SelectedIndex != recordComboItemIndex;
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        private void MenuItem_CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            checkUpdate();
        }
    }
}
