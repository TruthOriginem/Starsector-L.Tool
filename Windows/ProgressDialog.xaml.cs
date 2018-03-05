using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyProgressDialog
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public ProgressDialog()
        {
            InitializeComponent();
        }

        public void SetProgressValue(double value)
        {
            this.progressBar1.Value = value;
        }

        public void SetMessage(string mess)
        {
            this.infoBlock.Text = mess;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            throw new UserExitException();
        }

        public class UserExitException : Exception
        {
            public UserExitException()
                : base("用户退出!!")
            {
            }
        }
    }
}
