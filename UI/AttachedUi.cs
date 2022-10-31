using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SignOfSilenceVR
{
    class AttachedUi : MonoBehaviour
    {
        public bool followHead = true;

        public static void Create<TAttachedUi>(Canvas canvas, float scale = 0)
            where TAttachedUi : AttachedUi
        {
            canvas.gameObject.AddComponent<TAttachedUi>();
            if (scale > 0) canvas.transform.localScale = Vector3.one * scale;
            canvas.renderMode = RenderMode.WorldSpace;
        }

        protected void Update()
        {
            if (followHead)
            {
                UpdateTransform();
            }
        }

        public void setPosition(Vector3 position)
        {
            transform.position = position;
        }

        private void UpdateTransform()
        {
            transform.position = Donteco.MainCameraCached.Current.transform.position
                + Donteco.MainCameraCached.Current.transform.forward;
            transform.rotation = Donteco.MainCameraCached.Current.transform.rotation;
        }
    }
}
