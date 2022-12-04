using UnityEngine;
using Valve.VR;

namespace SignOfSilenceVR
{
    public class VRHands : MonoBehaviour
    {

        public static GameObject RightHand = null;
        public static GameObject LeftHand = null;
        public static Transform parentHands;
        public static Transform parentCamera;
        public static bool showModel = false;
        public static VrLaser vrLaser = null;

        private void Update()
        {
            SpawnHands();

            var parentUpdate = getCamera();
            if (parentUpdate && parentUpdate != parentCamera)
            {
                parentCamera = parentUpdate;
                vrLaser.SetUp(parentCamera.gameObject.GetComponentInChildren<Camera>());
            }
        }

        public static void SpawnHands()
        {
            var parentUpdate = getCamera();
            if (parentUpdate && parentUpdate != parentHands)
            {
                parentHands = parentUpdate;
                createHands();
                attachLightToHand();
            }
        }

        public static Transform getCamera()
        {
            if (CameraManager.cameraParent)
            {
                showModel = false;
                return CameraManager.cameraParent;
            }
            if (Camera.main)
            {
                showModel = true;
                return Camera.main.transform.parent;
            }
            return null;
        }

        public static void createHands()
        {
            RightHand = Instantiate(AssetLoader.RightHandBase, Vector3.zeroVector,
                Quaternion.identityQuaternion);
            RightHand.transform.parent = parentHands;
            RightHand.transform.localScale = Vector3.one;
            vrLaser = VrLaser.Create(RightHand.transform);
            RightHand.transform.Find("Model").gameObject.SetActive(showModel);

            LeftHand = Instantiate(AssetLoader.LeftHandBase, Vector3.zeroVector,
                Quaternion.identityQuaternion);
            LeftHand.transform.parent = parentHands;
            LeftHand.transform.localScale = Vector3.one;
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
                    //DEFAULT LIGHT
                    var spotlight = CameraManager.cameraParent.Find("Head")
                        .transform.Find("Spotlight").transform;
                    spotlight.parent = RightHand.transform;
                    spotlight.localPosition = Vector3.zero;
                    spotlight.eulerAngles = new Vector3(35, 0, 0);
                    //spotlight.GetComponent<Light>().cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));

                    //Head controls and raycast
                    var raycaster = CameraManager.cameraParent.Find("Head")
                        .transform.Find("RaycasterAndControls").transform;
                    raycaster.parent = RightHand.transform;
                    raycaster.localPosition = Vector3.zero;
                    raycaster.eulerAngles = new Vector3(35, 0, 0);

                    /*
                    //Right model hand
                    var righHandModel = CameraManager.cameraParent.parent.transform.Find("MODEL_ROOT").transform
                        .GetChild(0).transform.Find("Right_arm").transform;
                    righHandModel.parent = RightHand.transform;
                    righHandModel.localPosition = Vector3.zero;
                    var righHand = CameraManager.cameraParent.parent.transform.Find("HEAD_HANDS/right").transform;
                    righHand.parent = RightHand.transform;
                    righHand.localPosition = Vector3.zero;

                    //ALL OTHER ITEMS
                    var items = CameraManager.cameraParent.parent.transform.Find("ITEMS").transform;
                    items.parent = RightHand.transform;
                    items.localPosition = Vector3.zero;
                    */
                }
            }
        }

        public static void OnDestroy()
        {
            RightHand = null;
            LeftHand = null;
            parentHands = null;
            parentCamera = null;
        }
    }
}
