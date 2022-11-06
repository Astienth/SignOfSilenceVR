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
        private static Vector3 camLocPos = new Vector3(0.1f,0.7f,0.2f); 
        private static Vector3 camOffsetCrouch = new Vector3(0.1f, 0.15f, 0.2f); 
        private static Vector3 camOffsetCrouchedWalking = new Vector3(0.1f, 0.45f, 0.2f);

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
                   // MovePlayerToCamera();
                }
                //crouched and or walking
                var playerCtrl = LocalPlayer.GetComponent<PlayerMovementController>();
                switch (true)
                {
                    case true when playerCtrl.IsCrouching && CameraPatches.isMoving:
                        cameraParent.localPosition = Vector3.Lerp(cameraParent.localPosition,
                            camOffsetCrouchedWalking, Time.deltaTime);
                        break;
                    case true when playerCtrl.IsCrouching && !CameraPatches.isMoving:
                        cameraParent.localPosition = Vector3.Lerp(cameraParent.localPosition,
                            camOffsetCrouch, Time.deltaTime);
                        break;
                    default:
                        cameraParent.localPosition = Vector3.Lerp(cameraParent.localPosition,
                            camLocPos, Time.deltaTime);
                        break;
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
            //NOT WORKING ON Y AXE
            var diffAngle = playerHead.transform.rotation.eulerAngles.y - playerCamera.transform.rotation.eulerAngles.y;
            cameraParent.Rotate(0, diffAngle, 0);
            var diffPos = playerHead.transform.position - playerCamera.transform.position;
            cameraParent.localPosition = camLocPos;
            cameraParent.position += diffPos;

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
                RightHand = Instantiate(AssetLoader.RightHandBase, Vector3.zeroVector,
                    Quaternion.identityQuaternion);
                RightHand.transform.parent = (LocalPlayer) ? LocalPlayer.transform : Camera.main.transform;
                RightHand.SetActive(false);
                var pose = RightHand.GetComponent<SteamVR_Behaviour_Pose>();
                pose.inputSource = SteamVR_Input_Sources.RightHand;
                pose.poseAction = SteamVR_Actions.default_RightHandPose;
            }
            
            if (!LeftHand)
            {
                LeftHand = Instantiate(AssetLoader.LeftHandBase, Vector3.zeroVector,
                    Quaternion.identityQuaternion);
                LeftHand.transform.parent = (LocalPlayer) ? LocalPlayer.transform : Camera.main.transform;
                LeftHand.SetActive(false);
                var pose = LeftHand.GetComponent<SteamVR_Behaviour_Pose>();
                pose.inputSource = SteamVR_Input_Sources.LeftHand;
                pose.poseAction = SteamVR_Actions.default_LeftHandPose;
            }
            if (RightHand && LeftHand)
            {
                RightHand.SetActive(true);
                LeftHand.SetActive(true);
            }
        }
    }

}