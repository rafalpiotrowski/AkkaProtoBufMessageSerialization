using ProtoBuf;

namespace Library;
public interface IProtocolMember
{
}

[ProtoContract]
public sealed record AccountCreated : IProtocolMember
{
    [ProtoMember(1)] public required string AccountId { get; init; }
}

[ProtoContract]
public sealed record CreateAccount : IProtocolMember
{
    [ProtoMember(1)] public required string AccountId { get; init; }
    [ProtoMember(2)] public required string Name { get; init; }
}