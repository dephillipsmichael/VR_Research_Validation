using UnityEngine;

public class FishBehavior : MonoBehaviour {

    const float SPEED = 0.5f;
    const float DISTANCE_CHECK = 2f;

    [SerializeField]
    Transform PathParent;

    Vector3 desiredPos;

    int currNode = 0;

    void Start() {
        SetDesiredNode();
    }

    void Update() {
        // Interpolate to position of current node
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * SPEED);

        //if close to current node, move to next one
        if (Vector3.Distance(transform.position, desiredPos) < DISTANCE_CHECK) {
            SetNextPosition();            
        }

        transform.LookAt(desiredPos);
        // The fish model has a 90 degree offset
        transform.rotation *= Quaternion.AngleAxis(90, transform.up);
    }

    void SetDesiredNode() {
        desiredPos = PathParent.GetChild(currNode).position;
    }

    void SetNextPosition() {

        currNode++;

        if (currNode > PathParent.childCount - 1) {
            currNode = 0;
        }

        SetDesiredNode();
    }
}
