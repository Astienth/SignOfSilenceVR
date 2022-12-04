using System;
using System.Collections.Generic;
using System.Linq;
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
        public static Canvas currentCanvas;
        public static bool diedOnce = false;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            patchedCanvases.Clear();
            currentCanvas = null;
            diedOnce = false;
            initMainCanvas();
        }

        private void LateUpdate()
        {
            if (canvasBlur)
            {
                canvasBlur.SetActive(false);
                var sight = GameObject.Find("PlayerUI/CanvasMain/Sight");
                if (sight)
                {
                    sight.gameObject.SetActive(false);
                }
            }
            checkForStatistics();
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
                canvas.gameObject.AddComponent<GraphicRaycaster>();
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
                    currentCanvas = canvas;
                    var target = new GameObject("PlayerHeadUI");
                    target.layer = LayerMask.NameToLayer("UI");
                    target.transform.parent = CameraManager.LocalPlayer.transform;
                    target.transform.localPosition = standingUI;
                    target.transform.rotation = Quaternion.identity;
                    //canvas.worldCamera = Camera.main;
                    canvas.gameObject.AddComponent<GraphicRaycaster>();
                    var ui = canvas.gameObject.AddComponent<AttachedUi>();
                    ui.updateCrouch = true;
                    ui.speedTransform = CameraManager.speedTransform + 10;
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
                    var camLoading = GameObject.Find("LoadingScreen/View/CameraForLoadingScreen").transform;
                    target.transform.position = camLoading.position + new Vector3(0, 1, 1.5f);
                    target.transform.rotation = Quaternion.Euler(0, 0, 0);
                    var ui = canvas.gameObject.AddComponent<AttachedUi>();
                    ui.speedTransform = 50;
                    ui.SetTargetTransform(target.transform);
                    ui.SetScale(0.0015f);
                    patchedCanvases.Add(canvas.name);
                }
            }
        }

        public void checkForStatistics()
        {
            if(!diedOnce && currentCanvas && currentCanvas.transform.Find("GameStatistics/fade")
                .gameObject.activeSelf)
            {
                var cam = GameObject.Find("Quest_EverybodyDead/EndGameCamera/Camera");
                if (cam)
                {
                    currentCanvas.GetComponent<AttachedUi>().updateCrouch = false;
                    var target = GameObject.Find("PlayerHeadUI");
                    target.transform.parent = null;
                    target.transform.position = cam.transform.position + new Vector3(-1.4f, 0, -0.8f);
                    target.transform.rotation = Quaternion.Euler(0, 220, 0);
                    VRHands.RightHand.transform.parent = cam.transform;
                    cam.GetComponent<Camera>().eventMask = ~(1 << LayerMask.NameToLayer("UI"));
                    diedOnce = true;
                }
            }
        }

        public static bool isPointerActive()
        {
            if(currentCanvas != null)
            {
                if(currentCanvas.transform.Find("Menu/area").gameObject.activeSelf
                    || currentCanvas.transform.Find("Prologue").gameObject.activeSelf
                    || currentCanvas.transform.Find("Inventory").gameObject.activeSelf
                    || currentCanvas.transform.Find("GameStatistics/fade").gameObject.activeSelf)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        public void fixPlayerUI(Canvas canvas)
        {
            //getting the blurring component 
            canvasBlur = GameObject.Find("PlayerUI/CanvasBlur/Blur").gameObject;
            canvas.gameObject.layer = LayerMask.NameToLayer("UI");
            GameObject.Find("PlayerUI").gameObject.layer = LayerMask.NameToLayer("UI");
            GameObject.Find("PlayerUI/CanvasBlur").gameObject.layer = LayerMask.NameToLayer("UI");

            //moving hearts and sound and bottom UI
            var hearts = GameObject.Find("PlayerUI/CanvasMain/Hearts");
            hearts.transform.localPosition += new Vector3(0,-100f,-450f);
            var noise = GameObject.Find("PlayerUI/CanvasMain/Noise");
            noise.transform.localPosition += new Vector3(520f, -100f, -450f); 
            var RightBottomPanel = GameObject.Find("PlayerUI/CanvasMain/RightBottomPanel");
            RightBottomPanel.transform.localPosition += new Vector3(-520f, -100f, -450f);

            var notif = GameObject.Find("PlayerUI/CanvasMain/PlayerActionNotification");
            notif.transform.localPosition += new Vector3(0, -100f, -0);

            GameObject.Find("PlayerUI/CanvasMain/Sight").gameObject.SetActive(false);

            //ignore collisions with ui layer
            string[] layers = Enumerable.Range(0, 32).Select(index => LayerMask.LayerToName(index))
                .Where(l => !string.IsNullOrEmpty(l)).ToArray();
            foreach (string layer in layers)
            {
                if (layer == "UI")
                {
                    Physics.IgnoreLayerCollision(LayerMask.NameToLayer("UI")
                        , LayerMask.NameToLayer(layer));
                    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("UI")
                        , LayerMask.NameToLayer(layer));
                }
            }

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
            target.transform.position = Vector3.zero;
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
