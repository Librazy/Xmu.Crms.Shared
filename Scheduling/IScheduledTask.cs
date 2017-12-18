using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmu.Crms.Shared.Scheduling
{
    public interface IScheduledTask
    {
        TimeSpan Interval { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}