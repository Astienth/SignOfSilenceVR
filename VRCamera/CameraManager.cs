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
    public class CameraManager : MonoBehaviour
    {
        // VR Origin and body stuff
        public static GameObject LocalPlayer = null;
        public static GameObject LeftHand = null;
        public static GameObject RightHand = null;
        public static Camera playerCamera = null;
        public static Transform cameraParent;
        private static Transform playerHead = null;
        private static Vector3 camLocPos = new Vector3(0,0.8f,0.2f);

        // VR Input stuff
        public static bool RightHandGrab = false;
        public static bool LeftHandGrab = false;
        public static Vector2 LeftJoystick = Vector2.zero;
        public static Vector2 RightJoystick = Vector2.zero;

        // FIrst person camera stuff
        public static float Turnrate = 3f;

        private void Start()
        {
            findPlayer();
            if (LocalPlayer)
            {
                setPlayerHead();
                AdjustPlayerHeadPosition();
                SetupCamera();
            }
        }

        internal void Update()
        {
            if (LocalPlayer)
            {
                var cameraToHead = Vector3.ProjectOnPlane(LocalPlayer.transform.position
                 - playerCamera.transform.position, LocalPlayer.transform.up);

                if (cameraToHead.sqrMagnitude > 0.5f || cameraToHead.sqrMagnitude > 10f)
                {
                    MoveCameraToPlayerHead();
                }
            }
            else
            {
                findPlayer();
                if (LocalPlayer)
                {
                    setPlayerHead();
                    AdjustPlayerHeadPosition();
                    SetupCamera();
                }
            }
        }

        private static void AdjustPlayerHeadPosition()
        {
            //playerHead.localPosition = new Vector3(playerHead.localPosition.x, playerHead.localPosition.y, 0);
        }

        public void SetupCamera()
        {
            // Make an empty parent object for moving the camera around.
            playerCamera = findOriginCamera();
            if (playerCamera)
            {
                cameraParent = new GameObject("VrCameraParent").transform;
                cameraParent.parent = LocalPlayer.transform;
                cameraParent.position = playerHead.position;
                cameraParent.rotation = playerHead.rotation;
                cameraParent.localRotation = Quaternion.identity;
                cameraParent.localPosition = Vector3.zero;
                cameraParent.localPosition = camLocPos;
                playerCamera.transform.parent = cameraParent;
            }
        }

        public static void MoveCameraToPlayerHead()
        {
            cameraParent.position += cameraParent.position - cameraParent.Find("Head").transform.position;
        }

        private void setPlayerHead()
        {
            if (playerHead == null)
            {
                playerHead = LocalPlayer.transform.Find("HEAD_HANDS").transform;
            }
        }

        private Camera findOriginCamera()
        {
            if (LocalPlayer)
            {
                return LocalPlayer.transform.Find("HEAD_HANDS")
                    .transform.Find("CameraContainer")
                    .transform.Find("Head").GetComponent<Camera>();
            }
            return null;
        }

        public void findPlayer()
        {
            if (LocalPlayer == null)
            {
                GameObject localPlayer = this.gameObject.FindLocalPlayer();
                //player doesn't exist
                if ((UnityEngine.Object)localPlayer == (UnityEngine.Object)null)
                {
                    LocalPlayer = null;
                    return;
                }
                //player exists
                //Logs.WriteWarning("PLAYER EXISTS");
                var netID = this.transform.root.GetComponentCached<NetIdentity>();
                if (!netID || (netID && netID.IsLocalPlayer))
                {
                    LocalPlayer = localPlayer;
                }
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