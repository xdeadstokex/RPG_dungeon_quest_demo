using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.SystemUtils
{
    public class CameraUtils
    {
        public static void SetCameraParent(GameObject target, Camera cam)
        {
            float z = cam.transform.localPosition.z;
            cam.transform.SetParent(target.transform);
            cam.transform.localPosition = new Vector3(0f, 0f, z);
        }
    }
}
