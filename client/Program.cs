using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace ConsoleApp5
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                 .WithUrl("https://localhost:44356/chathub", options =>
                 {
                    options.UseDefaultCredentials = true;
                 })
                 .ConfigureLogging(logging => logging.AddConsole())
                 .WithAutomaticReconnect()
                 .Build();
            try
            {
                await connection.StartAsync();

                Console.WriteLine("Starting connection. Press Ctrl-C to close.");

                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (sender, a) =>
                {
                    a.Cancel = true;
                    Console.WriteLine("Stopping loops...");
                    cts.Cancel();
                };

                connection.Closed += e =>
                {
                    Console.WriteLine("Connection closed with error: {0}", e);

                    cts.Cancel();
                    return Task.CompletedTask;
                };


                //await connection.InvokeAsync("DataExchangeHub_Subscribe",
                //    "Cache", "Rulesets");

                // keep the thread alive
                while (!cts.IsCancellationRequested)
                    await Task.Delay(1000);
            }
            catch (AggregateException aex) when (aex.InnerExceptions.All(e => e is OperationCanceledException))
            {
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                await connection.DisposeAsync();
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
