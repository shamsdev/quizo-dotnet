namespace QuizoDotnet.Application.Logic.Game;

public class GameTimerService
{
    private CancellationTokenSource? cancellationTokenSource;

    public void CancelTimer()
    {
        cancellationTokenSource?.Cancel();
    }

    public void DisposeTimer()
    {
        CancelTimer();
        cancellationTokenSource?.Dispose();
    }

    public async void ScheduleJobAsync(int delay, Func<Task> job)
    {
        CancelTimer();
        cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Delay(delay, cancellationTokenSource.Token);
            await job();
        }
        catch (OperationCanceledException)
        {
            // Optional: handle cancellation
        }
    }
}