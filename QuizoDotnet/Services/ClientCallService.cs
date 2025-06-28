using KarizmaPlatform.Connection.Server.Interfaces;
using QuizoDotnet.Application.Interfaces;

namespace QuizoDotnet.Services;

public class ClientCallService(IMainHubContext mainHubContext) : IClientCallService
{
    public Task Send(string connectionId, string address, object body)
    {
        return mainHubContext.Send(connectionId, address, body);
    }
}