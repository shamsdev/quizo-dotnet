using KarizmaPlatform.Connection.Server.Attributes;
using KarizmaPlatform.Connection.Server.Base;

namespace QuizoDotnet.Handlers.RequestHandlers;

[RequestHandler("/test")]
public class TestRequestHandler : BaseRequestHandler
{
    [Action("get-test", needAuthorizedUser: false)]
    public async Task<string> GetTest()
    {
        await Task.Delay(1000);
        return "Test";
    }
}