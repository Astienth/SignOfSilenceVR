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
        private static GameObject canvasBlur = null;
        public static Vector3 standingUI = new Vector3(0, 1.6f, 1);
        public static Vector3 crouchingUI = new Vector3(0, 0.7f, 1);
        public static Vector3 crouchmovingUI = new Vector3(0, 0.9f, 1.2f);

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            initMainCanvas();
        }

        private void LateUpdate()
        {
            if (canvasBlur)
            {
                canvasBlur.SetActive(false);
            }
        }

        public void initMainCanvas()
        {
            //title screen
            if (SceneManager.GetActiveScene().name == "menu")
            {
                var canvas = GameObject.Find("UI/Canvas")?.GetComponent<Canvas>();
                var target = new GameObject("TitleScreen");
                target.transform.position = new Vector3(902.6f, 64.4f, 230.2f);
                target.transform.rotation = Quaternion.Euler(0, 58f, 0);
                var ui = canvas.gameObject.AddComponent<AttachedUi>();
                ui.speedTransform = 50;
                ui.SetTargetTransform(target.transform);
                ui.SetScale(0.0015f);
                patchedCanvases.Add(canvas.name);
            }
            //Player UI
            if (CameraManager.playerCamera && CameraManager.LocalPlayer != null)
            {
                var canvas = GameObject.Find("PlayerUI/CanvasMain")?.GetComponent<Canvas>();
                if (canvas && !patchedCanvases.Contains(canvas.name) )
                {
                    var target = new GameObject("PlayerHeadUI");
                    target.transform.parent = CameraManager.LocalPlayer.transform;
                    target.transform.localPosition = standingUI;
                    target.transform.rotation = Quaternion.identity;
                    //canvas.worldCamera = Camera.main;
                    var ui = canvas.gameObject.AddComponent<AttachedUi>();
                    ui.updateCrouch = true;
                    ui.speedTransform = CameraManager.speedTransform + 6;
                    ui.SetTargetTransform(target.transform);
                    ui.SetScale(0.0015f);
                    fixPlayerUI(canvas);
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
                    target.transform.localPosition = standingUI;
                    target.transform.rotation = Quaternion.identity;
                    var ui = canvas.gameObject.AddComponent<AttachedUi>();
                    ui.SetTargetTransform(target.transform);
                    ui.SetScale(0.0015f);
                    patchedCanvases.Add(canvas.name);
                }
            }
        }

        public void fixPlayerUI(Canvas canvas)
        {
            //getting the blurring component 
            canvasBlur = GameObject.Find("PlayerUI/CanvasBlur/Blur").gameObject;
            //moving hearts and sound and bottom UI
            var hearts = GameObject.Find("PlayerUI/CanvasMain/Hearts");
            hearts.transform.localPosition += new Vector3(0,-150f,-500f);
            var noise = GameObject.Find("PlayerUI/CanvasMain/Noise");
            noise.transform.localPosition += new Vector3(520f, -150f, -500f); 
            var RightBottomPanel = GameObject.Find("PlayerUI/CanvasMain/RightBottomPanel");
            RightBottomPanel.transform.localPosition += new Vector3(-520f, -150f, -500f);
            GameObject.Find("PlayerUI/CanvasMain/Sight").gameObject.SetActive(false);

            /*
            //UI camera
            var target = new GameObject("UICam");
            target.layer = LayerMask.NameToLayer("UI");
            Camera UICam = target.AddComponent<Camera>();
            UICam.name = "UICam";
            UICam.clearFlags = CameraClearFlags.Depth;
            UICam.targetDisplay = 0;
            UICam.cullingMask = LayerMask.GetMask("UI");
            UICam.depth = 10;
            target.transform.parent = CameraManager.playerCamera.transform;
            target.transform.localPosition = Vector3.zero;
            target.transform.rotation = Quaternion.identity;
            CameraManager.playerCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
            */
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
