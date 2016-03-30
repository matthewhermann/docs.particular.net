﻿using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Settings;
using NServiceBus.Transports.SQLServer;
#pragma warning disable 618

class Program
{
    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.SqlServer.StoreAndForwardSender";
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVXYZ";
        Random random = new Random();
        EndpointConfiguration configuration = new EndpointConfiguration("Samples.SqlServer.StoreAndForwardSender");

        #region SenderConfiguration

        configuration.UseTransport<SqlServerTransport>()
            .EnableLagacyMultiInstanceMode(ConnectionProvider.GetConnecton);

        configuration.UsePersistence<NHibernatePersistence>();
        configuration.Pipeline.Register("Forward", new ForwardBehavior(), "Forwards messages to destinations.");
        configuration.Pipeline.Register("Store", 
            b => new SendThroughLocalQueueRoutingToDispatchConnector(b.Build<ReadOnlySettings>().LocalAddress()), 
            "Send messages through local endpoint.");

        #endregion

        IEndpointInstance endpoint = await Endpoint.Start(configuration);

        Console.WriteLine("Press enter to publish a message");
        Console.WriteLine("Press any key to exit");
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();
            if (key.Key != ConsoleKey.Enter)
            {
                break;
            }
            string orderId = new string(Enumerable.Range(0, 4).Select(x => letters[random.Next(letters.Length)]).ToArray());
            await endpoint.Publish(new OrderSubmitted
            {
                OrderId = orderId,
                Value = random.Next(100)
            });
            Console.WriteLine("Order {0} placed", orderId);
        }

        await endpoint.Stop();
    }

}