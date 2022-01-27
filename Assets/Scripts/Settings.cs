using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pvr_UnitySDKAPI;
using UnityEngine.UI;

public static class Settings
{
    const float DEFAULT_DURATION = 60.0f;
    const float DEFAULT_SHARK = 15.0f;
    const float DEFAULT_FISH_DENSITY = 60f;

    public static string PLAYER_PREF_KEY_DURATION = "SettingDuration";
    public static string PLAYER_PREF_KEY_SHARK = "SettingSharkTime";
    public static string PLAYER_PREF_KEY_FISH_DENSITY = "SettingFishDensity";

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
        else if (key == PLAYER_PREF_KEY_SHARK)
        {
            PlayerPrefs.SetFloat(PLAYER_PREF_KEY_SHARK, DEFAULT_SHARK);
            return DEFAULT_SHARK;
        }
        else if (key == PLAYER_PREF_KEY_FISH_DENSITY)
        {
            PlayerPrefs.SetFloat(PLAYER_PREF_KEY_FISH_DENSITY, DEFAULT_FISH_DENSITY);
            return DEFAULT_FISH_DENSITY;
        }

        return 0.0f;
    }

    public static void setPlayerPref(string key, float val)
    {
        PlayerPrefs.SetFloat(key, val);
    }
}