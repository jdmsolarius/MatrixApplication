using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;

public class ThreadInfo
{
    public Thread Thread { get; set; }
    public DateTime? StartTime { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; }

    public ThreadInfo(Thread thread, DateTime? startTime, CancellationTokenSource cancellationTokenSource)
    {
        Thread = thread;
        StartTime = startTime;
        CancellationTokenSource = cancellationTokenSource;
    }
}

public class ThreadTimer : System.Timers.Timer, IDisposable
{
    private ConcurrentBag<ThreadInfo> _threads = new ConcurrentBag<ThreadInfo>();
    private Action<CancellationToken> _task;
    private string _threadName;
    private System.Timers.Timer _debugTimer;
    private bool _isRunning;

    public ThreadTimer(double interval, Action<CancellationToken> task, string threadName = "") : base(interval)
    {
        _task = task;
        _threadName = threadName;
        Elapsed += OnElapsed;

        _debugTimer = new System.Timers.Timer(1 * 5 * 1000);
        _debugTimer.Elapsed += OnDebugTimerElapsed;
        _debugTimer.Start();
    }


    private void OnElapsed(object sender, ElapsedEventArgs e)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var thread = new Thread(() =>
        {
            _task(cancellationTokenSource.Token);
        });
        thread.IsBackground = true;
        thread.Name = _threadName;
        if (!_threads.Select(t => t.Thread.Name).Contains(_threadName))
        {
            thread.Start();
            _threads.Add(new ThreadInfo(thread, DateTime.UtcNow, cancellationTokenSource));
        }
    }

    private void OnDebugTimerElapsed(object sender, ElapsedEventArgs e)
    {
        Trace.WriteLine("Thread count: " + ThreadCount);
        Trace.WriteLine("Total memory: " + GC.GetTotalMemory(false));
        Trace.WriteLine("Processor count: " + Environment.ProcessorCount);
        Trace.WriteLine("System uptime: " + TimeSpan.FromMilliseconds(Environment.TickCount));

        // Print information about each thread
        foreach (var threadInfo in _threads)
        {
            Trace.WriteLine("Thread name: " + threadInfo.Thread.Name);
            Trace.WriteLine("Thread state: " + threadInfo.Thread.ThreadState);
            Trace.WriteLine("Is Running: " + threadInfo.Thread.IsAlive);
            Trace.WriteLine("Thread running time: " + (DateTime.UtcNow - threadInfo.StartTime));
            // Add additional thread information as needed
        }
    }

    public Thread? GrabThread(string name)
    {
        return _threads.FirstOrDefault(t => t?.Thread?.Name == name)?.Thread == null ? null : _threads.FirstOrDefault(t => t.Thread.Name == name).Thread;
    }

    public void StopThread(string name)
    {
        var threadInfo = _threads.FirstOrDefault(t => t.Thread.Name == name);
        if (threadInfo != null)
        {
            threadInfo.CancellationTokenSource.Cancel();
            threadInfo.Thread.Join();
            _threads = new ConcurrentBag<ThreadInfo>(_threads.Where(t => t.Thread.Name != name).AsEnumerable());
        }
        else
        {
            return;
        }
    }
    public double? GetThreadElapsedTime(string name)
    {
        var threadInfo = _threads.FirstOrDefault(t => t.Thread.Name == name);

        if (threadInfo != null)
        {
            double elapsedTime = (DateTime.UtcNow - threadInfo.StartTime.Value).TotalMilliseconds;
            return elapsedTime;
        }
        else
        {
            return null;
        }
    }
    public void AwaitThread(string name, CancellationToken token)
    {
        var threadInfo = _threads.FirstOrDefault(t => t.Thread.Name == name);
        if (threadInfo == null)
            return;

        while (threadInfo.Thread.IsAlive)
        {
            if (token.IsCancellationRequested)
                break;

            Thread.Sleep(100);
        }
    }

    public void StopAllThreads()
    {
        foreach (ThreadInfo threadInfo in _threads)
        {
            threadInfo.CancellationTokenSource.Cancel();
            threadInfo.Thread.Join();
        }
        _threads.Clear();
    }

    public void WaitForAllThreads()
    {
        foreach (ThreadInfo threadInfo in _threads)
        {
            threadInfo.Thread.Join();
        }
    }

    public int ThreadCount
    {
        get { return _threads.Count; }
    }

    public bool IsRunning
    {
        get { return Enabled; }
    }

    public void Dispose()
    {
         StopAllThreads();
        _debugTimer.Dispose();
    }
}