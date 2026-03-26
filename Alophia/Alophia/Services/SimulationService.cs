using System;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace Alophia.Services;

public class SimulationService : ISimulationService
{
    private DispatcherQueueTimer? _timer;

    public event TickEventHandler? Tick;

    public bool IsRunning { get; private set; }
    public int TicksElapsed { get; private set; }

    public void Start()
    {
        if (IsRunning) return;
        IsRunning = true;

        _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(120);
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    public void Pause()
    {
        if (!IsRunning) return;
        IsRunning = false;

        if (_timer != null)
        {
            _timer.Stop();
            _timer = null;
        }
    }

    public void Reset()
    {
        IsRunning = false;
        TicksElapsed = 0;

        if (_timer != null)
        {
            _timer.Stop();
            _timer = null;
        }

        Tick?.Invoke(this, new TickEventArgs { TicksElapsed = TicksElapsed });
    }

    private void OnTimerTick(DispatcherQueueTimer timer, object args)
    {
        TicksElapsed++;
        Tick?.Invoke(this, new TickEventArgs { TicksElapsed = TicksElapsed });
    }
}
