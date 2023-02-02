using Akka.Actor;
using Akka.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace Library;

public abstract class ProtobufMessageSerializer<TMessageSerializer> : SerializerWithStringManifest
    where TMessageSerializer : ProtobufMessageSerializer<TMessageSerializer>
{
    private static readonly Dictionary<Type, string> manifest = new();

    private static readonly Dictionary<Type, Func<object, byte[]>> serializers = new();
    private static readonly Dictionary<string, Func<byte[], object>> deserializers = new();

    protected ProtobufMessageSerializer(int identifier, ExtendedActorSystem system) : base(system)
    {
        Identifier = identifier;
    }

    /// <summary>
    /// Unique value greater than 100 as [0-100] is reserved for Akka.NET System serializers. 
    /// </summary>
    public override int Identifier { get; }

    protected static void Add<T>(string manifestKey) where T : class
    {
        if (!manifest.TryAdd(typeof(T), manifestKey))
            throw new ArgumentException($"MessageSerializer Manifest already added: {manifestKey}->{typeof(T).Name}", nameof(manifestKey));

        if (!serializers.TryAdd(typeof(T), Serialize))
            throw new ArgumentException($"MessageSerializer Type already added: {manifestKey}->{typeof(T).Name}", nameof(manifestKey));

        deserializers[manifestKey] = Deserialize<T>;
    }

    public static object Deserialize<T>(byte[] bytes) where T : class
    {
        Console.WriteLine($"Message type {typeof(T)} deserialized");

        return ProtoBuf.Serializer.Deserialize<T>(new ReadOnlySpan<byte>(bytes));
    }

    public static byte[] Serialize(object obj)
    {
        Console.WriteLine($"Message type {obj.GetType()} serialized");

        using var memoryStream = new MemoryStream();
        ProtoBuf.Serializer.Serialize(memoryStream, obj);
        return memoryStream.ToArray();
    }

    public static bool TryGetManifest<T>([MaybeNullWhen(false)] out string manifestKey)
        => manifest.TryGetValue(typeof(T), out manifestKey);
    public static bool TryGetManifest(Type objectType, [MaybeNullWhen(false)] out string manifestKey)
        => manifest.TryGetValue(objectType, out manifestKey);

    public static bool TryGetSerializer(Type objectType, [MaybeNullWhen(false)] out Func<object, byte[]> serializer)
        => serializers.TryGetValue(objectType, out serializer);
    public static bool TryGetSerializer<T>([MaybeNullWhen(false)] out Func<object, byte[]> serializer)
        => serializers.TryGetValue(typeof(T), out serializer);

    public static bool TryGetDeserializer(string manifestKey, [MaybeNullWhen(false)] out Func<byte[], object> deserializer)
        => deserializers.TryGetValue(manifestKey, out deserializer);

    public static string GetManifest<T>()
    {
        if (manifest.TryGetValue(typeof(T), out var key))
            return key;
        throw new ArgumentOutOfRangeException("{T}", $"Unsupported message type [{typeof(T)}]");
    }

    public override string Manifest(object o)
    {
        if (TryGetManifest(o.GetType(), out var key))
            return key;
        throw new ArgumentOutOfRangeException(nameof(o), $"Unsupported message type [{o.GetType()}]");
    }

    public override object FromBinary(byte[] bytes, string manifest)
    {
        if (TryGetDeserializer(manifest, out var deserializer))
            return deserializer(bytes);
        throw new ArgumentOutOfRangeException(nameof(manifest), $"Unsupported message manifest [{manifest}]");
    }

    public override byte[] ToBinary(object obj)
    {
        if (TryGetSerializer(obj.GetType(), out var serializer))
            return serializer(obj);
        throw new ArgumentOutOfRangeException(nameof(obj), $"Unsupported message type [{obj.GetType()}]");
    }
}