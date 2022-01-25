using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFade : MonoBehaviour {

    public bool defaultOff;

    readonly float fadeSpeed = 10f;

    public bool neverBlockRaycasts = false;

    float desiredAlpha;
    CanvasGroup canvasGroup;

    [SerializeField]
    List<UnityEvent> showScreenEventList = new List<UnityEvent>();

    [SerializeField]
    List<UnityEvent> closeScreenEventList = new List<UnityEvent>();

    void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        if (defaultOff) CloseScreen();
        else OpenScreen();
    }

    void ToggleScreen(bool active) {
        desiredAlpha = active ? 1 : 0;
        if (canvasGroup != null) {
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
        }
    }

    public bool IsOpen() {
        return desiredAlpha.Equals(1);
    }

    public void OpenScreen() {
        ToggleScreen(true);
        foreach(UnityEvent evt in showScreenEventList)
        {
            evt?.Invoke();
        }
    }

    public void CloseScreen(bool immediate = false) {
        ToggleScreen(false);
        if (immediate) {
            desiredAlpha = 0;
            canvasGroup.alpha = 0;
        }
        foreach (UnityEvent evt in closeScreenEventList)
        {
            evt?.Invoke();
        }
    }

    void Update() {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, desiredAlpha, Time.deltaTime * fadeSpeed);
    }
}
