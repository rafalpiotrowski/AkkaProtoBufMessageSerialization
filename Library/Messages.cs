using ProtoBuf;

namespace Library;
public interface IProtocolMember
{
}

public interface IWithAccountId 
{
    string AccountId { get; }
}

[ProtoContract]
public sealed record AccountCreated : IProtocolMember, IWithAccountId
{
    [ProtoMember(1)] public required string AccountId { get; init; }
}

[ProtoContract]
public sealed record CreateAccount : IProtocolMember, IWithAccountId
{
    [ProtoMember(1)] public required string AccountId { get; init; }
    [ProtoMember(2)] public required string Name { get; init; }
}