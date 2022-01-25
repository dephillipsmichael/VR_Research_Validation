using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pvr_UnitySDKAPI;
using UnityEngine.UI;

public static class Settings
{
    const float DEFAULT_DURATION = 60.0f;

    public static string PLAYER_PREF_KEY_DURATION = "SettingDuration";

    public static float getPlayerPref(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetFloat(key);
        }

        if (key == PLAYER_PREF_KEY_DURATION)
        {
            PlayerPrefs.SetFloat(PLAYER_PREF_KEY_DURATION, DEFAULT_DURATION);
            return DEFAULT_DURATION;
        }
        return 0.0f;
    }

    public static void setPlayerPref(string key, float val)
    {
        PlayerPrefs.SetFloat(key, val);
    }
}