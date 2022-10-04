using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class FishBehavior : Singleton<FishBehavior> {

    const float SWIM_SPEED = 1.25f;
    const float SPEED = 0.75f;
    const float DISTANCE_CHECK = 1f;

    public const string SHARK_NAME = "Shark";
    public const string DOLPHIN_NAME = "Dolphin";
    const string FISH1_NAME = "OrangeFish";
    const string FISH2_NAME = "BlueFish";

    Vector3 behindLeftRock = new Vector3(-9.9599991f, -1.73000002f, 13.1630001f);    
    Vector3 behindRightRock = new Vector3(12.8900003f, -0.800000012f, 12.2980003f);
    Vector2 behindRockYRange = new Vector2(-3.0f, 0.0f);

    // This JSON is the console log of the foodEatenComboList at the end of the simulation
    // It was used to find unique paths that have fish eat a fish flake 
    public static String foodEaterJson = "{\"list\":[{\"fishSpawnTimeFromStart\":4.199999809265137,\"fishPathIdx\":14,\"flakeSpawnTimeFromStart\":3.5,\"flakePileIdx\":1,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.24251915514469148},{\"fishSpawnTimeFromStart\":5.399999618530273,\"fishPathIdx\":4,\"flakeSpawnTimeFromStart\":5.25,\"flakePileIdx\":2,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.23158805072307588},{\"fishSpawnTimeFromStart\":6.59999942779541,\"fishPathIdx\":15,\"flakeSpawnTimeFromStart\":4.5,\"flakePileIdx\":2,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.1436949521303177},{\"fishSpawnTimeFromStart\":7.799999237060547,\"fishPathIdx\":12,\"flakeSpawnTimeFromStart\":6.25,\"flakePileIdx\":2,\"flakeSway\":0.125,\"flakeStartingAngle\":0.9237701892852783},{\"fishSpawnTimeFromStart\":7.999999523162842,\"fishPathIdx\":12,\"flakeSpawnTimeFromStart\":6.25,\"flakePileIdx\":2,\"flakeSway\":0.125,\"flakeStartingAngle\":0.9237701892852783},{\"fishSpawnTimeFromStart\":10.19999885559082,\"fishPathIdx\":1,\"flakeSpawnTimeFromStart\":8.5,\"flakePileIdx\":0,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.5976690649986267},{\"fishSpawnTimeFromStart\":10.399999618530274,\"fishPathIdx\":2,\"flakeSpawnTimeFromStart\":8.5,\"flakePileIdx\":0,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.5976690649986267},{\"fishSpawnTimeFromStart\":11.59999942779541,\"fishPathIdx\":11,\"flakeSpawnTimeFromStart\":10.5,\"flakePileIdx\":1,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.9597282409667969},{\"fishSpawnTimeFromStart\":12.799999237060547,\"fishPathIdx\":4,\"flakeSpawnTimeFromStart\":12.0,\"flakePileIdx\":1,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.35332489013671877},{\"fishSpawnTimeFromStart\":12.599998474121094,\"fishPathIdx\":12,\"flakeSpawnTimeFromStart\":12.0,\"flakePileIdx\":1,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.35332489013671877},{\"fishSpawnTimeFromStart\":14.999998092651368,\"fishPathIdx\":0,\"flakeSpawnTimeFromStart\":13.25,\"flakePileIdx\":0,\"flakeSway\":0.125,\"flakeStartingAngle\":0.6310779452323914},{\"fishSpawnTimeFromStart\":13.79999828338623,\"fishPathIdx\":15,\"flakeSpawnTimeFromStart\":12.0,\"flakePileIdx\":1,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.35332489013671877},{\"fishSpawnTimeFromStart\":15.19999885559082,\"fishPathIdx\":6,\"flakeSpawnTimeFromStart\":15.0,\"flakePileIdx\":2,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.054517194628715518},{\"fishSpawnTimeFromStart\":16.19999885559082,\"fishPathIdx\":3,\"flakeSpawnTimeFromStart\":15.0,\"flakePileIdx\":2,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.054517194628715518},{\"fishSpawnTimeFromStart\":16.399999618530275,\"fishPathIdx\":12,\"flakeSpawnTimeFromStart\":16.25,\"flakePileIdx\":1,\"flakeSway\":0.125,\"flakeStartingAngle\":0.6873485445976257},{\"fishSpawnTimeFromStart\":17.399999618530275,\"fishPathIdx\":4,\"flakeSpawnTimeFromStart\":16.25,\"flakePileIdx\":1,\"flakeSway\":0.125,\"flakeStartingAngle\":0.6873485445976257},{\"fishSpawnTimeFromStart\":18.80000114440918,\"fishPathIdx\":1,\"flakeSpawnTimeFromStart\":17.0,\"flakePileIdx\":0,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.12113483250141144},{\"fishSpawnTimeFromStart\":17.600000381469728,\"fishPathIdx\":10,\"flakeSpawnTimeFromStart\":18.0,\"flakePileIdx\":0,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.7818337082862854},{\"fishSpawnTimeFromStart\":21.000001907348634,\"fishPathIdx\":6,\"flakeSpawnTimeFromStart\":18.0,\"flakePileIdx\":0,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.7818337082862854},{\"fishSpawnTimeFromStart\":19.80000114440918,\"fishPathIdx\":14,\"flakeSpawnTimeFromStart\":19.0,\"flakePileIdx\":1,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.45267394185066225},{\"fishSpawnTimeFromStart\":22.40000343322754,\"fishPathIdx\":3,\"flakeSpawnTimeFromStart\":20.5,\"flakePileIdx\":0,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.24354830384254456},{\"fishSpawnTimeFromStart\":21.200002670288087,\"fishPathIdx\":11,\"flakeSpawnTimeFromStart\":20.5,\"flakePileIdx\":0,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.24354830384254456},{\"fishSpawnTimeFromStart\":23.40000343322754,\"fishPathIdx\":8,\"flakeSpawnTimeFromStart\":23.5,\"flakePileIdx\":0,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.278911828994751},{\"fishSpawnTimeFromStart\":24.800004959106447,\"fishPathIdx\":2,\"flakeSpawnTimeFromStart\":24.5,\"flakePileIdx\":2,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.6629438996315002},{\"fishSpawnTimeFromStart\":27.20000648498535,\"fishPathIdx\":13,\"flakeSpawnTimeFromStart\":25.25,\"flakePileIdx\":2,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.717400848865509},{\"fishSpawnTimeFromStart\":28.20000648498535,\"fishPathIdx\":15,\"flakeSpawnTimeFromStart\":26.0,\"flakePileIdx\":2,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.8005017638206482},{\"fishSpawnTimeFromStart\":28.400007247924806,\"fishPathIdx\":4,\"flakeSpawnTimeFromStart\":27.5,\"flakePileIdx\":1,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.09736627340316773},{\"fishSpawnTimeFromStart\":30.80000877380371,\"fishPathIdx\":1,\"flakeSpawnTimeFromStart\":29.0,\"flakePileIdx\":0,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.13191671669483186},{\"fishSpawnTimeFromStart\":31.80000877380371,\"fishPathIdx\":7,\"flakeSpawnTimeFromStart\":29.0,\"flakePileIdx\":0,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.13191671669483186},{\"fishSpawnTimeFromStart\":29.600008010864259,\"fishPathIdx\":9,\"flakeSpawnTimeFromStart\":29.0,\"flakePileIdx\":0,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.13191671669483186},{\"fishSpawnTimeFromStart\":34.200008392333987,\"fishPathIdx\":6,\"flakeSpawnTimeFromStart\":31.25,\"flakePileIdx\":0,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.7975878715515137},{\"fishSpawnTimeFromStart\":35.40000915527344,\"fishPathIdx\":0,\"flakeSpawnTimeFromStart\":34.0,\"flakePileIdx\":0,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.9967425465583801},{\"fishSpawnTimeFromStart\":37.800010681152347,\"fishPathIdx\":2,\"flakeSpawnTimeFromStart\":35.5,\"flakePileIdx\":0,\"flakeSway\":0.125,\"flakeStartingAngle\":0.17458947002887727},{\"fishSpawnTimeFromStart\":38.0000114440918,\"fishPathIdx\":13,\"flakeSpawnTimeFromStart\":36.0,\"flakePileIdx\":2,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.30688220262527468},{\"fishSpawnTimeFromStart\":39.0000114440918,\"fishPathIdx\":2,\"flakeSpawnTimeFromStart\":37.0,\"flakePileIdx\":0,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.008124755695462227},{\"fishSpawnTimeFromStart\":39.20001220703125,\"fishPathIdx\":3,\"flakeSpawnTimeFromStart\":37.0,\"flakePileIdx\":0,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.008124755695462227},{\"fishSpawnTimeFromStart\":40.4000129699707,\"fishPathIdx\":13,\"flakeSpawnTimeFromStart\":38.25,\"flakePileIdx\":2,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.2797335982322693},{\"fishSpawnTimeFromStart\":41.600013732910159,\"fishPathIdx\":0,\"flakeSpawnTimeFromStart\":40.5,\"flakePileIdx\":1,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.33569979667663576},{\"fishSpawnTimeFromStart\":42.80001449584961,\"fishPathIdx\":13,\"flakeSpawnTimeFromStart\":41.25,\"flakePileIdx\":2,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.25723662972450259},{\"fishSpawnTimeFromStart\":44.00001525878906,\"fishPathIdx\":11,\"flakeSpawnTimeFromStart\":43.25,\"flakePileIdx\":0,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.9618030190467835},{\"fishSpawnTimeFromStart\":47.40001678466797,\"fishPathIdx\":3,\"flakeSpawnTimeFromStart\":45.5,\"flakePileIdx\":0,\"flakeSway\":0.125,\"flakeStartingAngle\":0.720040500164032},{\"fishSpawnTimeFromStart\":46.40001678466797,\"fishPathIdx\":15,\"flakeSpawnTimeFromStart\":45.5,\"flakePileIdx\":0,\"flakeSway\":0.125,\"flakeStartingAngle\":0.720040500164032},{\"fishSpawnTimeFromStart\":48.60001754760742,\"fishPathIdx\":1,\"flakeSpawnTimeFromStart\":47.0,\"flakePileIdx\":0,\"flakeSway\":0.1875,\"flakeStartingAngle\":0.17414875328540803},{\"fishSpawnTimeFromStart\":50.00001907348633,\"fishPathIdx\":5,\"flakeSpawnTimeFromStart\":49.0,\"flakePileIdx\":1,\"flakeSway\":0.125,\"flakeStartingAngle\":0.6174574494361877},{\"fishSpawnTimeFromStart\":49.800018310546878,\"fishPathIdx\":2,\"flakeSpawnTimeFromStart\":49.0,\"flakePileIdx\":1,\"flakeSway\":0.125,\"flakeStartingAngle\":0.6174574494361877},{\"fishSpawnTimeFromStart\":51.20001983642578,\"fishPathIdx\":12,\"flakeSpawnTimeFromStart\":51.0,\"flakePileIdx\":1,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.6307363510131836},{\"fishSpawnTimeFromStart\":52.400020599365237,\"fishPathIdx\":12,\"flakeSpawnTimeFromStart\":50.25,\"flakePileIdx\":2,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.6975977420806885},{\"fishSpawnTimeFromStart\":52.20001983642578,\"fishPathIdx\":6,\"flakeSpawnTimeFromStart\":51.0,\"flakePileIdx\":1,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.6307363510131836},{\"fishSpawnTimeFromStart\":53.60002136230469,\"fishPathIdx\":5,\"flakeSpawnTimeFromStart\":52.25,\"flakePileIdx\":1,\"flakeSway\":0.125,\"flakeStartingAngle\":0.26762115955352785},{\"fishSpawnTimeFromStart\":53.400020599365237,\"fishPathIdx\":12,\"flakeSpawnTimeFromStart\":53.5,\"flakePileIdx\":0,\"flakeSway\":0.125,\"flakeStartingAngle\":0.39637434482574465},{\"fishSpawnTimeFromStart\":55.80002212524414,\"fishPathIdx\":1,\"flakeSpawnTimeFromStart\":55.0,\"flakePileIdx\":2,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.6196321845054627},{\"fishSpawnTimeFromStart\":57.20002365112305,\"fishPathIdx\":14,\"flakeSpawnTimeFromStart\":55.0,\"flakePileIdx\":2,\"flakeSway\":0.0625,\"flakeStartingAngle\":0.6196321845054627},{\"fishSpawnTimeFromStart\":56.000022888183597,\"fishPathIdx\":14,\"flakeSpawnTimeFromStart\":56.0,\"flakePileIdx\":0,\"flakeSway\":0.125,\"flakeStartingAngle\":0.7929339408874512},{\"fishSpawnTimeFromStart\":57.000022888183597,\"fishPathIdx\":10,\"flakeSpawnTimeFromStart\":57.25,\"flakePileIdx\":0,\"flakeSway\":0.125,\"flakeStartingAngle\":0.3965196907520294}]}";
    SavedFishFlakeComboList fishThatEatFoodList = new SavedFishFlakeComboList();
    List<SimpleFishPath> availablePathList = new List<SimpleFishPath>();

    public GameObject sharkPrefab;
    public GameObject dolphinPrefab;

    public GameObject stripedPrefab;
    List<Fish> stripedFishList = new List<Fish>();
    List<GameObject> stripedObjList = new List<GameObject>();

    public GameObject bluePrefab;
    List<Fish> blueFishList = new List<Fish>();
    List<GameObject> blueObjList = new List<GameObject>();

    GameObject shark;
    Fish sharkFishObj;

    GameObject dolphin;
    Fish dolphinFishObj;

    float sharkTime = 15.0f;

    float startTime = 0.0f;
    float startOffset = 0.0f;
    float durationInSeconds = 20.0f;
    float fishPerMinute = 60.0f;

    bool hasStartedFishing = false;
    bool isDoneFishing = false;

    System.Random random;

    public SavedFishFlakeComboList foodEatenComboList = new SavedFishFlakeComboList();
    int foodEaten = 0;

    void Start() {
        CreateAvailableFishPaths();
        random = new System.Random();
        testFishListCreation();
    }

    public void startFishin()
    {
        durationInSeconds = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_DURATION);
        sharkTime = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_SHARK);
        fishPerMinute = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_FISH_DENSITY);
        hasStartedFishing = true;
        isDoneFishing = false;
        random = new System.Random();
        startTime = Time.time;
        foodEatenComboList.list = new List<SavedFishFlakeCombo>();
        foodEaten = 0;
        CreateBlueFishList();
        CreateStripedFishList();
        CreateSharkAndDolphin();
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

        fishThatEatFoodList = JsonUtility.FromJson<SavedFishFlakeComboList>(foodEaterJson);        
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

    SavedFishFlakeCombo randomFishThatEats()
    {
        if (fishThatEatFoodList.list.Count == 0)
        {
            return null;
        }
        return fishThatEatFoodList.list[random.Next(fishThatEatFoodList.list.Count)];
    }

    int randomPathIdx()
    {
        if (availablePathList.Count == 0)
        {
            return 0;
        }
        return random.Next(availablePathList.Count);
    }

    void Update() {

        if (isDoneFishing || !hasStartedFishing)
        {
            return;
        }

        float timeOffset = Time.time - startTime;        

        if (timeOffset > (durationInSeconds + 10.0 + startOffset)) 
        {
            DataLogger.Instance.logEventTimestamp("end", Time.time);
            CanvasStateManager.Instance.fishyTimeOver();
            isDoneFishing = true;

                        
            String json = JsonUtility.ToJson(foodEatenComboList);
            Debug.Log(json);
            Debug.Log("Fish eaten percentage = " + (float)foodEaten / (float)(stripedFishList.Count + blueFishList.Count));

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

        if (dolphinFishObj != null &&
            timeOffset > dolphinFishObj.spawnTime)
        {
            UpdateFish(dolphinFishObj, dolphin, timeOffset);
        }
    }

    void UpdateFish(Fish fish, GameObject fishObj, float timeOffset)
    {
        SimpleFishPath path = availablePathList[fish.availablePathIdx];
        if (fish.nodeIdx < path.path.Count)
        {
            if (fish.nodeIdx == 0)
            {
                fishObj.SetActive(true);
                fish.nodeIdx = 1;
            }

            Vector3 desiredPos = path.path[fish.nodeIdx];

            // Interpolate to position of current node
            fishObj.transform.position = Vector3.Lerp(fishObj.transform.position, desiredPos, Time.deltaTime * SWIM_SPEED);

            //if close to current node, move to next one
            if (Vector3.Distance(fishObj.transform.position, desiredPos) < DISTANCE_CHECK)
            {
                fish.nodeIdx += 1;
                if (fish.nodeIdx >= path.path.Count)
                {
                    fishObj.SetActive(false);
                    float fishStartTime = startTime + fish.spawnTime;
                    float fishEndTime = Time.time;
                    string fishStartingPos = "right";
                    if (path.path[0].x < 0)
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

            if (!fish.hasFood && fish.name != SHARK_NAME && fish.name != DOLPHIN_NAME && fish.shouldEatFood)
            {
                for (int i = 0; i < FlakeManager.Instance.flakeGameObjList.Count; i++)
                {
                    GameObject flake = FlakeManager.Instance.flakeGameObjList[i];
                    float distanceToFood = Vector3.Distance(flake.transform.position, fishObj.transform.position);
                    if (distanceToFood < 0.5f)
                    {
                        Debug.Log("Fish got food");
                        fish.hasFood = true;
                        fishObj.transform.Find("Flake").gameObject.SetActive(true);
                        fishObj.GetComponent<AudioSource>().Play();

                        foodEaten++;
                        FlakeManager.Flake flakeEaten = FlakeManager.Instance.flakeList[i];
                        SavedFishFlakeCombo combo = new SavedFishFlakeCombo(fish.spawnTime, flakeEaten.spawnTime, fish.availablePathIdx, 
                            flakeEaten.pileIndex, flakeEaten.startingSidesway, flakeEaten.speedstartingSinAngle);
                        foodEatenComboList.list.Add(combo);
                    }
                }          
            }
        }
    }

    void CreateFoodEater(string fishName, GameObject fishPrefab, List<Fish> fishListToAddTo, List<GameObject> fishGameObjectListToAddTo, float time)
    {
        Debug.Log("CreateFoodEaters");

        FlakeManager manager = FlakeManager.Instance;
        SavedFishFlakeCombo fishThatEats = randomFishThatEats();
        float timeDiffForFlake = fishThatEats.flakeSpawnTimeFromStart - fishThatEats.fishSpawnTimeFromStart;
        Fish fish = new Fish(time, fishThatEats.fishPathIdx, true);
        float flakeTime = time + timeDiffForFlake;

        int pileIndex = fishThatEats.flakePileIdx;
        FlakeManager.Flake flake = new FlakeManager.Flake(
            flakeTime, pileIndex, manager.randomStartingAngle(), manager.randomSidesway());
        manager.flakeList.Add(flake);

        Vector3 flakeStartingPos = manager.GetFlakePile(pileIndex).transform.position;
        flakeStartingPos.y = 7.0f;
        Quaternion startingRot = Quaternion.Euler(9.06400013f, 1.08999622f, 0.542997122f);
        GameObject flakeObj = Instantiate(manager.flakePrefab, flakeStartingPos, startingRot);
        flakeObj.name = "FishFlakeToEat" + manager.flakeGameObjList.Count;
        flakeObj.SetActive(false);
        manager.flakeGameObjList.Add(flakeObj);

        Vector3 startingPos = availablePathList[fish.availablePathIdx].path[0];
        fishListToAddTo.Add(fish);
        GameObject fishObj = Instantiate(fishPrefab, startingPos, new Quaternion(0, 0, 0, 0));
        fish.name = fishName + fishGameObjectListToAddTo.Count;
        fishObj.name = fish.name;
        fishObj.SetActive(false);
        fishObj.transform.Find("Flake").gameObject.SetActive(false);
        fishGameObjectListToAddTo.Add(fishObj);
    }

    void CreateNonFoodEater(string fishName, GameObject fishPrefab, List<Fish> fishListToAddTo, List<GameObject> fishGameObjectListToAddTo, float time)
    {
        Fish fish = new Fish(time, randomPathIdx(), false);
        Vector3 startingPos = availablePathList[fish.availablePathIdx].path[0];
        fishListToAddTo.Add(fish);
        GameObject fishObj = Instantiate(fishPrefab, startingPos, new Quaternion(0, 0, 0, 0));
        fish.name = fishName + fishGameObjectListToAddTo.Count;
        fishObj.name = fish.name;
        fishObj.SetActive(false);
        fishObj.transform.Find("Flake").gameObject.SetActive(false);
        fishGameObjectListToAddTo.Add(fishObj);
    }

    public void testFishListCreation()
    {
        List<Dictionary<float, bool>> list = new List<Dictionary<float, bool>>();
        for (int i = 0;  i < 10; i ++)
        {
            list.Add(createFishList(2f));
        }

        Debug.Log("Testing create fish function");
        for (int k = 0; k < 10; k++)
        {
            Dictionary<float, bool> kList = list[k];            
            string logKListKeys = "";
            string logKListValues = "";
            foreach (var pair in kList)
            {
                logKListKeys += pair.Key + ", ";
                logKListValues += pair.Value + ", ";
            }
            Debug.Log("Testing Keys " + logKListKeys);
            Debug.Log("Testing Values " + logKListValues);
            for (int j = 0; j < 10; j++)
            {                
                Dictionary<float, bool> jList = list[j];
                if (kList.Count != jList.Count)
                {
                    Debug.Log("COUNTS ARE DIFFF ERRRROR");
                }
            }
        }
    }

    // Returns a dictionary of fish start times, and value of if they eat food or not
    Dictionary<float, bool> createFishList(float startTime)
    {
        Dictionary<float, bool> fishDict = new Dictionary<float, bool>();        

        float time = startTime;
        float fishPerSecond = fishPerMinute / 60.0f;
        while (time < durationInSeconds)
        {
            // Add all the times with no food eater status
            fishDict.Add(time, false);
            time = time + (1.0f / fishPerSecond) + (0.2f * (1.0f / fishPerSecond));
        }

        // The requirement is to have the same number of fish total and same number of fish that eat food
        // However, we want their paths, and which fish actually eat the food to be random
        // To accomplish this, only randomize (shuffle) the status of if a fish is a food eater or not
        float percentageOfFoodEaters = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_FISH_EAT_PERCENTAGE);
        int fishCount = fishDict.Keys.Count;
        int numberOfFishEatingFood = (int)((percentageOfFoodEaters / 100f) * fishCount);
        List<bool> foodEaterStatusList = new List<bool>();
        for (int fishIdx = 0; fishIdx < fishCount; fishIdx++)
        {
            foodEaterStatusList.Add(fishIdx < numberOfFishEatingFood);
        }

        ShuffleList(foodEaterStatusList);

        int i = 0;
        float[] keys = fishDict.Keys.ToArray();
        foreach(float key in keys)
        {
            fishDict[key] = foodEaterStatusList[i];
            i++;
        }

        return fishDict;
    }
    private void ShuffleList(List<bool> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            bool value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void CreateBlueFishList()
    {
        Debug.Log("CreateBlueFish");
        blueFishList.Clear();

        // Start blue fish 1 second before the striped fish start
        Dictionary<float, bool> fishDict = createFishList(2f);
        foreach(float startTime in fishDict.Keys)
        {
            if (fishDict[startTime])
            {
                CreateFoodEater(FISH2_NAME, bluePrefab, blueFishList, blueObjList, startTime);
            }
            else
            {
                CreateNonFoodEater(FISH2_NAME, bluePrefab, blueFishList, blueObjList, startTime);
            }
        }
    }

    void CreateStripedFishList()
    {
        Debug.Log("CreateStripedFish");
        stripedFishList.Clear();

        // Start striped fish 1 second after the blue fish start
        Dictionary<float, bool> fishDict = createFishList(3f);
        foreach (float startTime in fishDict.Keys)
        {
            if (fishDict[startTime])
            {
                CreateFoodEater(FISH1_NAME, stripedPrefab, stripedFishList, stripedObjList, startTime);
            }
            else
            {
                CreateNonFoodEater(FISH1_NAME, stripedPrefab, stripedFishList, stripedObjList, startTime);
            }
        }
    }

    void CreateSharkAndDolphin()
    {
        Debug.Log("Create Shark and Dolphin");

        // Create Shark start time       
        float sharkTimeRandom = sharkTime + UnityEngine.Random.Range(-5.0f, 5.0f);
        // Create dolphin start time
        float randomInterval = UnityEngine.Random.Range(1.0f, 5.0f);
        // 50% chance of dolphin appearing before the shark
        if ((int)UnityEngine.Random.Range(0, 2) == 0)
        {
            randomInterval = -randomInterval;
        }
        float dolphinTimeRandom = sharkTimeRandom + randomInterval;

        // Create Shark obj
        sharkFishObj = new Fish(sharkTimeRandom, randomPathIdx(), false);
        Vector3 startingPos = availablePathList[sharkFishObj.availablePathIdx].path[0];
        sharkFishObj.rotationCorrection = false;

        shark = Instantiate(sharkPrefab, startingPos, new Quaternion(0, 0, 0, 0));
        
        sharkFishObj.name = SHARK_NAME;
        shark.name = sharkFishObj.name;
        shark.SetActive(false);
        shark.transform.Find("Flake").gameObject.SetActive(false);

        // Create dolphin obj
        dolphinFishObj = new Fish(dolphinTimeRandom, randomPathIdx(), false);
        Vector3 startingPosDolphin = availablePathList[dolphinFishObj.availablePathIdx].path[0];
        dolphinFishObj.rotationCorrection = false;

        dolphin = Instantiate(dolphinPrefab, startingPosDolphin, new Quaternion(0, 0, 0, 0));

        dolphinFishObj.name = DOLPHIN_NAME;
        dolphin.name = dolphinFishObj.name;
        dolphin.SetActive(false);
    }   

    
    public class Fish
    {
        public int availablePathIdx;
        public float spawnTime;
        public bool hasFood = false;
        public int nodeIdx = 0;
        public int flakeIdx = -1;
        public float flakeYOffset = 0f;
        public bool rotationCorrection = true;
        public int pileArray = 0;
        public string name = "";
        public bool shouldEatFood = false;

        public Fish(float spawnTime, int pathIdx, bool willEatFood)
        {
            this.spawnTime = spawnTime;
            this.availablePathIdx = pathIdx;
            this.shouldEatFood = willEatFood;
        }
    }

    [Serializable]
    public class SimpleFishPath
    {
        public List<Vector3> path = new List<Vector3>();
        public SimpleFishPath(List<Vector3> pathList)
        {
            this.path = pathList;
        }
    }

    [Serializable]
    public class SavedFishFlakeComboList
    {
        public List<SavedFishFlakeCombo> list;        
    }

    [Serializable]
    public class SavedFishFlakeCombo
    {
        public float fishSpawnTimeFromStart;
        public int fishPathIdx;
        public float flakeSpawnTimeFromStart;
        public int flakePileIdx;
        public float flakeSway;
        public float flakeStartingAngle;

        public SavedFishFlakeCombo(float fishStart, float flakeStart, int pathIdx, int pileIdx, float savedFlakeSway, float savedFlakeStartingAngle)
        {
            fishSpawnTimeFromStart = fishStart;
            flakeSpawnTimeFromStart = flakeStart;
            fishPathIdx = pathIdx;
            flakePileIdx = pileIdx;
            flakeSway = savedFlakeSway;
            flakeStartingAngle = savedFlakeStartingAngle;
        }
    }    
}
