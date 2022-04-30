using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasStateManager : Singleton<CanvasStateManager>
{
    public int index = 0;

    public int fishyTimeIdx = 5;

    private Vector3 startScale;

    private List<int> iterableCanvasIndexList = new List<int>();

    private System.Random rng = new System.Random();

   // Start is called before the first frame update
    void Start()
    {
        // Randomly re-order the questions for Shark/Dolphin/Ocotopus
        // Last 3 questions should be randomly ordered, so use mapping
        ScreenFade[] screens = transform.GetComponentsInChildren<ScreenFade>();
        int count = screens.Length;

        List<int> lastThreeQuestionsOrder = new List<int>();
        lastThreeQuestionsOrder.Add(count - 3);
        lastThreeQuestionsOrder.Add(count - 2);
        lastThreeQuestionsOrder.Add(count - 1);

        // Randomize the order of this list
        int n = lastThreeQuestionsOrder.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = lastThreeQuestionsOrder[k];
            lastThreeQuestionsOrder[k] = lastThreeQuestionsOrder[n];
            lastThreeQuestionsOrder[n] = value;
        }

        iterableCanvasIndexList.Clear();
        for (int i = 0; i < count; i++)
        {
            if (i < (count - 3))  // add the normal expected index
            {
                iterableCanvasIndexList.Add(i);
                print(i + " is " + i);
            }            
            else // Add the random index from last 3 questions
            {
                int randomIdx = lastThreeQuestionsOrder[count - i - 1];
                iterableCanvasIndexList.Add(randomIdx);
                print(i + " is " + randomIdx);
            }
        }

        // Save the order displayed to the settings file
        for (int i = 0; i < iterableCanvasIndexList.Count; i++)
        {
            print("Screen " + screens[iterableCanvasIndexList[i]].gameObject.name);
        }
        openCanvas(index);
    }   

    protected override void Awake()
    {
        base.Awake();
        startScale = transform.localScale;
    }

    private ScreenFade getCanvasAtIndex(int index)
    {
        int actualIndex = iterableCanvasIndexList[index];
        return transform.GetComponentsInChildren<ScreenFade>()[actualIndex];
    }

    private int canvasCount()
    {
        return iterableCanvasIndexList.Count;
    }

    public void moveForward()
    {
        moveForward(null);
    }

    public void moveForward(string answer)
    {
        if (index >= 0 && index < canvasCount())
        {
            string identifier = getCanvasAtIndex(index).gameObject.name;
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

    public void refreshCurrentCanvas()
    {
        openCanvas(index);
    }

    private void openCanvas(int index)
    {
        closeAllCanvas();
        ScreenFade screen = getCanvasAtIndex(index);
        LocalizedText[] textToLocalize = screen.gameObject.GetComponentsInChildren<LocalizedText>();
        foreach(LocalizedText text in textToLocalize)
        {
            text.localize();
        }
        screen.OpenScreen();
        DataLogger.Instance.logEventTimestamp("UserShownScreen" + index, Time.time);
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