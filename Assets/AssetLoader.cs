using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;

namespace SignOfSilenceVR
{
    class AssetLoader
    {
        public static GameObject LeftHandBase;
        public static GameObject RightHandBase;

        public AssetLoader()
        {
            var VRHands = LoadBundle("vrhands");
            LeftHandBase = LoadAsset<GameObject>(VRHands, "SteamVR/Prefabs/vr_glove_left.prefab");
            RightHandBase = LoadAsset<GameObject>(VRHands, "SteamVR/Prefabs/vr_glove_right.prefab");
        }

        private T LoadAsset<T>(AssetBundle bundle, string prefabName) where T : UnityEngine.Object
        {
            var asset = bundle.LoadAsset<T>($"Assets/{prefabName}");
            if (asset)
                return asset;
            else
            {
                Logs.WriteError($"Failed to load asset {prefabName}");
                return null;
            }

        }

        private static AssetBundle LoadBundle(string assetName)
        {
            var myLoadedAssetBundle =
                AssetBundle.LoadFromFile(
                    $"{Plugin.gamePath}/Sign Of Silence_Data/AssetsVR/{assetName}");
            if (myLoadedAssetBundle == null)
            {
                Logs.WriteError($"Failed to load AssetBundle {assetName}");
                return null;
            }

            return myLoadedAssetBundle;
        }

    }
}