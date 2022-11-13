﻿using UnityEngine;
using UnityEngine.UI;

namespace SignOfSilenceVR
{
    public class AttachedUi : MonoBehaviour
    {
        private Transform targetTransform;
        public float m_scale = 0;
        public Canvas m_canvas;
        private BoxCollider collider;
        public float localScale = 0.001f;
        public bool updatePosition = true;
        public bool updateOnce = false;
        public bool firstUpdate = false;

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
            if (updatePosition)
            {
                UpdateTransform();
            }
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
            if (!updatePosition || (firstUpdate && updateOnce))
            {
                return;
            }
            gameObject.transform.position = targetTransform.position;
            gameObject.transform.rotation = targetTransform.rotation;
            firstUpdate = true;
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
