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
            CreateUIColliders();
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
                        /*
                        var target = new GameObject("TitleScreen");
                        target.transform.position = new Vector3(903.6f, 64.2f, 230.3f);
                        target.transform.rotation = Quaternion.Euler(0, 69, 0);
                        AdjustScaler(canvas);
                        AttachedUi.Create<AttachedUi>(canvas, target.transform, 0.0015f);
                        patchedCanvases.Add(canvas);
                        */
                    }
                    else
                    {
                        if (CameraManager.LocalPlayer != null)
                        {
                            var target = new GameObject("PlayerHeadUI");
                            target.transform.parent = CameraManager.playerCamera.transform;
                            target.transform.localPosition = new Vector3(0, 0, 2);
                            target.transform.rotation = Quaternion.identity;
                            AdjustScaler(canvas);
                            AttachedUi.Create<AttachedUi>(canvas, target.transform, 0.0015f);
                            patchedCanvases.Add(canvas);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates box colliders for all canvases that have at least one selectable item
        /// </summary>
        private static void CreateUIColliders()
        {
            var selectables = Resources.FindObjectsOfTypeAll<Selectable>();
            foreach (var selectable in selectables)
            {
                if (selectable.targetGraphic != null &&
                   selectable.targetGraphic.canvas != null &&
                   !IsCanvasToIgnore(selectable.targetGraphic.canvas.name))
                {
                    SetupInteractableCanvasCollider(selectable.targetGraphic.canvas);
                }
            }
        }

        private static void SetupInteractableCanvasCollider(Canvas canvas, GameObject proxy = null)
        {
            if (proxy == null) proxy = canvas.gameObject;
            var collider = proxy.GetComponent<BoxCollider>();
            if (collider == null)
            {
                var rectTransform = canvas.GetComponent<RectTransform>();
                var thickness = 0.1f;
                collider = proxy.gameObject.AddComponent<BoxCollider>();
                collider.size = rectTransform.sizeDelta;
                collider.center = new Vector3(0, 0, thickness * 0.5f);
                proxy.layer = LayerMask.NameToLayer("UI");
                canvas.worldCamera = Camera.main;
            }
        }

        private static void AdjustScaler(Canvas canvas, float localScale = 0.001f)
        {
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                scaler.scaleFactor = 1;
                scaler.referencePixelsPerUnit = 100;
            }
            canvas.transform.localScale = Vector3.one * localScale;
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
