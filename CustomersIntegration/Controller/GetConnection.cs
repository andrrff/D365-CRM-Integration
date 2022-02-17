using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace CustomersIntegration.Controller
{
    static class Const
    {
        public const int Retry            = 3;
        public const int TimeoutSeconds   = 15;
    }

    public static class GetConnection
    {
        public static ServiceClient Run(ILogger log)
        {
            string url      = Environment.GetEnvironmentVariable("URL");
            string username = Environment.GetEnvironmentVariable("E-mail");
            string password = Environment.GetEnvironmentVariable("Password");
            string authType = Environment.GetEnvironmentVariable("AuthType");

            string connectionString =   "Url="      + url       + "; " +
                                        "AuthType=" + authType  + "; " +
                                        "Username=" + username  + "; " +
                                        "Password=" + password  + "; ";

            ServiceClient.MaxConnectionTimeout = new TimeSpan(0, 0, Const.TimeoutSeconds);

            try
            {
                for (int retry = 0; retry < Const.Retry; retry++)
                {
                    ServiceClient serviceClient = new ServiceClient(connectionString);

                    if (serviceClient.IsReady)
                    {
                        log.LogInformation("Success to Established Connection!!!");

                        return serviceClient;
                    }
                    else
                    {
                        log.LogError("Failed to Established Connection!!!");
                        log.LogInformation("Trying to establish connection again...");
                    }
                }
                return new ServiceClient(connectionString);
            }
            catch
            {
                log.LogError("Error in OAuth process");
                return new ServiceClient(connectionString);
            }
        }
    }
}
