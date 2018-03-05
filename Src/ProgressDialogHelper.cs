using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyProgressDialog
{
    public class ProgressDialogHelper
    {
        public ProgressDialog ProDialog;
        public Thread Worker;

        public void CloseProgressDialog()
        {
            ProDialog.Dispatcher.BeginInvoke(new Action(() =>
            {
                ProDialog.Close();
            }));
        }

        public void WorkerThreadAbort()
        {
            if (Worker.IsAlive)
                Worker.Abort();
        }

        public void SetValueAndMessage(double value, string mess)
        {
            ProDialog.Dispatcher.BeginInvoke(new Action(() =>
            {
                ProDialog.SetProgressValue(value);
                ProDialog.SetMessage(mess);
            }));
        }

        public void SetValue(double value)
        {
            ProDialog.Dispatcher.BeginInvoke(new Action(() =>
            {
                ProDialog.SetProgressValue(value);
            }));
        }

        public void SetMessage(string mess)
        {
            ProDialog.Dispatcher.BeginInvoke(new Action(() =>
            {
                ProDialog.SetMessage(mess);
            }));
        }

        public void Show(Action workAction)
        {
            this.Worker = new Thread(new ThreadStart(workAction));
            this.ProDialog = new ProgressDialog();

            Worker.Start();
            ProDialog.ShowDialog();
        }
    }
}
