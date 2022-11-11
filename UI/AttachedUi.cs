using UnityEngine;
using UnityEngine.UI;

namespace SignOfSilenceVR
{
    public class AttachedUi : MonoBehaviour
    {
        private Transform targetTransform;

        public static void Create<TAttachedUi>(Canvas canvas, Transform target, float scale = 0)
            where TAttachedUi : AttachedUi
        {
            var instance = canvas.gameObject.AddComponent<TAttachedUi>();
            if (scale > 0) canvas.transform.localScale = Vector3.one * scale;
            canvas.renderMode = RenderMode.WorldSpace;

            instance.targetTransform = target;
        }

        private void Start()
        {
            SetupInteractableCanvasCollider();
            AdjustScaler(canvas);
        }

        protected virtual void Update()
        {
            if (!targetTransform)
            {
                Logs.WriteWarning($"Target transform for AttachedUi {name} is missing, destroying");
                Destroy(this);
                return;
            }

            UpdateTransform();
        }

        public void SetTargetTransform(Transform target)
        {
            targetTransform = target;
        }

        private void UpdateTransform()
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
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
    }
}
