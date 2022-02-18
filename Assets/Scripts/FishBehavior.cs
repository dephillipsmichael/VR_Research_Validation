using UnityEngine;
using System;
using System.Collections.Generic;

public class FishBehavior : Singleton<FishBehavior> {

    const float SWIM_SPEED = 1.25f;
    const float SPEED = 0.75f;
    const float DISTANCE_CHECK = 1f;

    const string SHARK_NAME = "Shark";
    const string FISH1_NAME = "OrangeFish";
    const string FISH2_NAME = "BlueFish";

    Vector3 behindLeftRock = new Vector3(-7.9599991f, -1.73000002f, 13.1630001f);    
    Vector3 behindRightRock = new Vector3(12.8900003f, -0.800000012f, 12.2980003f);
    Vector2 behindRockYRange = new Vector2(-3.0f, 0.0f);

    List<SimpleFishPath> availablePathList = new List<SimpleFishPath>();

    public GameObject sharkPrefab;

    public GameObject stripedPrefab;
    List<Fish> stripedFishList = new List<Fish>();
    List<GameObject> stripedObjList = new List<GameObject>();

    public GameObject bluePrefab;
    List<Fish> blueFishList = new List<Fish>();
    List<GameObject> blueObjList = new List<GameObject>();

    GameObject shark;
    Fish sharkFishObj;

    float sharkTime = 15.0f;

    float startTime = 0.0f;
    float startOffset = 0.0f;
    float durationInSeconds = 20.0f;
    float fishPerMinute = 60.0f;

    bool isDoneFishing = false;

    System.Random random;

    void Start() {
    }

    public void startFishin()
    {
        durationInSeconds = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_DURATION);
        sharkTime = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_SHARK);
        fishPerMinute = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_FISH_DENSITY);
        isDoneFishing = false;
        random = new System.Random();
        startTime = Time.time;
        CreateAvailableFishPaths();
        CreateSalmon();
        CreateTrout();
        CreateShark();
        FlakeManager.Instance.startFishin();
        DataLogger.Instance.logEventTimestamp("start", startTime);
    }

    void CreateAvailableFishPaths()
    {
        Debug.Log("CreateAvailableFishPaths");

        float yVal = -1;
        float scale = 1.25f;

        Vector3 top0 = FlakeManager.Instance.flakePile0.transform.position;
        top0.y = yVal;
        top0.y += scale;

        Vector3 bottom0 = FlakeManager.Instance.flakePile0.transform.position;
        bottom0.y = yVal;
        bottom0.y -= scale;

        Vector3 top1 = FlakeManager.Instance.flakePile1.transform.position;
        top1.y = yVal;
        top1.y += scale;

        Vector3 bottom1 = FlakeManager.Instance.flakePile1.transform.position;
        bottom1.y = yVal;
        bottom1.y -= scale;

        Vector3 top2 = FlakeManager.Instance.flakePile2.transform.position;
        top2.y = yVal;
        top2.y += scale;


        Vector3 bottom2 = FlakeManager.Instance.flakePile2.transform.position;
        bottom2.y = yVal;
        bottom2.y -= scale;


        availablePathList.Clear();
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindLeftRock, top0, top1, top2, behindRightRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindLeftRock, top0, top1, bottom2, behindRightRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindLeftRock, top0, bottom1, top2, behindRightRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindLeftRock, top0, bottom1, bottom2, behindRightRock }));
        
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindLeftRock, bottom0, top1, top2, behindRightRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindLeftRock, bottom0, top1, bottom2, behindRightRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindLeftRock, bottom0, bottom1, top2, behindRightRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindLeftRock, bottom0, bottom1, bottom2, behindRightRock }));

        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindRightRock, top2, top1, top0, behindLeftRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindRightRock, top2, top1, bottom0, behindLeftRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindRightRock, top2, bottom1, top0, behindLeftRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindRightRock, top2, bottom1, bottom0, behindLeftRock }));

        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindRightRock, bottom2, top1, top0, behindLeftRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindRightRock, bottom2, top1, bottom0, behindLeftRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindRightRock, bottom2, bottom1, top0, behindLeftRock }));
        availablePathList.Add(new SimpleFishPath(new List<Vector3>() {
            behindRightRock, bottom2, bottom1, bottom0, behindLeftRock }));
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

        if (isDoneFishing)
        {
            return;
        }

        float timeOffset = Time.time - startTime;        

        if (timeOffset > (durationInSeconds + 10.0 + startOffset)) 
        {
            DataLogger.Instance.logEventTimestamp("end", Time.time);
            CanvasStateManager.Instance.fishyTimeOver();
            isDoneFishing = true;           
            return;
        }

        if (timeOffset < startOffset)
        {
            return;
        }
        timeOffset = timeOffset - startOffset;

        for (int i = 0; i < stripedFishList.Count; i++)
        {
            Fish fish = stripedFishList[i];
            GameObject fishObj = stripedObjList[i];

            if (timeOffset > fish.spawnTime)
            {
                UpdateFish(fish, fishObj, timeOffset);
            }
        }

        for (int i = 0; i < blueFishList.Count; i++)
        {
            Fish fish = blueFishList[i];
            GameObject fishObj = blueObjList[i];

            if (timeOffset > fish.spawnTime)
            {
                UpdateFish(fish, fishObj, timeOffset);
            }
        }

        if (sharkFishObj != null && 
            timeOffset > sharkFishObj.spawnTime)
        {
            UpdateFish(sharkFishObj, shark, timeOffset);
        }
    }

    void UpdateFish(Fish fish, GameObject fishObj, float timeOffset)
    {
        if (fish.nodeIdx < fish.path.path.Count)
        {
            if (fish.nodeIdx == 0)
            {
                fishObj.SetActive(true);
                fish.nodeIdx = 1;
            }

            // Loop through and find closest next fish flake
            /*            float yTarget = -2f;
                        int closestIdx = 0;
                        float closestVal = 10000f;
                        float distance = 0f;
                        for (var i = 0; i < FlakeManager.Instance.flakeGameObjList.Count; i++)
                        {
                            distance = Vector3.Distance(fishObj.transform.position, FlakeManager.Instance.flakeGameObjList[i].transform.position);
                            if (distance < closestVal)
                            {
                                closestVal = distance;
                                closestIdx = i;
                            }
                        }
                        Vector3 desiredPos = FlakeManager.Instance.flakeGameObjList[closestIdx].transform.position;*/


            Vector3 desiredPos = fish.path.path[fish.nodeIdx];

            // Interpolate to position of current node
            fishObj.transform.position = Vector3.Lerp(fishObj.transform.position, desiredPos, Time.deltaTime * SWIM_SPEED);

            //if close to current node, move to next one
            if (Vector3.Distance(fishObj.transform.position, desiredPos) < DISTANCE_CHECK)
            {
                fish.nodeIdx += 1;
                if (fish.nodeIdx >= fish.path.path.Count)
                {
                    float fishStartTime = startTime + fish.spawnTime;
                    float fishEndTime = Time.time;
                    string fishStartingPos = "right";
                    if (fish.path.path[0].x < 0)
                    {
                        fishStartingPos = "left";
                    }
                    DataLogger.Instance.logFishAndSharkData(fishStartTime, fishEndTime, fishObj.name, fishStartingPos, fish.hasFood);
                }
            }

            fishObj.transform.LookAt(desiredPos);
            // The fish model has a 90 degree offset
            if (fish.rotationCorrection) 
            {
                fishObj.transform.rotation *= Quaternion.AngleAxis(90, transform.up);
            }            

            if (!fish.hasFood)
            {
                foreach (GameObject flake in FlakeManager.Instance.flakeGameObjList)
                {
                    float distanceToFood = Vector3.Distance(flake.transform.position, fishObj.transform.position);
                    if (distanceToFood < 0.5f)
                    {
                        Debug.Log("Fish got food");
                        fish.hasFood = true;
                        fishObj.transform.Find("Flake").gameObject.SetActive(true);
                    }
                }               
            }
        }
    }

    void CreateSalmon()
    {
        Debug.Log("CreateSalmon");
        blueFishList.Clear();

        float[] timeList = new[] { 2f, 4.2f, 6.15f, 8.5f, 10.33f, 12.15f, 14.46f, 16.67f, 18.2f, 19.9f };

        float time = 2f;
        float fishPerSecond = fishPerMinute / 60.0f;
        while (time < durationInSeconds)
        {  
            Fish fish = new Fish(time, randomPath());
            Vector3 startingPos = fish.path.path[0];
            blueFishList.Add(fish);
            GameObject fishObj = Instantiate(bluePrefab, startingPos, new Quaternion(0, 0, 0, 0));
            fishObj.name = FISH2_NAME + blueObjList.Count;
            fishObj.SetActive(false);
            fishObj.transform.Find("Flake").gameObject.SetActive(false);
            blueObjList.Add(fishObj);

            time = time + (1.0f / fishPerSecond) + (0.2f * (1.0f / fishPerSecond));
        }
    }

    void CreateShark()
    {
        Debug.Log("Create Shark");        
        float sharkTimeRandom = sharkTime + UnityEngine.Random.Range(-5.0f, 5.0f);
        sharkFishObj = new Fish(sharkTimeRandom, availablePathList[0]);
        sharkFishObj.rotationCorrection = false;
        shark = Instantiate(sharkPrefab, sharkFishObj.path.path[0], new Quaternion(0, 0, 0, 0));
        shark.name = SHARK_NAME;
        shark.SetActive(false);
        shark.transform.Find("Flake").gameObject.SetActive(false);
    }
    
    void CreateTrout()
    {
        Debug.Log("CreateTrout");
        stripedFishList.Clear();

        float time = 3f;
        float fishPerSecond = fishPerMinute / 60.0f;
        while (time < durationInSeconds)
        {
            Fish fish = new Fish(time, randomPath());
            stripedFishList.Add(fish);
            Vector3 startingPos = fish.path.path[0];
            GameObject fishObj = Instantiate(stripedPrefab, startingPos, new Quaternion(0, 0, 0, 0));
            fishObj.name = FISH1_NAME + stripedObjList.Count;
            fishObj.SetActive(false);
            fishObj.transform.Find("Flake").gameObject.SetActive(false);
            stripedObjList.Add(fishObj);

            time = time + (1.0f / fishPerSecond) + (0.2f * (1.0f / fishPerSecond));
        }
    }

    public class Fish
    {
        public SimpleFishPath path;
        public float spawnTime;
        public bool hasFood = false;
        public int nodeIdx = 0;
        public int flakeIdx = -1;
        public float flakeYOffset = 0f;
        public bool rotationCorrection = true;
        public int pileArray = 0;

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
