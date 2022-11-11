using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

namespace SignOfSilenceVR
{
    public class VRPointerInput : BaseInputModule
    {
        public Camera eventCamera = null;
        public SteamVR_Input_Sources m_inputSource = SteamVR_Input_Sources.RightHand;
        public SteamVR_Action_Boolean m_clickAction = SteamVR_Actions.default_TriggerRight;
        public PointerEventData m_Data = null;
        public GameObject m_CurrentGameObject = null;

        protected override void Awake()
        {
            base.Awake();
            m_Data = new PointerEventData(eventSystem);
            eventCamera = transform.parent.GetComponent<Camera>();
        }

        public override void Process()
        {
            m_Data.Reset();
            m_Data.position = new Vector2(eventCamera.pixelWidth / 2, eventCamera.pixelHeight / 2);

            eventSystem.RaycastAll(m_Data, m_RaycastResultCache);
            m_Data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            m_CurrentGameObject = m_Data.pointerCurrentRaycast.gameObject;
            
            m_RaycastResultCache.Clear();

            //hover
            HandlePointerExitAndEnter(m_Data, m_CurrentGameObject);

            //Press
            if(m_clickAction.GetStateDown(m_inputSource))
            {
                ProcessPress(m_Data);
            }

            //Release
            if (m_clickAction.GetStateUp(m_inputSource))
            {
                ProcessRelease(m_Data);
            }
        }

        public PointerEventData getData()
        {
            return m_Data;
        }

        private void ProcessPress(PointerEventData data)
        {
            data.pointerPressRaycast = data.pointerCurrentRaycast;
            GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(
                m_CurrentGameObject,
                data,
                ExecuteEvents.pointerDownHandler
            );

            if(newPointerPress == null)
            {
                newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(m_CurrentGameObject);
            }

            data.pressPosition = data.position;
            data.pointerPress = newPointerPress;
            data.rawPointerPress = m_CurrentGameObject;
        }
        
        private void ProcessRelease(PointerEventData data)
        {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);
            GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerUpHandler>(m_CurrentGameObject);
            if(data.pointerPress == pointerUpHandler)
            {
                ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
            }
            eventSystem.SetSelectedGameObject(null);

            data.pressPosition = Vector2.zero;
            data.pointerPress = null;
            data.rawPointerPress = null;
        }
    }
}
