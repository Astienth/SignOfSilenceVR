using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;
using Donteco;

namespace SignOfSilenceVR
{
    public static class CameraManager
    {
        // VR Origin and body stuff
        public static Transform OriginalCameraParent = null;
        public static GameObject LocalPlayer = null;
        public static GameObject LeftHand = null;
        public static GameObject RightHand = null;

        // VR Input stuff
        public static bool RightHandGrab = false;
        public static bool LeftHandGrab = false;
        public static Vector2 LeftJoystick = Vector2.zero;
        public static Vector2 RightJoystick = Vector2.zero;

        // FIrst person camera stuff
        public static float Turnrate = 3f;

        public static void HandleFirstPersonCamera()
        {
            if (LocalPlayer != null)
            {
                if (LocalPlayer.GetComponent<PlayerMovementController>().IsCrouching)
                {
                    LocalPlayer.transform.Find("HEAD_HANDS").transform.localPosition = new Vector3(0, 0.4f, 0);
                }
                else
                {
                    LocalPlayer.transform.Find("HEAD_HANDS").transform.localPosition = new Vector3(0, 0.8f, 0);
                }

                //ROTATION
                //Vector3 RotationEulers = new Vector3(0, Turnrate * RightJoystick.x, 0);
                //VROrigin.transform.Rotate(RotationEulers);
            }
        }

        public static void SpawnHands()
        {
            if (!RightHand)
            {
                RightHand = GameObject.Instantiate(AssetLoader.RightHandBase, Vector3.zeroVector, Quaternion.identityQuaternion);
                RightHand.transform.parent = LocalPlayer.transform;
            }
            
            if (!LeftHand)
            {
                LeftHand = GameObject.Instantiate(AssetLoader.LeftHandBase, Vector3.zeroVector, Quaternion.identityQuaternion);
                LeftHand.transform.parent = LocalPlayer.transform;
            }
        }
    }

}