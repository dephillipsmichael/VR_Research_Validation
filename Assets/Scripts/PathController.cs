using UnityEngine;

public class PathController : MonoBehaviour {

    [SerializeField]
    public GameObject node; //the player could instantiate these to create their own path

    //init this in Awake to avoid race condition with fish init in Start()
    void Awake() {
        AlignDirections();
        DisableRenderers();
    }

    void DisableRenderers() {
        foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
            rend.enabled = false;
        }
    }

    void AlignDirections() {
        for (int i = 0; i < transform.childCount; i++) {
            Transform currChild = transform.GetChild(i);
            int nextChildIndex = i < (transform.childCount - 1) ? i + 1 : 0;
            Transform nextChild = transform.GetChild(nextChildIndex);
            currChild.LookAt(nextChild);
        }
    }
}