using UnityEngine;
using UnityEngine.UI;

namespace SignOfSilenceVR
{
    public class AttachedUi : MonoBehaviour
    {
        private Transform targetTransform;
        public Canvas canvas;
        public float m_scale = 0;
        public Canvas m_canvas;
        private BoxCollider collider;
        public float localScale = 0.001f;

        private void Awake()
        {
            m_canvas = gameObject.GetComponent<Canvas>();
            if (m_scale > 0) m_canvas.transform.localScale = Vector3.one * m_scale;
            m_canvas.renderMode = RenderMode.WorldSpace;
        }

        private void Start()
        {
            SetUpCollider();
            AdjustScaler();
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

        public void SetScale(float scale)
        {
            m_scale = scale;
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

        private void SetUpCollider()
        {
            collider = gameObject.GetComponent<BoxCollider>();
            if (collider != null) return;

            var rectTransform = gameObject.GetComponent<RectTransform>();
            collider = gameObject.gameObject.AddComponent<BoxCollider>();
            var rectSize = rectTransform.sizeDelta;
            collider.size = new Vector3(rectSize.x, rectSize.y, 0.1f);
            gameObject.layer = LayerMask.NameToLayer("UI");
            m_canvas.gameObject.layer = LayerMask.NameToLayer("UI");
        }

        private void AdjustScaler()
        {
            var scaler = m_canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                scaler.scaleFactor = 1;
                scaler.referencePixelsPerUnit = 100;
            }
            m_canvas.transform.localScale = Vector3.one * localScale;
        }
    }
}
