using System;
using UnityEngine;

namespace SignOfSilenceVR
{
    public class Pointer : MonoBehaviour
    {
        public float m_defaultLength = 5.0f;
        public LineRenderer m_lineRenderer = null;
        public VRInputManager m_inputManager;

        private void Awake()
        {
            gameObject.AddComponent<LineRenderer>();
            m_lineRenderer = GetComponent<LineRenderer>();
            m_lineRenderer.startWidth = 0.005f;
            m_lineRenderer.endWidth = 0.001f;
            m_lineRenderer.endColor = new Color(1, 1, 1, 0.3f);
            m_lineRenderer.startColor = Color.white;
            m_lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        }

        private void Update()
        {
            UpdateLine();
        }

        private void UpdateLine()
        {
            var targetLength = m_defaultLength;
            RaycastHit hit = CreateRaycast(targetLength);
            Vector3 endPosition = transform.position + (transform.forward * targetLength);
            if(hit.collider != null)
            {
                endPosition = hit.point;
            }
            m_lineRenderer.SetPosition(0, transform.position);
            m_lineRenderer.SetPosition(1, endPosition);
        }

        private RaycastHit CreateRaycast(float length)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);
            Physics.Raycast(ray, out hit, m_defaultLength);
            return hit;
        }
    }
}
