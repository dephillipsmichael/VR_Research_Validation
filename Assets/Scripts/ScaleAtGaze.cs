// Copyright  2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

using UnityEngine;

public class ScaleAtGaze : MonoBehaviour
{
    public float scaleIncrease = 1.5f;
    private Vector3 ogScale;
    public float AnimationTime = 0.1f;

    private Pvr_UnitySDKAPI.EyeTrackingGazeRay gazeRay;

    private void Start()
    {
        ogScale = transform.localScale;
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            return;
        }

        Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref gazeRay);
        Ray ray = new Ray(gazeRay.Origin, gazeRay.Direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.name == transform.name)
            {
                transform.localScale = ogScale * scaleIncrease;
            }
            else
            {
                transform.localScale = ogScale;
            }               

        }
        else
        {
            transform.localScale = ogScale;
        }
    }
}