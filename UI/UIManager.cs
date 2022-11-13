using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SignOfSilenceVR
{
    class UIManager : MonoBehaviour
    {
        private static readonly List<string> patchedCanvases = new List<string>();
        private static readonly string[] canvasesToIgnore =
        {
                "com.sinai.unityexplorer_Root", // Unity Explorer
                "com.sinai.universelib.resizeCursor_Root",
                "com.sinai.unityexplorer.MouseInspector_Root"
        };

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            initMainCanvas();
        }

        public void initMainCanvas()
        {
            //title screen
            if (SceneManager.GetActiveScene().name == "menu")
            {
                var canvas = GameObject.Find("UI/Canvas")?.GetComponent<Canvas>();
                var target = new GameObject("TitleScreen");
                target.transform.position = new Vector3(903.6f, 64.2f, 230.3f);
                target.transform.rotation = Quaternion.Euler(0, 69, 0);
                var ui = canvas.gameObject.AddComponent<AttachedUi>();
                ui.SetTargetTransform(target.transform);
                ui.updateOnce = true;
                ui.SetScale(0.0015f);
                patchedCanvases.Add(canvas.name);
            }
            //Player UI
            if (CameraManager.playerCamera && CameraManager.LocalPlayer != null)
            {
                var canvas = GameObject.Find("PlayerUI/CanvasMain")?.GetComponent<Canvas>();
                Logs.WriteWarning("CANVAS FOUND ? "+ patchedCanvases.Contains(canvas.name));
                if (canvas && !patchedCanvases.Contains(canvas.name) )
                {
                    var target = new GameObject("PlayerHeadUI");
                    Logs.WriteWarning("CANVAS UI POSITION");
                    target.transform.parent = CameraManager.LocalPlayer.transform;
                    target.transform.localPosition = new Vector3(0, 1, 1);
                    //target.transform.rotation = Quaternion.identity;
                    var ui = canvas.gameObject.AddComponent<AttachedUi>();
                    ui.SetTargetTransform(target.transform);
                    ui.SetScale(0.0015f);
                    patchedCanvases.Add(canvas.name);
                }
            }
            
            //Loading screen
            if (Camera.main)
            {
                var canvas = GameObject.Find("LoadingScreen/View/Canvas")?.GetComponent<Canvas>();
                if (canvas && !patchedCanvases.Contains(canvas.name))
                {
                    var target = new GameObject("LoadingScreenUI");
                    target.transform.parent = Camera.main.transform;
                    target.transform.localPosition = new Vector3(0, 0, 2);
                    //target.transform.rotation = Quaternion.identity;
                    var ui = canvas.gameObject.AddComponent<AttachedUi>();
                    ui.updateOnce = true;
                    ui.SetTargetTransform(target.transform);
                    ui.SetScale(0.0015f);
                    patchedCanvases.Add(canvas.name);
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
