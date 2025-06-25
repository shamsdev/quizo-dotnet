using KarizmaPlatform.Connection.Server.Attributes;

namespace QuizoDotnet.Handlers.RequestHandlers;

[RequestHandler("/test")]
public class TestRequestHandler
{
    [Action("get-test")]
    public async Task<string> GetTest()
    {
        await Task.Delay(3000);
        return "Test";
    }
}