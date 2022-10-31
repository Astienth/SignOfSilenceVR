using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SignOfSilenceVR
{
    class UIPatches : MonoBehaviour
    {
        private static readonly List<Canvas> patchedCanvases = new List<Canvas>();
        private static readonly string[] canvasesToDisable =
        {
            "BlackBars", // Cinematic black bars.
            "Camera" // Disposable camera.
        };

        private static readonly string[] canvasesToIgnore =
        {
            "com.sinai.unityexplorer_Root", // UnityExplorer.
            "com.sinai.unityexplorer.MouseInspector_Root", // UnityExplorer.
            "ExplorerCanvas"
        };

        private void FixedUpdate()
        {
            if (SceneManager.GetActiveScene().name == "menu")
            {
                var canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
                if (canvas && !patchedCanvases.Contains(canvas))
                {
                    AdjustScaler(canvas);
                    ApplyWorldSpace(canvas);
                    canvas.GetComponent<AttachedUi>().followHead = false;
                    canvas.GetComponent<AttachedUi>().setPosition(new Vector3(980,65,230));
                    patchedCanvases.Add(canvas);
                }
            }
        }

        private void ApplyWorldSpace(Canvas canvas)
        {
            try
            {
                if (!canvas || IsCanvasToIgnore(canvas.name))
                {
                    return;
                }
                if (IsCanvasToDisable(canvas.name))
                {
                    canvas.enabled = false;
                    return;
                }
                if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    return;
                }
                AttachedUi.Create<AttachedUi>(canvas, 0.001f);
                return;
            }
            catch (Exception exception)
            {
                Logs.WriteWarning($"Failed to move canvas to world space ({this.name}): {exception}");
                return;
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

        private static bool IsCanvasToDisable(string canvasName)
        {
            foreach (var s in canvasesToDisable)
                if (Equals(s, canvasName))
                    return true;
            return false;
        }
    }
}
