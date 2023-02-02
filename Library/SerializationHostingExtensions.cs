using Akka.Hosting;

namespace Library;

public static class SerializationHostingExtensions
{
    public static AkkaConfigurationBuilder AddSerialization(this AkkaConfigurationBuilder builder)
    {
        return builder.WithCustomSerializer("account-messages", new[] { typeof(IProtocolMember) },
            system => new MessageSerializer(system));
    }
}