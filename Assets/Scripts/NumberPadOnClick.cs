using UnityEngine;
using UnityEngine.UI;

public class NumberPadOnClick : MonoBehaviour
{
    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(buttonClicked);
    }

    void buttonClicked()
    {
        transform.parent.GetComponent<NumberPadManager>().buttonPressed(gameObject.name);
    }
}
