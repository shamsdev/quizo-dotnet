namespace QuizoDotnet.Application.Logic.Game;

public class GameTimerService
{
    private CancellationTokenSource CancellationTokenSource { get; } = new();

    public void CancelTimer()
    {
        CancellationTokenSource.Cancel();
    }

    public void DisposeTimer()
    {
        CancelTimer();
        CancellationTokenSource.Dispose();
    }

    public async void ScheduleJobAsync(int delay, Func<Task> job)
    {
        CancelTimer();

        try
        {
            await Task.Delay(delay, CancellationTokenSource.Token);
            await job();
        }
        catch (OperationCanceledException)
        {
            // Optional: handle cancellation
        }
    }
}