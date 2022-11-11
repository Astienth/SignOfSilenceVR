using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SignOfSilenceVR
{
    class UIManager : MonoBehaviour
    {
        private static readonly List<Canvas> patchedCanvases = new List<Canvas>();
        private static readonly string[] canvasesToIgnore =
        {
                "com.sinai.unityexplorer_Root", // Unity Explorer
                "com.sinai.universelib.resizeCursor_Root",
                "com.sinai.unityexplorer.MouseInspector_Root"
        };
        private readonly List<GameObject> canvasesToDisable = new List<GameObject>();

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            initCanvases();
        }

        public static void initCanvases()
        {
            var canvases = Resources.FindObjectsOfTypeAll<Canvas>();
            foreach (var canvas in canvases)
            {
                if (IsCanvasToIgnore(canvas.name))
                {
                    continue;
                }

                if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) return;

                if (canvas && !patchedCanvases.Contains(canvas))
                {
                    if (SceneManager.GetActiveScene().name == "menu")
                    {
                        var target = new GameObject("TitleScreen");
                        target.transform.position = new Vector3(903.6f, 64.2f, 230.3f);
                        target.transform.rotation = Quaternion.Euler(0, 69, 0);
                        var ui = canvas.gameObject.AddComponent<AttachedUi>();
                        ui.SetTargetTransform(target.transform);
                        ui.SetScale(0.0015f);
                        patchedCanvases.Add(canvas);
                    }
                    else
                    {
                        if (CameraManager.LocalPlayer != null)
                        {
                            var target = new GameObject("PlayerHeadUI");
                            target.transform.parent = CameraManager.playerCamera.transform;
                            target.transform.localPosition = new Vector3(0, 0, 2);
                            //target.transform.rotation = Quaternion.identity;
                            var ui = canvas.gameObject.AddComponent<AttachedUi>();
                            ui.SetTargetTransform(target.transform);
                            ui.SetScale(0.0015f);
                            patchedCanvases.Add(canvas);
                        }
                    }
                }
            }
        }

        private static bool IsCanvasToIgnore(string canvasName)
        {
            foreach (var s in canvasesToIgnore)
                if (Equals(s, canvasName))
                    return true;
            return false;
        }
    }
}
