using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasStateManager : Singleton<CanvasStateManager>
{
    private int index = 0;

    private const int fishyTimeIdx = 5;

    private Vector3 startScale;

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

        if (index == fishyTimeIdx)
        {
            honeyIShrunkTheCanvas();
            FishBehavior.Instance.startFishin();
            DataLogger.Instance.startTracking();
        }
    }

    public void fishyTimeOver()
    {
        honeyIGrewTheCanvas();
        moveForward();
        DataLogger.Instance.stopTracking();
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
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (ScreenFade screen in transform.GetComponentsInChildren<ScreenFade>())
        {
            print("Screen " + screen.name);
        }        
        openCanvas(index);
    }

    public void Update()
    {
        if (Application.isEditor)
        {
            if(Input.GetMouseButtonDown(0))
            {
                moveForward();
            }
            if (Input.GetMouseButtonDown(1))
            {
                moveBackwards();
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
}