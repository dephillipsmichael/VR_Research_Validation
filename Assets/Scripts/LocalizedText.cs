using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour
{ 
    [SerializeField]
    public string localizationKey;

    void Awake()
    {
        localize();   
    }

    public void localize()
    {
        if (localizationKey == null)
        {
            return;
        }

        TextMeshProUGUI proText = GetComponent<TextMeshProUGUI>();
        if (proText != null)
        {
            proText.text = Settings.getLocalizedString(localizationKey);
        }
        else
        {
            Text basicText = GetComponent<Text>();
            if (basicText != null) 
            {
                basicText.text = Settings.getLocalizedString(localizationKey);
            }            
        }        
    }
}
