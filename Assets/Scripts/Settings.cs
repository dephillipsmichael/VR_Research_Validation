using System.Collections.Generic;
using UnityEngine;
using System;

public static class Settings
{
    const float DEFAULT_DURATION = 60.0f;
    const float DEFAULT_SHARK = 15.0f;
    const float DEFAULT_FISH_DENSITY = 60f;

    public static string PLAYER_PREF_KEY_DURATION = "SettingDuration";
    public static string PLAYER_PREF_KEY_SHARK = "SettingSharkTime";
    public static string PLAYER_PREF_KEY_FISH_DENSITY = "SettingFishDensity";
    public static string PLAYER_PREF_KEY_LANGUAGE = "SettingLanguage";

    public static string LANGUAGE_PREF_ENGLISH = "en_US";
    public static string LANGUAGE_PREF_PORTUGUESE = "pt_PT";

    public static List<LocalizedKeyValue> localizationList = null;

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
        else if (key == PLAYER_PREF_KEY_FISH_DENSITY)
        {
            PlayerPrefs.SetFloat(PLAYER_PREF_KEY_FISH_DENSITY, DEFAULT_FISH_DENSITY);
            return DEFAULT_FISH_DENSITY;
        }

        return 0.0f;
    }

    public static string getPlayerPref(string key, string defaultVal)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetString(key);
        }
        else if (key == PLAYER_PREF_KEY_LANGUAGE)
        {
            return LANGUAGE_PREF_ENGLISH;
        }

        return "";
    }

    public static void setPlayerPref(string key, float val)
    {
        PlayerPrefs.SetFloat(key, val);
    }

    public static void setPlayerPref(string key, string val)
    {
        PlayerPrefs.SetString(key, val);    
    }

    public static string getLocalizedString(string key)
    {
        if (localizationList == null)
        {
            reloadLocalization();
        }
        foreach(LocalizedKeyValue keyValue in localizationList)
        {
            if (key == keyValue.key)
            {
                return keyValue.value;
            }
        }
        return key;
    }

    public static void reloadLocalization()
    {
        var jsonTextFile = Resources.Load<TextAsset>("Text/LocalizedStrings");        
        LocalizationFile locFile = JsonUtility.FromJson<LocalizationFile>(jsonTextFile.text);
        if (Settings.getPlayerPref(PLAYER_PREF_KEY_LANGUAGE, LANGUAGE_PREF_ENGLISH) == LANGUAGE_PREF_ENGLISH)
        {
            localizationList = locFile.en_US;
        }
        else  // Use portuguese
        {
            localizationList = locFile.pt_PT;
        }
    }

    [Serializable]
    public class LocalizationFile
    {
        public List<LocalizedKeyValue> en_US;
        public List<LocalizedKeyValue> pt_PT;        
    }

    [Serializable]
    public class LocalizedKeyValue
    {
        public string key;
        public string value;
    }
}