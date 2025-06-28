namespace QuizoDotnet.Application.Interfaces;

public interface IClientCallService
{
    public Task Send(string connectionId, string address, object body);
}