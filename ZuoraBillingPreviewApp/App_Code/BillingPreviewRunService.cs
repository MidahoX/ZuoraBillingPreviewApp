﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZuoraBillingPreviewApp.WebReference;

namespace ZuoraBillingPreviewApp.App_Code
{
    public class BillingPreviewRunService
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string TargetDate { get; set; }        

        private ZuoraService ZuoraServiceInstance;

        public BillingPreviewRunService()
        {
            ZuoraServiceInstance = new ZuoraService();
        }

        public bool Login(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            try
            {
                LoginResult loginResult = ZuoraServiceInstance.login(username, password);
                if (!string.IsNullOrWhiteSpace(loginResult.Session))
                {
                    // set the session id returned by login request
                    SessionHeader sessionHeader = new SessionHeader();
                    sessionHeader.session = loginResult.Session;
                    ZuoraServiceInstance.SessionHeaderValue = sessionHeader;
                    return true;
                }
                else
                {
                    return false;
                }
            }catch{
                return false;
            }
        }        

        public string SubmitBillingPreviewRequest(string targetDate)
        {
            BillingPreviewRun billPreviewRun = new BillingPreviewRun();
            billPreviewRun.TargetDate = new DateTime(2017, 1, 1);
            billPreviewRun.TargetDateSpecified = true;

            // create zuora request object
            zObject[] requests = new zObject[1];
            requests[0] = (zObject)billPreviewRun;

            try
            {
                // trigger create function service
                SaveResult[] results = ZuoraServiceInstance.create(requests);


                SaveResultStatus requestResultObj = ProcessSingleRequest(results);


                return requestResultObj.RequestId;
            }
            catch
            {
                return "";
            }
        }

        public string GetBillingRequestById(string requestId)
        {
            string url = "";
            try
            {                
                string query = string.Format("select CreatedById, UpdatedDate, UpdatedById, Id, TotalAccounts, Status, ResultFileUrl from BillingPreviewRun where Id='{0}'", requestId);
                QueryResult qr = ZuoraServiceInstance.query(query);

                if (qr.records.Count() > 0)
                {
                    foreach (BillingPreviewRun record in qr.records)
                    {
                        url = record.ResultFileUrl;
                    }
                }
            }
            catch
            {
                
            }

            return url;
        }

        public SaveResultStatus ProcessSingleRequest(SaveResult[] results)
        {
            SaveResultStatus requestResult = new SaveResultStatus();
            StringBuilder text = new StringBuilder();
            foreach (SaveResult r in results)
            {
                if (!r.Success)
                {
                    Error[] errors = r.Errors;
                    foreach (var error in errors)
                    {
                        text.AppendLine(error.Message);
                    }
                    requestResult.Success = r.Success;
                }
                else
                {
                    requestResult.RequestId = r.Id;
                }
            }

            requestResult.Message = text.ToString();

            return requestResult;
        }
    }

    public class SaveResultStatus{
        public string RequestId { get; set; }
        public string Message {get;set;}
        public bool Success { get; set; }
    }    
}