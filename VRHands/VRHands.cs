﻿using UnityEngine;
using Valve.VR;

namespace SignOfSilenceVR
{
    public class VRHands : MonoBehaviour
    {

        public static GameObject RightHand = null;
        public static GameObject LeftHand = null;
        public static Transform parent;
        public static bool showPointer = false;
        public static bool showModel = false;

        private void Update()
        {
            SpawnHands();
        }

        public static void SpawnHands()
        {
            var parentUpdate = getCamera();
            if (parentUpdate && parentUpdate != parent)
            {
                parent = parentUpdate;
                createHands();
                attachLightToHand();
            }
        }

        public static Transform getCamera()
        {
            if (CameraManager.cameraParent)
            {
                showModel = false;
                showPointer = false;
                return CameraManager.cameraParent.transform;
            }
            if (Camera.main)
            {
                showPointer = true;
                showModel = true;
                return Camera.main.transform.parent.transform;
            }
            return null;
        }

        public static void createHands()
        {
            RightHand = Instantiate(AssetLoader.RightHandBase, Vector3.zeroVector,
                Quaternion.identityQuaternion);
            RightHand.transform.parent = parent;
            RightHand.transform.position = parent.position;
            RightHand.transform.localPosition = Vector3.zero;
            RightHand.transform.localRotation = Quaternion.identity;
            RightHand.transform.localScale = Vector3.one;
            if (showPointer) RightHand.AddComponent<Pointer>();
            RightHand.transform.Find("Model").gameObject.SetActive(showModel);

            LeftHand = Instantiate(AssetLoader.LeftHandBase, Vector3.zeroVector,
                Quaternion.identityQuaternion);
            LeftHand.transform.parent = parent;
            LeftHand.transform.position = parent.position;
            LeftHand.transform.localPosition = Vector3.zero;
            LeftHand.transform.localRotation = Quaternion.identity;
            LeftHand.transform.localScale = Vector3.one;
            //if (showPointer) LeftHand.AddComponent<Pointer>();
            LeftHand.transform.Find("Model").gameObject.SetActive(showModel);
        }

        public static void attachLightToHand()
        {
            if (CameraManager.cameraParent != null)
            {
                if(RightHand)
                {
                    //CameraManager.cameraParent.Find("Head")
                    //  .transform.Find("LightsItemInHand").transform.parent = RightHand.transform;
                    var spotlight = CameraManager.cameraParent.Find("Head")
                        .transform.Find("Spotlight").transform;
                    spotlight.parent = RightHand.transform;
                    spotlight.localPosition = Vector3.zero;
                    spotlight.eulerAngles = new Vector3(35, 0, 0);
                }
            }
        }

        public static void OnDestroy()
        {
            RightHand = null;
            LeftHand = null;
            parent = null;
        }
    }
}
