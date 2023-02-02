

using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Hosting;
using Akka.Hosting;
using Akka.Remote.Hosting;
using Library;
using Service1;
using System.Collections.Immutable;

var builder = WebApplication.CreateBuilder(args);

const string SystemName = "smaple-app";

builder.Services.AddAkka(SystemName, builder =>
{
    builder
        .AddSerialization()
        .WithRemoting("localhost", 0)
        .WithClustering(
            new ClusterOptions()
            {
                Roles = ImmutableArray<string>.Empty.Add("accounts").ToArray(),
                SeedNodes = ImmutableArray<string>.Empty.Add($"akka.tcp://{SystemName}@localhost:9001").ToArray()
            })
        .WithActors((system, registry) =>
        {
            Cluster.Get(system).RegisterOnMemberUp(() =>
            {
                system.Log.Info($"RegisterOnMemberUp");

                var accountVerification = system.ActorOf(Props.Create<AccountVerificationActor>(), "account-verification-system");
                registry.Register<AccountVerificationActor>(accountVerification);
                accountVerification.Tell(new CreateAccount() { AccountId = "account1", Name = "Test account1" });
                accountVerification.Tell(new CreateAccount() { AccountId = "account2", Name = "Test account2" });
            });
            Cluster.Get(system).RegisterOnMemberRemoved(() =>
            {
                system.Log.Info($"RegisterOnMemberRemoved");

            });
        });
});

var app = builder.Build();

app.MapGet("/", () => "Service2!");
app.MapGet("/create", () =>
{
    var system = app.Services.GetService<ActorSystem>();
    var registry = ActorRegistry.For(system!);
    var actor = registry.Get<AccountVerificationActor>();
    actor.Tell(new CreateAccount() { AccountId = "account", Name = "Test account" });
});

app.Run();
