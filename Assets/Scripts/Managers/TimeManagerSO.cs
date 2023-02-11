using UnityEngine;
using System.Diagnostics;

/// <summary>
/// This ScriptableObject centralises data about the StopWatch used throughout the levels as the data is being used by multiple classes
/// </summary>
[CreateAssetMenu(fileName = "TimeManagerSO", menuName = "ScriptableObjects/Time Manager")]
public class TimeManagerSO : ScriptableObject
{
    private Stopwatch timer;
    private void OnEnable()
    {
        timer = new Stopwatch();
    }

    public void StartTimer()
    {
        // check whether timer is running because we want to keep track of the time the player takes to beat a level within three lives
        // we do not want the timer to reset every time the scene reloads when player dies but has lives remaining
        if (!timer.IsRunning)
        {
            timer.Start();
        }
    }

    public void StopTimer()
    {
        timer.Stop();
    }

    public void ResetTimer()
    {
        timer.Reset();
    }

    public long GetTiming()
    {
        // get current elapsed time of the stopwatch
        return timer.ElapsedMilliseconds;
    }
}
