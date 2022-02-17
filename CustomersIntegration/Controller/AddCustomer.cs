using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CustomersIntegration.Controller;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using CustomersIntegration.Model;

namespace CustomerIntegration.Controller
{
    public static class AddCustomer
    {
        public static async Task<IActionResult> Run(HttpRequest _req, 
                                                    ILogger     _log, 
                                                    dynamic     _data)
        {
            using ServiceClient serviceClient = GetConnection.Run(_log);

            try
            {
                var customerId  = await serviceClient.CreateAsync(new Entity("account")
                {
                    ["name"]                        = BodyValue.getName(_req, _data),
                    ["emailaddress1"]               = BodyValue.getEmail(_req, _data),
                    ["inove_txt_document"]          = BodyValue.getDocument(_req, _data),
                    ["inove_txt_cpf"]               = BodyValue.getDocument(_req, _data),
                    ["inove_cjo_clienttype"]        = BodyValue.getClientType(_req, _data)
                });

                return new OkObjectResult("Customer created!!!");
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(exception.Message);
            }
        }
    }
}

