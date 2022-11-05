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
        private static Vector3 camLocPos = new Vector3(0,0.7f,0.2f); 
        private static Vector3 camOffsetCrouch = new Vector3(0, 0.15f, 0.25f);

        private void Start()
        {
            findPlayer();
            if (LocalPlayer)
            {
                setPlayerHead();
                SetupCamera();
            }
        }

        internal void Update()
        {
            if (LocalPlayer)
            {
                var cameraToHead = Vector3.ProjectOnPlane(playerHead.position
                 - playerCamera.transform.position, playerHead.transform.up);

                if (cameraToHead.sqrMagnitude > 0.5f)
                {
                    Logs.WriteWarning(cameraToHead.sqrMagnitude);
                   // MovePlayerToCamera();
                }
                if (LocalPlayer.GetComponent<PlayerMovementController>().IsCrouching)
                {
                    cameraParent.localPosition = camOffsetCrouch;
                }
                else
                {
                    cameraParent.localPosition = camLocPos;
                }
            }
            else
            {
                findPlayer();
                if (LocalPlayer)
                {
                    setPlayerHead();
                    SetupCamera();
                }
            }
        }

        public static void resetPlayerHeadPosition()
        {
            var diffAngle = playerHead.transform.rotation.eulerAngles.y - playerCamera.transform.rotation.eulerAngles.y;
            cameraParent.Rotate(0, diffAngle, 0);
            var diffPos = playerHead.transform.position - playerCamera.transform.position;
            cameraParent.position += diffPos;
            cameraParent.localPosition = camLocPos;

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
                playerCamera.transform.parent = cameraParent;
                resetPlayerHeadPosition();
                //hide head
                hideHead();
            }
        }

        public static void MovePlayerToCamera()
        {
            LocalPlayer.transform.position = new Vector3(
                playerCamera.transform.position.x,
                playerCamera.transform.position.y, 
                playerCamera.transform.position.z
            );
        }

        private void setPlayerHead()
        {
            if (playerHead == null)
            {
                playerHead = LocalPlayer.transform.Find("HEAD_HANDS").transform;
            }
        }
        private void hideHead()
        {
            var headBone = getHeadBone();
            if (headBone != null)
            {
                headBone.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            }
        }

        private Transform getHeadBone()
        {
            if (LocalPlayer == null)
            {
                return null;
            }
            var animator = LocalPlayer.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                return null;
            }
            return animator.GetBoneTransform(HumanBodyBones.Head);
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