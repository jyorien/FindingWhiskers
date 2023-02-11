using UnityEngine;

/// <summary>
/// Utils stores helper methods that are used by multiple classes
/// </summary>
public static class Utils
{
    /// <summary>
    /// Formats milliseconds into mm:ss:msmsms format
    /// </summary>
    /// <param name="milliseconds">The milliseconds to be formatted</param>
    /// <returns>Returns a string with the milliseconds formatted as mm:ss:msmsms</returns>
    public static string FormatMillisecondsToDisplayTime(long milliseconds)
    {
        // convert milliseconds to seconds, minutes and milliseconds
        int seconds = (int)(milliseconds / 1000) % 60;
        int minutes = (int)(milliseconds / 1000) / 60;
        int remainingMilliseconds = (int)milliseconds % 1000;

        // format time as 00:00:000 (mm:ss:msmsms)
        string formattedTime = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, remainingMilliseconds);
        return formattedTime;
    }

    /// <summary>
    /// Save best timing into PlayerPrefs based on which level the player completed.
    /// </summary>
    /// <param name="currentLevel">The level the timing is supposed to be saved for</param>
    /// <param name="timing">The timing to be saved</param>
    public static void SaveLevelBestTiming(int currentLevel, int timing)
    {
        string keyName = $"Level{currentLevel}BestTiming";
        PlayerPrefs.SetInt(keyName, timing);
    }

    /// <summary>
    /// Retrieve best timing based on which level the player completed
    /// </summary>
    /// <param name="currentLevel">The level to retrieve best timing for</param>
    /// <returns></returns>
    public static int GetLevelBestTiming(int currentLevel)
    {
        string keyName = $"Level{currentLevel}BestTiming";
        return PlayerPrefs.GetInt(keyName);
    }
}

/// <summary>
/// This interface is to be implemented by scripts that can be damaged by the player
/// </summary>
interface IDamageable
{
    /// <summary>
    /// This gets called when the player collides with the GameObject that implements this method
    /// </summary>
    public void TakeDamage();
}

public enum GroundType { NONE, DIRT, ICE }
public enum BottomColliderType { NONE, FLOOR, ENEMY }