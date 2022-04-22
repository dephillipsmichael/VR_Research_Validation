using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;

public class DataLogger : Singleton<DataLogger>
{
    private Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;
    private Ray ray = new Ray();
    private RaycastHit hit;

    public GameObject eyesRaycastObj;

    public static string PARTICIPANT_ID = "ParticipantIdEntry";

    private string participantId = "";
    private QuestionData answers = new QuestionData();
    public GazeAtData gazetAt = new GazeAtData();
    public EyesAndHeadData eyesAndHeadData = new EyesAndHeadData();
    public FishAndSharkResultDataList fishPositionData = new FishAndSharkResultDataList();
    public EventTimestampDataList eventList = new EventTimestampDataList();

    private bool isTracking = false;

    private string rootPath()
    {
        return "/sdcard/Fish";
    }

    private string createRootDirIfApplicable()
    {
        string dirPath = rootPath();

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        return dirPath;
    }

    // Start is called before the first frame update
    void Start()
    {
        isTracking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTracking || Application.isEditor)
        {
            return;
        }

        float time = Time.time;

        // Check to see if the user's eyes are looking at any game object
        Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
        ray.origin = gazeRay.Origin;
        ray.direction = gazeRay.Direction;
        if (Physics.Raycast(ray, out hit))
        {
            gazetAt.data.Add(new GazeAtData.GazingAt(time, hit.transform.name));
        }

        // Track the head and eye direction
        eyesAndHeadData.data.Add(new EyesAndHeadData.EyesAndHead(time,
            Pvr_UnitySDKSensor.Instance.HeadPose.Position,
            Pvr_UnitySDKSensor.Instance.HeadPose.Orientation,
            ray.origin, ray.direction));
    }

    public void setAnswer(string identifier, string answer)
    {
        Debug.Log("Set " + identifier + " = " + answer);
        answers.data.Add(new QuestionData.Answer(identifier, answer));
        if (identifier == PARTICIPANT_ID)
        {
            participantId = answer;
        }
    }

    public void logEventTimestamp(String eventName, float timestamp) 
    {
        EventTimestampDataList.EventTimestampData data = new EventTimestampDataList.EventTimestampData();
        data.eventName = eventName;
        data.time = timestamp;
        eventList.eventList.Add(data);
    }

    public void logFishAndSharkData(float startTime, float endTime, string identifier, string startPosition, bool didEatFood)
    {
        FishAndSharkResultDataList.FishAndSharkResultData data = new FishAndSharkResultDataList.FishAndSharkResultData();
        data.startPosition = startPosition;
        data.startTime = startTime;
        data.endTime = endTime;
        data.identifier = identifier;
        data.didEatFood = didEatFood;
        fishPositionData.dataList.Add(data);
    }

    public void startTracking()
    {
        Debug.Log("Start tracking");
        eyesRaycastObj.SetActive(true);
        isTracking = true;
    }    

    public void stopTracking()
    {
        Debug.Log("Stop tracking");
        isTracking = false;
        eyesRaycastObj.SetActive(false);                
    }

    public void saveSettingsToAnswersFile()
    {
        float duration = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_DURATION);
        string durationText = (int)duration + " seconds";
        setAnswer(Settings.PLAYER_PREF_KEY_DURATION, durationText);

        float sharkTime = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_SHARK);
        string sharkTimeText = (int)sharkTime + " seconds";
        setAnswer(Settings.PLAYER_PREF_KEY_SHARK, sharkTimeText);

        float fishDensity = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_FISH_DENSITY);
        string fishDensityText = (int)fishDensity + " per minute";
        setAnswer(Settings.PLAYER_PREF_KEY_FISH_DENSITY, fishDensityText);

        float fishEatFoodPercentage = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_FISH_EAT_PERCENTAGE);
        string fishEatFoodPercentageText = (int)fishEatFoodPercentage + "%";
        setAnswer(Settings.PLAYER_PREF_KEY_FISH_EAT_PERCENTAGE, fishEatFoodPercentageText);

        string langStr = Settings.getPlayerPref(Settings.PLAYER_PREF_KEY_LANGUAGE, Settings.LANGUAGE_PREF_ENGLISH);
        setAnswer(Settings.PLAYER_PREF_KEY_LANGUAGE, langStr);
    }

    public void saveDataToFile()
    {
        Debug.Log("Saving data to file");

        string zipId = participantId;
        if (zipId == null)
        {
            zipId = "999999";
        }

        saveSettingsToAnswersFile();

        string dirPath = createRootDirIfApplicable();
        using (var fs = File.Create(dirPath + "/Participant" + zipId + ".zip"))
        using (var outStream = new ZipOutputStream(fs))
        {
            outStream.Password = "SDP";
            outStream.UseZip64 = UseZip64.Off;

            // Write answers CSV
            byte[] answersCsv = createAnswersCsv(answers);
            outStream.PutNextEntry(new ZipEntry("Answers.csv"));
            outStream.Write(answersCsv, 0, answersCsv.Length);

            // Write answers CSV
            byte[] gazeCsv = createGazeCsv(gazetAt);
            outStream.PutNextEntry(new ZipEntry("GazingAt.csv"));
            outStream.Write(gazeCsv, 0, gazeCsv.Length);

            // Write Events CSV
            byte[] eventsCsv = createTimestampEventsCsv(eventList);
            outStream.PutNextEntry(new ZipEntry("EventList.csv"));
            outStream.Write(eventsCsv, 0, eventsCsv.Length);

            // Write Fish And Shark Info CSV
            byte[] fishAndSharkInfoCsv = createFishAndSharkInfoCsv(fishPositionData);
            outStream.PutNextEntry(new ZipEntry("FishAndSharkInfo.csv"));
            outStream.Write(fishAndSharkInfoCsv, 0, fishAndSharkInfoCsv.Length);

            // Write Eyes and Head at JSON
            byte[] eyesAndHeadJson =
                System.Text.Encoding.UTF8.GetBytes(
                JsonUtility.ToJson(eyesAndHeadData));
            outStream.PutNextEntry(new ZipEntry("EyesAndHead.json"));
            outStream.Write(eyesAndHeadJson, 0, eyesAndHeadJson.Length);
        }      

        gazetAt.data.Clear();
        eyesAndHeadData.data.Clear();
        answers.data.Clear();
        eventList.eventList.Clear();
        fishPositionData.dataList.Clear();
    }

    private byte[] createAnswersCsv(QuestionData answers)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Identifier,Value\n");
        foreach (QuestionData.Answer answer in answers.data)
        {
            sb.Append(answer.identifier);
            sb.Append(",");
            sb.Append(answer.value);
            sb.Append("\n");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private byte[] createGazeCsv(GazeAtData gazes)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Time,Gazing At\n");
        foreach (GazeAtData.GazingAt gaze in gazes.data)
        {
            sb.Append(gaze.time);
            sb.Append(",");
            sb.Append(gaze.name);
            sb.Append("\n");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private byte[] createTimestampEventsCsv(EventTimestampDataList data)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Time,Event Name\n");
        foreach (EventTimestampDataList.EventTimestampData dataPoint in data.eventList)
        {
            sb.Append(dataPoint.time);
            sb.Append(",");
            sb.Append(dataPoint.eventName);
            sb.Append("\n");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private byte[] createFishAndSharkInfoCsv(FishAndSharkResultDataList data)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Identifier,Start Time,End Time,Start Position,Did Eat Food?\n");
        foreach (FishAndSharkResultDataList.FishAndSharkResultData dataPoint in data.dataList)
        {
            sb.Append(dataPoint.identifier);
            sb.Append(",");
            sb.Append(dataPoint.startTime);
            sb.Append(",");
            sb.Append(dataPoint.endTime);
            sb.Append(",");
            sb.Append(dataPoint.startPosition);
            sb.Append(",");
            sb.Append(dataPoint.didEatFood);
            sb.Append("\n");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    [Serializable]
    public class QuestionData
    {
        public List<Answer> data = new List<Answer>();

        [Serializable]
        public class Answer
        {
            public string identifier;
            public string value;

            public Answer(string id, string val)
            {
                identifier = id;
                value = val;
            }
        }
    }

    [Serializable]
    public class GazeAtData
    {
        public List<GazingAt> data = new List<GazingAt>();

        [Serializable]
        public class GazingAt
        {
            public float time;
            public string name;

            public GazingAt(float time, string name)
            {
                this.time = time;
                this.name = name;
            }
        }
    }

    [Serializable]
    public class EyesAndHeadData
    {
        public List<EyesAndHead> data = new List<EyesAndHead>();

        [Serializable]
        public class EyesAndHead
        {
            public float time;
            public Vector3 headPos;
            public Quaternion headDir;
            public Vector3 eyePos;
            public Vector3 eyeDir;

            public EyesAndHead(float time, 
                Vector3 headPosition, Quaternion headOrientation,
                Vector3 eyePosition, Vector3 eyeDirection)
            {
                this.time = time;
                this.headPos = headPosition;
                this.headDir = headOrientation;
                this.eyePos = eyePosition;
                this.eyeDir = eyeDirection;
            }
        }
    }

    [Serializable]
    public class EventTimestampDataList
    {
        public List<EventTimestampData> eventList = new List<EventTimestampData>();

        [Serializable]
        public class EventTimestampData {
            public float time;
            public string eventName;
        }
    }

    [Serializable] 
    public class FishAndSharkResultDataList
    {
        public List<FishAndSharkResultData> dataList =  new List<FishAndSharkResultData>();

        [Serializable]
        public class FishAndSharkResultData
        {
            public float startTime;
            public float endTime;
            public string identifier;
            public string startPosition;
            public bool didEatFood;
        }
    }
}
