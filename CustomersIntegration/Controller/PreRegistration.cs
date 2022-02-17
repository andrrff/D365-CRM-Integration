using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CustomersIntegration.Model;
using Microsoft.Xrm.Sdk.Query;
using CustomersIntegration.Controller;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace CustomerIntegration.Controller
{
    static class Const
    {
        public const string EmailParm = "emailaddress1";
    }

    public static class PreRegistration
    {
        [FunctionName("PreRegistration")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            using ServiceClient serviceClient = GetConnection.Run(log);

            string  requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data        = JsonConvert.DeserializeObject(requestBody);

            try
            {
                string email = BodyValue.getEmail(req, data);

                Entity      customer        = new Entity("account");
                Entity      lead            = new Entity("lead");
                ColumnSet   columnCustomer  = new ColumnSet();
                ColumnSet   columnLead      = new ColumnSet();

                columnCustomer.AddColumn("name");
                columnLead.AddColumn("fullname");

                var accountsCollection  = await existsRecord(serviceClient, customer, 
                                                             columnCustomer,customer.LogicalName, 
                                                             Const.EmailParm,email);

                var leadsCollection     = await existsRecord(serviceClient, lead, 
                                                             columnLead, lead.LogicalName,
                                                             Const.EmailParm, email);

                Tuple<bool, bool> preRegistration = new Tuple<bool, bool>(
                                                            accountsCollection.Entities.Count   > 0, 
                                                            leadsCollection.Entities.Count      > 0);

                if (!preRegistration.Item1 && !preRegistration.Item2)
                {
                    log.LogInformation($"There is no customer or lead with email: {email}");
                    AddPotentialCustomer.Run(req, log, data);
                }
                else if (!preRegistration.Item1 && preRegistration.Item2)
                {
                    log.LogInformation($"There is no customer, but there is the lead with the email: {email}");
                    AddPotentialCustomer.Run(req, log, data, leadsCollection.Entities[0].Id, true);
                }
                else if(preRegistration.Item1)
                {
                    log.LogInformation($"There is already a customer registered with this email: {email}");
                }

                return new OkObjectResult("200 - OK");
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(exception.Message);
            }
        }

        private static Task<EntityCollection> existsRecord( ServiceClient   _serviceClient, 
                                                            Entity          _entity, 
                                                            ColumnSet       _column, 
                                                            string          _typeQuery,
                                                            string          _attributeName,
                                                            string          _value)
        {
            return _serviceClient.RetrieveMultipleAsync(new QueryExpression(_typeQuery)
            {
                EntityName  = _entity.LogicalName,
                ColumnSet   = _column,
                Criteria    = new FilterExpression
                {
                    Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = _attributeName,
                                Operator = ConditionOperator.Equal,
                                Values = { _value }
                            }
                        }
                }
            });
        }
    }
}
