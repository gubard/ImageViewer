using System;
using System.Threading;
using System.Threading.Tasks;

namespace ImageViewer.Helpers;

public static class Wrap
{
    public static async Task IgnoreCancelAsync(Func<Task> action)
    {
        try
        {
            await action.Invoke();
        }
        catch (TaskCanceledException) { }
    }
}
