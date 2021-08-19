using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;

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

    public void saveDataToFile()
    {
        Debug.Log("Saving data to file");

        string zipId = participantId;
        if (zipId == null)
        {
            zipId = "999999";
        }

        string dirPath = createRootDirIfApplicable();
        using (var fs = File.Create(dirPath + "/Participant" + zipId + ".zip"))
        using (var outStream = new ZipOutputStream(fs))
        {
            outStream.Password = "SDP";
            outStream.UseZip64 = UseZip64.Off;

            // Write answers JSON
            byte[] answersJson = 
                System.Text.Encoding.UTF8.GetBytes(
                JsonUtility.ToJson(answers));

            outStream.PutNextEntry(new ZipEntry("Answers.json"));
            outStream.Write(answersJson, 0, answersJson.Length);

            // Write Gaze at JSON
            byte[] gazeJson =
                System.Text.Encoding.UTF8.GetBytes(
                JsonUtility.ToJson(gazetAt));

            outStream.PutNextEntry(new ZipEntry("GazingAt.json"));
            outStream.Write(gazeJson, 0, gazeJson.Length);

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
}
