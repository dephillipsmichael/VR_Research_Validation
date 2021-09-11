using UnityEngine;
using System;
using System.Collections.Generic;

public class FlakeManager : Singleton<FlakeManager> {

    const float SPEED =2.0f;   

    const string SHARK_NAME = "Shark";

    [SerializeField]
    GameObject flakePile0;
    [SerializeField]
    GameObject flakePile1;
    [SerializeField]
    GameObject flakePile2;

    List<GameObject> flakeGameObjList = new List<GameObject>();
    List<Flake> flakeList = new List<Flake>();

    [SerializeField]
    public GameObject flakePrefab;

    float startTime = 0.0f;    
    float durationInSeconds = 20.0f;// 20 seconds * 60.0f; // 4 minutes

    bool isDoneFishing = false;

    System.Random random;

    void Start() {
    }

    public void startFishin()
    {
        isDoneFishing = false;
        random = new System.Random();
        startTime = Time.time;
        CreateFlakes();
    }

    void Update() {

        if (isDoneFishing)
        {
            return;
        }

        float timeOffset = Time.time - startTime;

        if (timeOffset > durationInSeconds + 5.0) 
        {
            isDoneFishing = true;
            return;
        }

        for (int i = 0; i < flakeList.Count; i++)
        {
            Flake flake = flakeList[i];
            GameObject flakeObj = flakeGameObjList[i];

            if (timeOffset > flake.spawnTime)
            {
                UpdateFlake(flake, flakeObj);
            }
        }
    }

    void UpdateFlake(Flake flake, GameObject flakeObj)
    {
        if (!flakeObj.activeSelf)
        {
            flakeObj.SetActive(true);
        }

        GameObject pile = GetFlakePile(flake.pileIndex);
        float sideSway = (Mathf.Sin(Time.time + flake.speedstartingSinAngle) * flake.sidesway);
        float changeInY = Time.deltaTime * SPEED;
        flakeObj.transform.position = new Vector3(pile.transform.position.x + sideSway, flakeObj.transform.position.y - changeInY, flakeObj.transform.position.z);
    }
    
    void CreateFlakes()
    {
        Debug.Log("Create Flakes");
        flakeList.Clear();
        for (float t = 3.0f; t < durationInSeconds; t += 1.0f)
        {          
            int pileIndex = random.Next(0, 3);
            int randomTime = random.Next(0, 3);
            float startingAngle = (float)random.NextDouble();
            float sideSway = random.Next(1, 4) * (0.25f / 4f);
            Flake flake = new Flake((float)(t + (randomTime * 0.25)), pileIndex, startingAngle, sideSway);
            flakeList.Add(flake);
            Vector3 startingPos = GetFlakePile(pileIndex).transform.position;
            startingPos.y = 7.0f;
            Quaternion startingRot = Quaternion.Euler(9.06400013f, 1.08999622f, 0.542997122f);
            GameObject flakeObj = Instantiate(flakePrefab, startingPos, startingRot);
            flakeObj.name = "FishFlake" + flakeGameObjList.Count;
            flakeObj.SetActive(false);
            flakeGameObjList.Add(flakeObj);
        }
    }

    GameObject GetFlakePile(int atIndex)
    {
        if (atIndex == 0)
        {
            return flakePile0;
        }
        else if (atIndex == 1)
        {
            return flakePile1;
        }
        else if (atIndex == 2)
        {
            return flakePile2;
        }
        return flakePile0;
    }

    public class Flake
    {
        public float sidesway = 0;
        public float speedstartingSinAngle = 0;
        public float spawnTime;
        public bool isEaten = false;
        public int pileIndex = 0;

        public Flake(float spawnTime, int pileIndex, float speedstartingSinAngle, float sidesway)
        {
            this.spawnTime = spawnTime;
            this.pileIndex = pileIndex;
            this.speedstartingSinAngle = speedstartingSinAngle;
            this.sidesway = sidesway;
        }
    }
}
