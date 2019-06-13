using System;
using System.Threading;
using System.Threading.Tasks;

// Todo: Move to a utility ns?
public class AsyncLock : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public async Task<AsyncLock> LockAsync(TimeSpan timeout, CancellationToken ct = default)
    {
        var lockAcquired = await _semaphore.WaitAsync(timeout, ct);

        return lockAcquired ? (this) : null;
    }

    public async Task<AsyncLock> LockAsync(CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        return this;
    }

    public void Dispose()
    {
        _semaphore.Release();
    }
}
