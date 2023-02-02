using Akka.Actor;
using Library;

namespace Service1;

public sealed class AccountVerificationActor : ReceiveActor
{
    public AccountVerificationActor()
    {
        Receive<CreateAccount>(msg =>
        {
            Context.System.Log.Info($"Request to create account: {msg}");

            var manager = Context.System.ActorSelection("akka.tcp://smaple-app@localhost:9001/user/account-manager");
            manager.Tell(msg);
        });

        Receive<AccountCreated>(msg =>
        {
            Context.System.Log.Info($"Account {msg} created");
        });
    }
}
