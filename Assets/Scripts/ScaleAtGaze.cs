// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using Tobii.G2OM;
using UnityEngine;

namespace Tobii.XR.Examples.DevTools
{
    //Monobehaviour which implements the "IGazeFocusable" interface, meaning it will be called on when the object receives focus
    public class ScaleAtGaze : MonoBehaviour, IGazeFocusable
    {
        public float scaleIncreaseOnGaze = 0.1f;
        private Vector3 originalScale;

        //The method of the "IGazeFocusable" interface, which will be called when this object receives or loses focus
        public void GazeFocusChanged(bool hasFocus)
        {
            //If this object received focus, fade the object's color to highlight color
            if (hasFocus)
            {
                transform.localScale = new Vector3(
                    originalScale.x + scaleIncreaseOnGaze,
                    originalScale.y + scaleIncreaseOnGaze,
                    originalScale.z + scaleIncreaseOnGaze);
            }
            //If this object lost focus, fade the object's color to it's original color
            else
            {
                transform.localScale = originalScale;
            }
        }

        private void Start()
        {
            originalScale = transform.localScale;
        }
    }
}