using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FishBehavior : MonoBehaviour {

    const float SPEED = 1f;
    const float DISTANCE_CHECK = 2f;

    [SerializeField]
    Transform PathParent;

    ParticleSystem particleSys; //maybe use this to randomize stuff over time???
    ParticleSystemRenderer particleRenderer;


    Vector3 desiredPos;
    Quaternion desiredRot;

    int currNode = 0;

    void Start() {
        particleSys = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();
        SetDesiredNode();
    }

    void Update() {

        //interpolate to position of current node
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * SPEED);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, Time.deltaTime * SPEED);

        //get screen facing vector and flip texture accordingly to give illusion they are swiming in direction of movement
        AlignToDirection();

        //if close to current node, move to next one
        if (Vector3.Distance(transform.position, desiredPos) < DISTANCE_CHECK) {
            SetNextPosition();
        }
    }

    void SetDesiredNode() {
        desiredPos = PathParent.GetChild(currNode).position;
        desiredRot = PathParent.GetChild(currNode).rotation;
    }

    void SetNextPosition() {

        currNode++;

        if (currNode > PathParent.childCount - 1) {
            currNode = 0;
        }

        SetDesiredNode();
    }

    void AlignToDirection() {
        Vector3 currScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 desiredScreenPoint = Camera.main.WorldToScreenPoint(desiredPos);
        Vector3 movementDirection = desiredScreenPoint - currScreenPoint;
        Vector3 flip = Vector3.zero;
        flip.x = movementDirection.x > 0 ? 1 : 0;
        particleRenderer.flip = flip;
    }
}
