using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace FunctionAppForSignalR
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log, ExecutionContext context)
        {
            try
            {

                //create a connection to the SignalR site
                    var hub = new HubConnection("https://gabefakesignalr.azurewebsites.net");
                    //hub.Stop();
                    string invocationId = context.InvocationId.ToString();

                //create a proxy to a SignalR Hub so that you can send data. In the Hello World example, the Hub class is named ChatHub
                    var proxy = hub.CreateHubProxy("ChatHub");
                    await hub.Start().ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            log.Info(Environment.NewLine + "  " + "Hub did not start successfullly.");
                            log.Info(Environment.NewLine + " Hub Connection Id :" + hub.ConnectionId);
                        }
                        else
                        {
                            log.Info(Environment.NewLine + "  " + "Hub started successfullly.");
                            log.Info(Environment.NewLine + " Hub Connection Id :" + hub.ConnectionId);
                        }
                    });

                //send the data to the signalR app. In the HelloWorld example, the method inside the ChatHub hub class that we call is named Send
                await proxy.Invoke("Send", invocationId, invocationId);


            }
            catch (Exception ex)
            {

                log.Info(ex.Message);
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
