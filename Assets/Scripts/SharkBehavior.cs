using UnityEngine;
using System;
using System.Collections.Generic;

public class SharkBehavior : MonoBehaviour {

    const float SPEED = 0.5f;
    const float DISTANCE_CHECK = 2f;

    [SerializeField]
    GameObject fishFood;

    Vector3 behindRightRock = new Vector3(-10.9599991f, -1.73000002f, 13.1630001f);    
    Vector3 behindLeftRock = new Vector3(14.8900003f, -0.800000012f, 12.2980003f);
    Vector2 behindRockYRange = new Vector2(-3.0f, 0.0f);

    List<SimpleFishPath> availablePathList = new List<SimpleFishPath>();
    
    public GameObject sharkPrefab;
    List<Fish> sharkFishList = new List<Fish>();
    List<GameObject> sharkObjList = new List<GameObject>();

    float startTime = 0.0f;
    float durationInSeconds = 20.0f;// 20 seconds * 60.0f; // 4 minutes

    System.Random random;

    void Start() {
        random = new System.Random();
        startTime = Time.time;
        CreateAvailableFishPaths();
        CreateSharks();
    }

    void CreateAvailableFishPaths()
    {
        if (fishFood == null)
        {
            return;
        }

        Debug.Log("CreateAvailableFishPaths");

        float scale = 1.25f;

        Vector3 topLeft = fishFood.transform.position;
        topLeft.y += scale;
        topLeft.x += scale;

        Vector3 topRight = fishFood.transform.position;
        topRight.y += scale;
        topRight.x -= scale;

        Vector3 bottomLeft = fishFood.transform.position;
        bottomLeft.y -= scale;
        bottomLeft.x -= scale;

        Vector3 bottomRight = fishFood.transform.position;
        bottomRight.y -= scale;
        bottomRight.x -= scale;


        availablePathList.Clear();
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindLeftRock, bottomLeft, bottomRight, behindRightRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindLeftRock, bottomLeft, topRight, behindRightRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindLeftRock, topLeft, bottomRight, behindRightRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindLeftRock, topLeft, topRight, behindRightRock }));

        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindRightRock, bottomRight, bottomLeft, behindLeftRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindRightRock, topRight, bottomLeft, behindLeftRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindRightRock, bottomRight, topLeft, behindLeftRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindRightRock, topRight, topLeft, behindLeftRock }));
    }

    SimpleFishPath randomPath()
    {
        if (availablePathList.Count == 0)
        {
            return null;
        }
        int randomIdx = random.Next(availablePathList.Count);
        return availablePathList[randomIdx];
    }

    void Update() {

        float timeOffset = Time.time - startTime;
        for (int i = 0; i < sharkFishList.Count; i++)
        {
            Fish fish = sharkFishList[i];
            GameObject fishObj = sharkObjList[i];

            if (timeOffset > fish.spawnTime)
            {
                UpdateFish(fish, fishObj);
            }
        }
    }

    void UpdateFish(Fish fish, GameObject fishObj)
    {
        if (fish.nodeIdx < fish.path.path.Count)
        {
            if (fish.nodeIdx == 0)
            {
                fishObj.SetActive(true);
                fish.nodeIdx = 1;
            }

            Vector3 desiredPos = fish.path.path[fish.nodeIdx];

            // Interpolate to position of current node
            fishObj.transform.position = Vector3.Lerp(fishObj.transform.position, desiredPos, Time.deltaTime * SPEED);

            //if close to current node, move to next one
            if (Vector3.Distance(fishObj.transform.position, desiredPos) < DISTANCE_CHECK)
            {
                fish.nodeIdx += 1;
            }

            fishObj.transform.LookAt(desiredPos);

            if (!fish.hasFood)
            {
                float distanceToFood = Vector3.Distance(fishFood.transform.position, fishObj.transform.position);
                if (distanceToFood < 1.0f)
                {
                    Debug.Log("Fish got food");
                    fish.hasFood = true;
                    fishObj.transform.Find("Flake").gameObject.SetActive(true);
                }
            }
        }
    }
    
    void CreateSharks()
    {
        Debug.Log("CreateShark");
        sharkFishList.Clear();
        for (float t = 10.0f; t < durationInSeconds; t += durationInSeconds)
        {
            Fish fish = new Fish(t, randomPath());
            sharkFishList.Add(fish);
            Vector3 startingPos = fish.path.path[0];
            GameObject fishObj = Instantiate(sharkPrefab, startingPos, new Quaternion(0, 0, 0, 0));
            fishObj.SetActive(false);
            fishObj.transform.Find("Flake").gameObject.SetActive(false);
            sharkObjList.Add(fishObj);
        }
    }

    public class Fish
    {
        public SimpleFishPath path;
        public float spawnTime;
        public bool hasFood = false;
        public int nodeIdx = 0;

        public Fish(float spawnTime, SimpleFishPath path)
        {
            this.spawnTime = spawnTime;
            this.path = path;
        }
    }

    public class SimpleFishPath
    {
        public List<Vector3> path = new List<Vector3>();
        public SimpleFishPath(List<Vector3> pathList)
        {
            this.path = pathList;
        }
    }
}
