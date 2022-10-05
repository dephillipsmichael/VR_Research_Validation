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
    Slider FishEatFoodSlider;
    [SerializeField]
    TextMeshProUGUI FishEatFoodValueText;

    [SerializeField]
    Toggle englishToggle;
    [SerializeField]
    Toggle portugueseToggle;

    [SerializeField]
    Toggle sharkToggle;
    [SerializeField]
    Toggle dolphinToggle;
    [SerializeField]
    Toggle randomOrderToggle;

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

            float fishEatFoodPercentage = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_FISH_EAT_PERCENTAGE);
            FishEatFoodValueText.text = (int)fishEatFoodPercentage + "%";
            FishEatFoodSlider.value = fishEatFoodPercentage;

            string langStr = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_LANGUAGE, Settings.LANGUAGE_PREF_ENGLISH);
            bool isEnglishSelected = (langStr == Settings.LANGUAGE_PREF_ENGLISH);
            englishToggle.isOn = isEnglishSelected;
            portugueseToggle.isOn = !isEnglishSelected;

            string sharkOrderStr = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_SHARK_ORDER, Settings.SHARK_ORDER_SHARK_FIRST);
            sharkToggle.isOn = sharkOrderStr == Settings.SHARK_ORDER_SHARK_FIRST;
            dolphinToggle.isOn = sharkOrderStr == Settings.SHARK_ORDER_DOLPHIN_FIRST;
            randomOrderToggle.isOn = sharkOrderStr == Settings.SHARK_ORDER_RANDOM;
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
                // Only allow the operator to change settings if
                // they are on the first or second onboarding screen
                if (CanvasStateManager.Instance.index <= 1)
                {
                    openCanvas(0);
                }                
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

    public void onFishEatPercentageSliderChanged(float val)
    {
        Settings.setPlayerPref(Settings.PLAYER_PREF_KEY_FISH_EAT_PERCENTAGE, val);
        FishEatFoodValueText.text = (int)val + "%";
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

    public void onSharkOrderSharkFirstSelected(bool selected)
    {
        Debug.Log("onSharkOrderSharkFirstSelected " + selected);
        if (selected)
        {
            Settings.setPlayerPref(Settings.PLAYER_PREF_KEY_SHARK_ORDER, Settings.SHARK_ORDER_SHARK_FIRST);            
        }
    }

    public void onSharkOrderDolphinFirstSelected(bool selected)
    {
        Debug.Log("onSharkOrderDolphinFirstSelected " + selected);
        if (selected)
        {
            Settings.setPlayerPref(Settings.PLAYER_PREF_KEY_SHARK_ORDER, Settings.SHARK_ORDER_DOLPHIN_FIRST);
        }
    }

    public void onSharkOrderRandomSelected(bool selected)
    {
        Debug.Log("onSharkOrderRandomSelected " + selected);
        if (selected)
        {
            Settings.setPlayerPref(Settings.PLAYER_PREF_KEY_SHARK_ORDER, Settings.SHARK_ORDER_RANDOM);
        }
    }
}