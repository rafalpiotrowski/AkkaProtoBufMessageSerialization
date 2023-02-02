using Akka.Actor;
using Library;

namespace Service1;

public sealed class AccountManagerActor : ReceiveActor
{
    public AccountManagerActor()
    {
        Receive<CreateAccount>(msg =>
        {
            Context.System.Log.Info($"Creating account: {msg}");
            Sender.Tell(new AccountCreated() { AccountId = msg.AccountId });
        });
    }
}
