using NATS.Client;
using System.Text;

namespace BidService.NatSubscriberService
{
    public class NatsSubscriberService : IHostedService
    {
        private readonly IConnection _natsConnection;

        public NatsSubscriberService(IConnection natsConnection)
        {
            _natsConnection = natsConnection;
        }

        /// <summary>
        /// This is he NATs Service that Listen to the messsage pusblished by the Publisher which will display in the console
        /// NOTE: you dont have to launch before u receive the message... its uses jetstream to stream the data in the console
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var subscription = _natsConnection.SubscribeAsync("auction.created");
            subscription.MessageHandler += (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Message.Data);
                Console.WriteLine($"Received message: {message}");
            };
            subscription.Start();
            Console.WriteLine("Subscribe to 'auction.created'. Listening for messages...");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Clean up resources, if necessary
            _natsConnection.Drain();
            _natsConnection.Close();
            return Task.CompletedTask;
        }
    }
}
