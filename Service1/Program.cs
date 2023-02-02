

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
        .WithRemoting("localhost", 9001)
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

                var manager = system.ActorOf(Props.Create<AccountManagerActor>(), "account-manager");
                registry.Register<AccountManagerActor>(manager);
            });
            Cluster.Get(system).RegisterOnMemberRemoved(() =>
            {
                system.Log.Info($"RegisterOnMemberRemoved");
            });
        });
});

var app = builder.Build();

app.MapGet("/", () => "Service1!");

app.Run();
