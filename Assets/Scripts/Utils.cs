using UnityEngine;

/// <summary>
/// Utils stores helper methods that are used by multiple classes
/// </summary>
public static class Utils
{
    public static string formatMillisecondsToDisplayTime(long milliseconds)
    {
        int seconds = (int)(milliseconds / 1000) % 60;
        int minutes = (int)(milliseconds / 1000) / 60;
        int remainingMilliseconds = (int)milliseconds % 1000;

        // format time as 00:00:000 (mm:ss:msmsms)
        string formattedTime = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, remainingMilliseconds);
        return formattedTime;
    }

    public static void SaveLevelBestTiming(int currentLevel, int timing)
    {
        // save best timing into PlayerPrefs based on which level the player completed
        string keyName = $"Level{currentLevel}BestTiming";
        PlayerPrefs.SetInt(keyName, timing);
    }

    public static int GetLevelBestTiming(int currentLevel)
    {
        // retrieve best timing based on which level the player completed
        string keyName = $"Level{currentLevel}BestTiming";
        return PlayerPrefs.GetInt(keyName);
    }
}

interface IDamageable
{
    public void TakeDamage();
}