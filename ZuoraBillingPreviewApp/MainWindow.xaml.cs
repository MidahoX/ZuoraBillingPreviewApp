using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using ZuoraBillingPreviewApp.App_Code;

namespace ZuoraBillingPreviewApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // disable button click
            btnSubmit.IsEnabled = false;

            var slowTask = Task<SaveResultStatus>.Factory.StartNew(() => SubmitBillingPreviewRequest());

            // update text result and show spinning icon

            await slowTask;
            // update result

            lbResult.Content = slowTask.Result.Message;

            // enable button click
            btnSubmit.IsEnabled = true;
        }

        private SaveResultStatus SubmitBillingPreviewRequest()
        {
            string url = "";

            string userId = txtApiUserId.Text;
            string password = txtApiUserPassword.Password;
            string targetDate = txtTargetDate.Text;

            BillingPreviewRunService billingService = new BillingPreviewRunService();
            billingService.Login(userId, password);
            string requestId = billingService.SubmitBillingPreviewRequest(targetDate);

            if (!string.IsNullOrWhiteSpace(requestId))
            {
                // try 5 times to wait for the result.
                for (int i = 0; i < 5 && url != ""; i++)
                {
                    // check for result
                    url = billingService.GetBillingRequestById(requestId);

                    // sleep 5 seconds
                    Thread.Sleep(5000);
                }
            }
            
            return new SaveResultStatus();
        }
    }
}
