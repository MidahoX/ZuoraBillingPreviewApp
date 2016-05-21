using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZuoraBillingPreviewApp.WebReference;
using System.Text;
using ZuoraBillingPreviewApp.App_Code;

namespace ZuoraBillingAppUnitTest
{
    [TestClass]
    public class ZuoraBillingPreviewTest
    {
        private BillingPreviewRunService BillingService;

        [TestInitialize]
        public void Init()
        {
            BillingService = new BillingPreviewRunService();
            BillingService.Login(AppSettings.Username, AppSettings.Password);
        }

        [TestMethod]
        public void Test_Login()
        {
            bool success = BillingService.Login(AppSettings.Username, AppSettings.Password);
            Assert.IsTrue(success);            
        }

        [TestMethod]
        public void Test_Submit_BillingPreviewRun_Request()
        {
            string requestId = BillingService.SubmitBillingPreviewRequest("");

            // if there is no error message than submit successfully
            Assert.IsTrue(!string.IsNullOrWhiteSpace(requestId));            
        }

        [TestMethod]
        public void Test_Get_BillingPreviewRun_Result()
        {
            // Sample request id 2c92a0fd54cd2ff40154d0a2b6b97afc
            string downloadUrl = BillingService.GetBillingRequestById("2c92a0fd54cd2ff40154d0a2b6b97afc");

            // if there is no error message than submit successfully
            Assert.IsTrue(!string.IsNullOrWhiteSpace(downloadUrl));     
        }      
    }
}
