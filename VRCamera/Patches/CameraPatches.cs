using System;
using UnityEngine;
using Donteco;

namespace SignOfSilenceVR
{
    public class CameraPatches: MonoBehaviour
    {
        private void Update()
        {
            // attach to head ?
            GameObject localPlayer = this.gameObject.FindLocalPlayer();
            if ((UnityEngine.Object)localPlayer == (UnityEngine.Object)null)
            {
                Logs.WriteWarning("OBJECT NULL");
                CameraManager.VROrigin = this.GetComponent<Camera>();
                return;
            }
            Camera componentInChildren = localPlayer.GetComponentInChildren<Camera>();
            CameraManager.VROrigin = componentInChildren;
        }
    }
}
