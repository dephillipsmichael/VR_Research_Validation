using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BridgeClient: Singleton<BridgeClient>
{
    bool IsInitialized = false;
    AndroidJavaObject unityKotlin;

    [SerializeField]
    Text TitleText;

    [SerializeField]
    GameObject UIMenuParent;

    private void Awake()
    {

    }

    public void initialize()
    {
        if (IsInitialized)
        {
            return;
        }

        TitleText.text = "Please select your Participant ID";

        AndroidJavaClass ajc;
        AndroidJavaObject ajo, context;
        ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
        context = ajo.Call<AndroidJavaObject>("getApplicationContext");

        unityKotlin = new AndroidJavaObject("com.unity3d.player.UnityKotlin", new object[] { context });

        IsInitialized = true;
    }

    public void signIn()
    {
        initialize();
        unityKotlin.Call("signIn", new object[] { "MDPTest001" });
    }

    public void signInComplete(string messsage)
    {
        Debug.Log(" Unity has been clicked." + messsage);

        UIMenuParent.SetActive(false);
    }
}
