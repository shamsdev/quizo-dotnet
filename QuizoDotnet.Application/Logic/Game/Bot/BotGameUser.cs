namespace QuizoDotnet.Application.Logic.Game.Bot;

public class BotGameUser : GameUser
{
    public void Receive(string address, object body)
    {
        Console.WriteLine($"[Bot] Receiving: {address}");
    }
}