// Copyright  2015-2020 Pico Technology Co., Ltd. All Rights Reserved.


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayVisualizer : MonoBehaviour {
    private LineRenderer _line;
    private Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;
    
    void Start ()
    {
        _line = gameObject.GetComponent<LineRenderer>();
        _line.startWidth = 0.002f;
        _line.endWidth = 0.002f;
    }
	
    void Update ()
    {
        if (Application.isEditor)
        {
            return;
        }

        var t = Pvr_UnitySDKSensor.Instance.HeadPose.Matrix;
        Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
        _line.SetPosition(0, t.MultiplyPoint(new Vector3(0,-0.05f,0.2f)));
        _line.SetPosition(1, gazeRay.Origin + gazeRay.Direction * 20);

        /*
        var t = Pvr_UnitySDKSensor.Instance.HeadPose.Matrix;
        Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
        //Vector3(0.172999993,-0.959999979,26.8700008)
        Vector3 translation = Camera.main.transform.position +
            (Camera.main.transform.forward * 0.2f) +
            (Camera.main.transform.up * -0.5f);
        _line.SetPosition(0, t.MultiplyPoint(translation));
        _line.SetPosition(1, gazeRay.Origin + gazeRay.Direction * 20);*/
    }
}