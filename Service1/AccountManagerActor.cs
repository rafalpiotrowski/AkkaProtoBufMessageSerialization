using Akka.Actor;
using Library;

namespace Service1;

public sealed class AccountManagerActor : ReceiveActor
{
    public AccountManagerActor()
    {
        Receive<CreateAccount>(msg =>
        {
            Sender.Tell(new AccountCreated() { AccountId = msg.AccountId });
        });
    }
}
