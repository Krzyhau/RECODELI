using System;
using System.Threading;
using System.Threading.Tasks;

namespace RecoDeli.Scripts.Utils
{
    public static class MainThreadExecutor
    {
        private static SynchronizationContext ctx;
        public static void Initialize(SynchronizationContext context)
        {
            ctx = context;
        }

        public static Task Run(Action action)
        {
            var completion = new TaskCompletionSource<bool>();

            ctx.Post(_ =>
            {
                try
                {
                    action();
                    completion.SetResult(true);
                }
                catch (Exception e)
                {
                    completion.SetException(e);
                }
            }, null);

            return completion.Task;
        }
    }
}
