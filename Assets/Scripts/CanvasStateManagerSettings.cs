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

    public static string PLAYER_PREF_KEY_DURATION = "SettingDuration";

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
        if (index >= 0 && index < canvasCount())
        {
            string identifier = transform.GetComponentsInChildren<ScreenFade>()[index].gameObject.name;
            if (identifier != null && answer != null)
            {
                DataLogger.Instance.setAnswer(identifier, answer);
            }
        }

        if ((index + 1) >= canvasCount())
        {
            // End of the line
            closeAllCanvas();
            DataLogger.Instance.saveDataToFile();
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

        if (index == 0)
        {
            if (PlayerPrefs.HasKey(PLAYER_PREF_KEY_DURATION)) {
                DurationSlider.value = PlayerPrefs.GetFloat(PLAYER_PREF_KEY_DURATION);
            }            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (ScreenFade screen in transform.GetComponentsInChildren<ScreenFade>())
        {
            print("Screen " + screen.name);
        }

        onDurationSliderChanged(Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_DURATION));        
    }

    public void Update()
    {
        if (Application.isEditor)
        {
            if(Input.GetMouseButtonDown(0))
            {
                openCanvas(0);
                //moveForward();
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
        CanvasStateManager.Instance.moveForward("no");
    }

    public void yesButtonClicked()
    {
        CanvasStateManager.Instance.moveForward("yes");
    }

    public void onDurationSliderChanged(float val)
    {
        Settings.setPlayerPref(PLAYER_PREF_KEY_DURATION, val);
        DurationValueText.text = (int)val + " seconds";
    }
}