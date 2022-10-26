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

    [SerializeField]
    private UnityEngine.UI.Slider sharkDistanceSlider;

    [SerializeField]
    private UnityEngine.UI.Slider dolphinDistanceSlider;

    private static string ANSWER_YES = "yes";
    private static string ANSWER_NO = "no";

    private static string QUESTION_SHARK = "Shark";
    private static string QUESTION_DOLPHIN = "Dolphin";

    // Start is called before the first frame update
    void Start()
    {
        // Randomly re-order the questions for Shark/Dolphin/Ocotopus
        // Last 3 questions should be randomly ordered, so use mapping
        ScreenFade[] screens = transform.GetComponentsInChildren<ScreenFade>();
        int screenCount = screens.Length;

        // Now that there are 2 optional questions at the end,
        // remove these before running the random algorithm
        // and then add them back in at the end
        int count = screenCount - 2;

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

        // Add the last 2 optional questions in after the random algo is done
        iterableCanvasIndexList.Add(screenCount - 2);
        iterableCanvasIndexList.Add(screenCount - 1);

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

    private int sharkDistanceQIdx()
    {
        return canvasCount() - 2;
    }

    private int dolphinDistanceQIdx()
    {
        return canvasCount() - 1;
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

        index++;

        if (index == sharkDistanceQIdx() && 
            DataLogger.Instance.getAnswer(QUESTION_SHARK) != ANSWER_YES)
        {
            index++; // Skip shark distance question if they did not answer yes to seeing it
        }

        if (index == dolphinDistanceQIdx() &&
            DataLogger.Instance.getAnswer(QUESTION_DOLPHIN) != ANSWER_YES)
        {
            index++; // Skip dolphin distance question if they did not answer yes to seeing it
        }

        if (index >= canvasCount())
        {
            // End of the line
            closeAllCanvas();
            DataLogger.Instance.saveDataToFile();
            return;
        }

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
        CanvasStateManager.Instance.moveForward(ANSWER_NO);
    }

    public void yesButtonClicked()
    {
        CanvasStateManager.Instance.moveForward(ANSWER_YES);
    }

    public void dolphinDistanceContinueClicked()
    {
        CanvasStateManager.Instance.moveForward(dolphinDistanceSlider.value.ToString());
    }

    public void sharkDistanceContinueClicked()
    {
        CanvasStateManager.Instance.moveForward(sharkDistanceSlider.value.ToString());
    }
}