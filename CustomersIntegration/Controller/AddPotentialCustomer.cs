using System;
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
    public static class AddPotentialCustomer
    {
        public static async Task<IActionResult> Run(HttpRequest _req, 
                                                    ILogger     _log, 
                                                    dynamic     _data, 
                                                    Guid        _id     = new Guid(), 
                                                    bool        _update = false)
        {
            using ServiceClient serviceClient = GetConnection.Run(_log);

            try
            {
                Entity lead = new Entity("lead")
                {
                    ["firstname"]               = BodyValue.getName(_req, _data),
                    ["emailaddress1"]           = BodyValue.getEmail(_req, _data),
                    ["inove_txt_document"]      = BodyValue.getDocument(_req, _data),
                    ["inove_txt_cpf"]           = BodyValue.getDocument(_req, _data),
                    ["inove_cjo_clienttype"]    = BodyValue.getClientType(_req, _data)
                };

                if (!_update)
                {
                    var leadId = await serviceClient.CreateAsync(lead);

                    return new OkObjectResult("Lead created!!!");
                }
                else
                {
                    lead.Id = _id;
                    await serviceClient.UpdateAsync(lead);

                    return new OkObjectResult("Lead updated!!!");
                }
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(exception.Message);
            }
        }
    }
}
