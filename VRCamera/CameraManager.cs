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
        private static Vector3 camLocPos = new Vector3(0,0.7f,0); 
        private static Vector3 camOffsetCrouch = new Vector3(0, 0.15f, 0); 
        private static Vector3 camOffsetCrouchedWalking = new Vector3(0, 0.45f, 0);

        private void Start()
        {
            findPlayer();
            if (LocalPlayer)
            {
                SetupCamera();
            }
        }

        internal void Update()
        {
            if (LocalPlayer)
            {
                var cameraToHead = Vector3.ProjectOnPlane(LocalPlayer.transform.position
                 - playerCamera.transform.position, LocalPlayer.transform.transform.up);

                if (cameraToHead.sqrMagnitude > 0.5f)
                {
                   // MovePlayerToCamera();
                }
                //update crouch position
                updateCrouchPosition();
            }
            else
            {
                findPlayer();
                if (LocalPlayer)
                {
                    SetupCamera();
                }
            }
        }

        public void updateCrouchPosition()
        {
            //crouched and or walking
            var playerCtrl = LocalPlayer.GetComponent<PlayerMovementController>();
            switch (true)
            {
                case true when playerCtrl.IsCrouching && CameraPatches.isMoving:
                    cameraParent.position = Vector3.Lerp(cameraParent.position,
                        LocalPlayer.transform.position + camOffsetCrouchedWalking, Time.deltaTime * 2);
                    break;
                case true when playerCtrl.IsCrouching && !CameraPatches.isMoving:
                    cameraParent.position = Vector3.Lerp(cameraParent.position,
                        LocalPlayer.transform.position + camOffsetCrouch, Time.deltaTime * 2);
                    break;
                default:
                    cameraParent.position = Vector3.Lerp(cameraParent.position,
                        LocalPlayer.transform.position + camLocPos, Time.deltaTime * 2);
                    break;
            }
        }

        public static void resetPlayerHeadPosition()
        {
            /*
            //{first rotation}
            //get current head heading in scene
            //(y-only, to avoid tilting the floor)
            float offsetAngle = steamCamera.rotation.eulerAngles.y;
            //now rotate CameraRig in opposite direction to compensate
            steamController.Rotate(0f, -offsetAngle, 0f);

            //{now position}
            //calculate postional offset between CameraRig and Camera
            Vector3 offsetPos = steamCamera.position - steamController.position;
            //reposition CameraRig to desired position minus offset
            steamController.position = (desiredHeadPos.position - offsetPos);
            */
        }

        public void SetupCamera()
        {
            // Make an empty parent object for moving the camera around.
            playerCamera = findOriginCamera();
            if (playerCamera)
            {
                cameraParent = new GameObject("VrCameraParent").transform;
                cameraParent.parent = LocalPlayer.transform;
                cameraParent.position = LocalPlayer.transform.position + camLocPos;
                cameraParent.rotation = LocalPlayer.transform.rotation;
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