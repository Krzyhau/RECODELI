using System;
using System.Threading;

namespace RecoDeli.Scripts.Utils
{
    public static class MainThreadExecutor
    {
        private static SynchronizationContext ctx;
        public static void Initialize(SynchronizationContext context)
        {
            ctx = context;
        }

        public static void Run(Action action)
        {
            ctx.Post(_ => action(), null);
        }
    }
}
