using UnityEngine;
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
            RightHand.AddComponent<Pointer>().gameObject.SetActive(showPointer);

            LeftHand = Instantiate(AssetLoader.LeftHandBase, Vector3.zeroVector,
                Quaternion.identityQuaternion);
            LeftHand.transform.parent = parent;
            LeftHand.transform.position = parent.position;
            LeftHand.transform.localPosition = Vector3.zero;
            LeftHand.transform.localRotation = Quaternion.identity;
            LeftHand.transform.localScale = Vector3.one;
            LeftHand.AddComponent<Pointer>().gameObject.SetActive(showPointer);
        }

        public static void OnDestroy()
        {
            RightHand = null;
            LeftHand = null;
            parent = null;
        }
    }
}
