using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pvr_UnitySDKAPI;
using UnityEngine.UI;
using TMPro;

public class CanvasStateManagerSettings : Singleton<CanvasStateManagerSettings>
{
    private int index = 0;

    private Vector3 startScale;

    [SerializeField]
    Slider DurationSlider;
    [SerializeField]
    TextMeshProUGUI DurationValueText;

    [SerializeField]
    Slider SharkSlider;
    [SerializeField]
    TextMeshProUGUI SharkValueText;

    [SerializeField]
    Slider FishDensitySlider;
    [SerializeField]
    TextMeshProUGUI FishDensityValueText;

    [SerializeField]
    Toggle englishToggle;
    [SerializeField]
    Toggle portugueseToggle;

    protected override void Awake()
    {
        base.Awake();
        startScale = transform.localScale;
    }

    private int canvasCount()
    {
        return transform.GetComponentsInChildren<ScreenFade>().Length; 
    }

    public void moveForward()
    {
        moveForward(null);
    }

    public void moveForward(string answer)
    {
        if ((index + 1) >= canvasCount())
        {
            // End of the line
            closeAllCanvas();
            return;
        }
        index++;
        openCanvas(index);
    }

    // Because we need raycasts for eye tracking, we need to
    // shrink the canvas down so it doesnt interfere
    private void honeyIShrunkTheCanvas()
    {
        transform.localScale = Vector3.zero;
    }

    public void honeyIGrewTheCanvas()
    {
        transform.localScale = startScale;
    }

    public void moveBackwards()
    {
        if (index <= 0)
        {
            closeAllCanvas();
            return;
        }
        index--;
        openCanvas(index);
    }

    private void hideUI()
    {
        GetComponent<ScreenFade>().CloseScreen();
    }

    private void closeAllCanvas()
    {
        ScreenFade root = GetComponent<ScreenFade>();
        foreach (ScreenFade screen in transform.GetComponentsInChildren<ScreenFade>())
        {
            if (screen != root) 
            {
                screen.CloseScreen();
            }            
        }
    }

    private void openCanvas(int index)
    {
        closeAllCanvas();
        transform.GetComponentsInChildren<ScreenFade>()[index].OpenScreen();        

        // Initialize UI to their current values
        if (index == 0)
        {
            float duration = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_DURATION);
            DurationValueText.text = (int)duration + " seconds";
            DurationSlider.value = duration;

            float sharkTime = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_SHARK);
            SharkValueText.text = (int)sharkTime + " seconds";
            SharkSlider.value = sharkTime;

            float fishDensity = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_FISH_DENSITY);
            FishDensityValueText.text = (int)fishDensity + " per minute";
            FishDensitySlider.value = fishDensity;

            string langStr = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_LANGUAGE, Settings.LANGUAGE_PREF_ENGLISH);
            bool isEnglishSelected = (langStr == Settings.LANGUAGE_PREF_ENGLISH);
            englishToggle.isOn = isEnglishSelected;
            portugueseToggle.isOn = !isEnglishSelected;
        }
    }

    // Start is called before the first frame update
    void Start()
    {  
    }

    public void Update()
    {
        if (Application.isEditor)
        {
            if(Input.GetMouseButtonDown(0))
            {
                openCanvas(0);

            }
            if (Input.GetMouseButtonDown(1))
            {
                moveBackwards();
            }
        } 
        else
        {
            if (Controller.UPvr_GetKeyLongPressed(0, Pvr_KeyCode.Y))
            {
                openCanvas(0);
            }
        }        
    }

    public void noButtonClicked()
    {
        CanvasStateManagerSettings.Instance.moveForward("no");
    }

    public void yesButtonClicked()
    {
        CanvasStateManagerSettings.Instance.moveForward("yes");
    }

    public void onDurationSliderChanged(float val)
    {
        Settings.setPlayerPref(Settings.PLAYER_PREF_KEY_DURATION, val);
        DurationValueText.text = (int)val + " seconds";
    }

    public void onSharkSliderChanged(float val)
    {
        Settings.setPlayerPref(Settings.PLAYER_PREF_KEY_SHARK, val);
        SharkValueText.text = (int)val + " seconds";
    }

    public void onFishDensitySliderChanged(float val)
    {
        Settings.setPlayerPref(Settings.PLAYER_PREF_KEY_FISH_DENSITY, val);
        FishDensityValueText.text = (int)val + " fish per minute";
    }

    public void onEnglishLanguageSelected(bool selected)
    {
        Debug.Log("onEnglishLanguageSelected " + selected);
        if (selected)
        {
            Settings.setPlayerPref(Settings.PLAYER_PREF_KEY_LANGUAGE, Settings.LANGUAGE_PREF_ENGLISH);
            Settings.reloadLocalization();
            CanvasStateManager.Instance.refreshCurrentCanvas();
        }
    }

    public void onPortugueseLanguageSelected(bool selected)
    {
        Debug.Log("onPortugueseLanguageSelected " + selected);
        if (selected)
        {
            Settings.setPlayerPref(Settings.PLAYER_PREF_KEY_LANGUAGE, Settings.LANGUAGE_PREF_PORTUGUESE);
            Settings.reloadLocalization();
            CanvasStateManager.Instance.refreshCurrentCanvas();
        }
    }
}