using UnityEngine;
using Valve.VR;
using Donteco;

namespace SignOfSilenceVR
{
    public class CameraManager : MonoBehaviour
    {
        // VR Origin and body stuff
        public static GameObject LocalPlayer = null;
        public static Camera playerCamera = null;
        public static Transform cameraParent;
        public static float speedTransform = 6f;

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
                    // resetPlayerHeadPosition();
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

        //update crouch position; following head ingame
        public void updateCrouchPosition()
        {
            Vector3 offsetPos = playerCamera.transform.position
                - getHeadBone().position;
            cameraParent.position = Vector3.Lerp(
                cameraParent.position,
                cameraParent.position - offsetPos,
                Time.deltaTime * speedTransform
            );
        }

        // Not used anymore but may be useful
        public static int getCrouchState()
        {
            //crouched and or walking
            var playerCtrl = LocalPlayer.GetComponent<PlayerMovementController>();
            switch (true)
            {
                case true when playerCtrl.IsCrouching && CameraPatches.isMoving:
                    return 1;
                case true when playerCtrl.IsCrouching && !CameraPatches.isMoving:
                    return 0;
                default:
                    return 2;
            }
        }

        public static void resetPlayerHeadPosition()
        {
            float offsetAngle = playerCamera.transform.rotation.eulerAngles.y
                - LocalPlayer.transform.rotation.eulerAngles.y;
            cameraParent.Rotate(0f, -offsetAngle, 0f);

            Vector3 offsetPos = playerCamera.transform.position
                - getHeadBone().position;
            cameraParent.position -= offsetPos;
        }

        public void SetupCamera()
        {
            // Make an empty parent object for moving the camera around.
            playerCamera = findOriginCamera();
            if (playerCamera)
            {
                cameraParent = new GameObject("VrCameraParent").transform;
                cameraParent.parent = LocalPlayer.transform;
                cameraParent.position = getHeadBone().position;
                cameraParent.rotation = LocalPlayer.transform.rotation;
                cameraParent.localRotation = Quaternion.identity;
                playerCamera.transform.parent = cameraParent;
                resetPlayerHeadPosition();
                //hide head
                hideHead();
            }
            else
            {
                cameraParent = new GameObject("VrCameraParent").transform;
                cameraParent.parent = Camera.main.transform.parent;
                cameraParent.position = Camera.main.transform.position;
                cameraParent.rotation = Camera.main.transform.rotation;
                cameraParent.localRotation = Quaternion.identity;
                Camera.main.transform.parent = cameraParent;
            }
        }

        //trying to implement a way to move virtual player
        // if real player moves in real life.
        //Might conflict with updateCrouchPosition
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
            var neck = getHeadBone(HumanBodyBones.Neck);
            if (neck != null)
            {
                neck.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            }
        }

        public static Transform getHeadBone(HumanBodyBones id = HumanBodyBones.Head)
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
            return animator.GetBoneTransform(id);
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
                var netID = this.transform.root.GetComponentCached<NetIdentity>();
                if (!netID || (netID && netID.IsLocalPlayer))
                {
                    LocalPlayer = localPlayer;
                }
            }
        }
    }

}