using System.Text;
using BambaIba.Application.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BambaIba.Application.Abstractions.Services;
public class VideoPublisher
{
    private readonly ConnectionFactory _factory;

    public VideoPublisher(IOptions<RabbitMqOptions> options)
    {
        RabbitMqOptions cfg = options.Value;
        _factory = new ConnectionFactory()
        {
            HostName = cfg.Host,
            UserName = cfg.User,
            Password = cfg.Pass
        };
    }

    public async Task PublishVideoForProcessingAsync(Guid videoId, CancellationToken cancellationToken = default)
    {
        // 1. Connexion async
        await using IConnection connection = await _factory.CreateConnectionAsync(cancellationToken);

        // 2. Channel async
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        // 3. Déclarer la queue (assure qu’elle existe)
        await channel.QueueDeclareAsync(
            queue: "video-processing",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken
        );

        // 4. Préparer le message
        byte[] body = Encoding.UTF8.GetBytes(videoId.ToString());

        // --- Création explicite des propriétés ---
        // Crée un objet qui implémente IBasicProperties
        IBasicProperties props = new RabbitMQ.Client.BasicProperties
        {
            // Exemple : Rendre le message persistant sur le disque du broker
            Persistent = true
        };

        // 5. Publier le message (note le <IBasicProperties>)
        await channel.BasicPublishAsync(
             exchange: "video_exchange",
             routingKey: "video-processing",
             //mandatory: false,
             //basicProperties: props, // Passage de l'objet BasicProperties
             body: body,
             cancellationToken: cancellationToken
         );
    }
}
