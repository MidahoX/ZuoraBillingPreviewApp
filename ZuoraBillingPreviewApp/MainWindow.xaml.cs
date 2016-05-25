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
            bool isValidated = ValidateInputs();
            SetResultLabelColor(true);

            if (isValidated)
            {
                SetResultLabelColor(false);

                // disable button click
                btnSubmit.IsEnabled = false;

                BillingPreviewRunInputs bprInputs = new BillingPreviewRunInputs(txtApiUserId.Text, txtApiUserPassword.Password, (DateTime)txtTargetDate.SelectedDate, txtRequestId.Text);

                // create async function
                imgLoading.Visibility = Visibility.Visible;
                lbResult.Text = "Please wait...";
                var slowTask = Task<BillingPreviewRunResult>.Factory.StartNew(() => SubmitBillingPreviewRequest(bprInputs));
                await slowTask;

                StringBuilder messages = new StringBuilder();
                // update text result and show spinning icon
                if (slowTask.Result.Status == "Completed")
                {
                    SetResultLabelColor(false);
                    messages.AppendLine("Status: " + slowTask.Result.Status);                    
                    messages.AppendLine("Download Link: " + slowTask.Result.ResultFileUrl);
                    messages.AppendLine("RequestId: " + slowTask.Result.RequestId);
                    messages.AppendLine("Update Date: " + slowTask.Result.UpdateDate);
                    messages.AppendLine("Total Accounts: " + slowTask.Result.TotalAccounts);
                }
                else
                {
                    SetResultLabelColor(true);                    
                    messages.AppendLine("Status: " + slowTask.Result.Status);
                    if (slowTask.Result.Status == "Pending")
                    {
                        messages.AppendLine("Message: The request is submitted and in progress. Use the requestId and click the submit button again.");
                    }
                    else
                        messages.AppendLine("Message: " + slowTask.Result.Message);
                    messages.AppendLine("RequestId: " + slowTask.Result.RequestId);                    
                }
                lbResult.Text = messages.ToString();

                imgLoading.Visibility = Visibility.Hidden;

                // enable button click
                btnSubmit.IsEnabled = true;
            }
            
        }

        private bool ValidateInputs()
        {
            StringBuilder message = new StringBuilder();
            if (string.IsNullOrWhiteSpace(txtApiUserId.Text))
                message.AppendLine("Please enter Api User Id.");
            if (string.IsNullOrWhiteSpace(txtApiUserPassword.Password))
                message.AppendLine("Please enter Api User Password.");
            if (string.IsNullOrEmpty(txtRequestId.Text))
            {
                if (string.IsNullOrWhiteSpace(txtTargetDate.Text))
                    message.AppendLine("Please pick a Date.");
                else
                {
                    try
                    {
                        var date = Convert.ToDateTime(txtTargetDate.Text);
                    }
                    catch
                    {
                        message.AppendLine("Target date is not in correct format. Please try again.");
                    }
                }
            }
            else
            {
                txtTargetDate.SelectedDate = DateTime.Now;
            }

            lbResult.Text = message.ToString();
            return message.ToString() == "";
        }

        private void SetResultLabelColor(bool isError)
        {
            if(isError)
            {
                lbResult.Foreground = Brushes.Red;
            }
            else
            {
                lbResult.Foreground = Brushes.Black;
            }
        }

        private BillingPreviewRunResult SubmitBillingPreviewRequest(BillingPreviewRunInputs inputs)
        {
            BillingPreviewRunResult result = new BillingPreviewRunResult("Start", "", 0, DateTime.Now, "", "");
            BillingPreviewRunService billingService = new BillingPreviewRunService();
            bool loginSuccess = billingService.Login(inputs.UserId, inputs.Password);

            if (loginSuccess)
            {
                string requestId = "";
                if (inputs.RequestId != "")
                {
                    requestId = inputs.RequestId;
                }
                else
                {
                    requestId = billingService.SubmitBillingPreviewRequest(inputs.TargetDate);
                }

                if (!string.IsNullOrWhiteSpace(requestId))
                {
                    // try 5 times to wait for the result.
                    for (int i = 0; i < 10; i++)
                    {
                        // check for result
                        result = billingService.GetBillingRequestById(requestId);

                        // break if the request is complete
                        if (result.Status == "Completed")
                        {
                            break;
                        }

                        // sleep 5 seconds
                        Thread.Sleep(5000);
                    }
                }
                else
                {
                    return new BillingPreviewRunResult("Error", "", 0, DateTime.Now, "Submit BillingPreviewRun request failed!", requestId);
                }

                return result;
            }
            else
                return new BillingPreviewRunResult("Error", "", 0, DateTime.Now, "Login failed! Check your credentials.", "");
        }

        private void txtRequestId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRequestId.Text))
            {
                btnSubmit.Content = "Submit BillingPreviewRun Request";
            }
            else
            {
                btnSubmit.Content = "Get File By Request ID";
            }
        }
    }

    public class BillingPreviewRunInputs
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public DateTime TargetDate { get; set; }
        public string RequestId { get; set; }

        public BillingPreviewRunInputs(string userid, string password, DateTime targetDate, string requestId)
        {
            this.UserId = userid;
            this.Password = password;
            this.TargetDate = targetDate;
            this.RequestId = requestId;
        }
    }
}
