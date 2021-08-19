using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NumberPadManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textField;

    [SerializeField]
    private Button continueButton;

    public int maxCharacters = 2;

    public void buttonPressed(string name)
    {
        if (textField.text == "_" || textField.text == "__")
        {
            textField.text = "";
        }

        if (name == "DEL")
        {
            int length = textField.text.Length;
            if (length == 0)
            {
                return;
            }
            textField.text = textField.text.Substring(0, length - 1);
        } else
        {
            if (textField.text.Length >= maxCharacters)
            {
                return;
            }
            textField.text += name;
        }

        continueButton.interactable = textField.text.Length > 0;
    }

    public void continueButtonClicked()
    {
        CanvasStateManager.Instance.moveForward(textField.text);
    }
}
