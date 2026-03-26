using System;

namespace Alophia.Services;

public delegate void TickEventHandler(object? sender, TickEventArgs e);

public class TickEventArgs : EventArgs
{
    public int TicksElapsed { get; init; }
}

public interface ISimulationService
{
    event TickEventHandler? Tick;

    bool IsRunning { get; }
    int TicksElapsed { get; }

    void Start();
    void Pause();
    void Reset();
}
