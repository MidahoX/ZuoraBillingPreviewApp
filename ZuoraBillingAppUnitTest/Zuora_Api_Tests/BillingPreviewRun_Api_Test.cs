using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZuoraBillingPreviewApp.WebReference;
using System.Text;
using ZuoraBillingPreviewApp.App_Code;

namespace ZuoraBillingAppUnitTest.Zuora_Api_Test
{
    [TestClass]
    public class Zuora_Api_Test
    {
        private BillingPreviewRunService BillingService;

        [TestInitialize]
        public void Init()
        {
            BillingService = new BillingPreviewRunService();
            BillingService.Login(AppSettings.Username, AppSettings.Password);
        }

        [TestMethod]
        public void Login_With_ApiAccount_Return_True()
        {
            bool success = BillingService.Login(AppSettings.Username, AppSettings.Password);
            Assert.IsTrue(success, "Login Failed. Check Username and Password.");            
        }

        [TestMethod]
        public void Submit_BillingPreviewRun_Request_Return_RequestId()
        {
            string requestId = BillingService.SubmitBillingPreviewRequest("");

            // if there is no error message than submit successfully
            Assert.IsTrue(!string.IsNullOrWhiteSpace(requestId));            
        }

        [TestMethod]
        public void Completed_BillingPreviewRun_Request_Should_Return_ResultFileUrl()
        {
            // Sample request id 2c92a0fd54cd2ff40154d0a2b6b97afc (this request should be completed)                        
            BillingPreviewRunResult result = BillingService.GetBillingRequestById("2c92a0fd54cd2ff40154d0a2b6b97afc");

            // if there is no error message than submit successfully
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result.ResultFileUrl));            
        }

        [TestMethod]
        public void Completed_BillingPreviewRun_Request_Should_Return_Completed_Status()
        {
            // Sample request id 2c92a0fd54cd2ff40154d0a2b6b97afc (this request is already completed)                        
            BillingPreviewRunResult result = BillingService.GetBillingRequestById("2c92a0fd54cd2ff40154d0a2b6b97afc");

            // if there is no error message than submit successfully
            Assert.IsTrue(result.Status == "Completed");
        }
    }
}
