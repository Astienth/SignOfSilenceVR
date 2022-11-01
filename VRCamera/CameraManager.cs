using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;

namespace SignOfSilenceVR
{
    public static class CameraManager
    {
        // VR Origin and body stuff
        public static Transform OriginalCameraParent = null;
        public static GameObject VROrigin = new GameObject();
        public static GameObject LeftHand = null;
        public static GameObject RightHand = null;

        // VR Input stuff
        public static bool RightHandGrab = false;
        public static bool LeftHandGrab = false;
        public static Vector2 LeftJoystick = Vector2.zero;
        public static Vector2 RightJoystick = Vector2.zero;

        // FIrst person camera stuff
        public static float Turnrate = 3f;

        public static void SpawnHands()
        {
            if (!RightHand)
            {
                RightHand = GameObject.Instantiate(AssetLoader.RightHandBase, Vector3.zeroVector, Quaternion.identityQuaternion);
                RightHand.transform.parent = VROrigin.transform;
            }
            if (!LeftHand)
            {
                LeftHand = GameObject.Instantiate(AssetLoader.LeftHandBase, Vector3.zeroVector, Quaternion.identityQuaternion);
                LeftHand.transform.parent = VROrigin.transform;
            }
        }

        public static void HandleFirstPersonCamera()
        {
            // attach to head ?
        }
    }

}