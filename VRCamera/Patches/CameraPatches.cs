using System;
using UnityEngine;
using Donteco;

namespace SignOfSilenceVR
{
    public class CameraPatches: MonoBehaviour
    {
        private void Update()
        {
            GameObject localPlayer = this.gameObject.FindLocalPlayer();
            //player doesn't exist
            if ((UnityEngine.Object)localPlayer == (UnityEngine.Object)null)
            {
                CameraManager.LocalPlayer = null;
                return;
            }
            //player exists
            //Logs.WriteWarning("PLAYER EXISTS");
            var netID = this.transform.root.GetComponentCached<NetIdentity>();
            if (!netID || (netID && netID.IsLocalPlayer))
            {
                CameraManager.LocalPlayer = localPlayer;
                CameraManager.HandleFirstPersonCamera();
            }
        }
    }
}
