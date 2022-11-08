using UnityEngine;
using Valve.VR;

namespace SignOfSilenceVR
{
    public class VRHands : MonoBehaviour
    {

        public static GameObject RightHand = null;
        public static GameObject LeftHand = null;

        private void Start()
        {
        }

        private void Update()
        {
            SpawnHands();
        }

        public static void SpawnHands()
        {
            var parent = (CameraManager.LocalPlayer) ?
                    CameraManager.cameraParent.transform : Camera.main.transform.parent.transform;
            Logs.WriteWarning(parent.ToString());
            if (!RightHand)
            {
                RightHand = Instantiate(AssetLoader.RightHandBase, Vector3.zeroVector,
                    Quaternion.identityQuaternion);
                RightHand.transform.parent = parent;
                RightHand.transform.position = parent.position;
                RightHand.transform.localPosition = Vector3.zero;
                RightHand.transform.localRotation = Quaternion.identity;
                RightHand.transform.localScale = Vector3.one;
                RightHand.AddComponent<Pointer>();
            }

            if (!LeftHand)
            {
                 LeftHand = Instantiate(AssetLoader.LeftHandBase, Vector3.zeroVector,
                     Quaternion.identityQuaternion);
                LeftHand.transform.parent = parent;
                LeftHand.transform.position = parent.position;
                LeftHand.transform.localPosition = Vector3.zero;
                LeftHand.transform.localRotation = Quaternion.identity;
                LeftHand.transform.localScale = Vector3.one;
                LeftHand.AddComponent<Pointer>();
            }
        }
    }
}
