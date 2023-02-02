using Akka.Actor;

namespace Library;

public sealed class MessageSerializer : ProtobufMessageSerializer<MessageSerializer>
{
    //Unique Id of our serializer, required by Akka.NET
    public const int Id = 1000;

    static MessageSerializer()
    {
        Add<CreateAccount>("ca");
        Add<AccountCreated>("ac");
    }

    public MessageSerializer(ExtendedActorSystem system) : base(Id, system) { }
}