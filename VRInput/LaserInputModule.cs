using UnityEngine;
using UnityEngine.EventSystems;

namespace SignOfSilenceVR;

public class LaserInputModule : BaseInputModule
{
    private const float rayDistance = 30f;
    public Camera EventCamera;
    private Vector3 lastHeadPose;
    private PointerEventData pointerData;
    private VrLaser vrLaser;

    public static LaserInputModule Create(VrLaser vrLaser)
    {
        var instance = vrLaser.gameObject.AddComponent<LaserInputModule>();
        instance.vrLaser = vrLaser;
        return instance;
    }

    public override void DeactivateModule()
    {
        base.DeactivateModule();
        if (pointerData != null)
        {
            HandlePendingClick();
            HandlePointerExitAndEnter(pointerData, null);
            pointerData = null;
        }

        eventSystem.SetSelectedGameObject(null, GetBaseEventData());
    }

    public override bool IsPointerOverGameObject(int pointerId)
    {
        return pointerData != null && pointerData.pointerEnter != null;
    }

    private void Update()
    {
        //make sure physycs respond to isTrigger canvases
        Physics.queriesHitTriggers = true;
        Process();
        //make sure physycs DONT respond to isTrigger canvases
        Physics.queriesHitTriggers = false;
    }

    public override void Process()
    {
        if (!EventCamera)
        {
            return;
        }

        CastRay();
        UpdateCurrentObject();

        var clickDown = vrLaser.ClickDown();
        var clickUp = vrLaser.ClickUp();

        if (!pointerData.eligibleForClick && clickDown)
            HandleTrigger();
        else if (clickUp)
            HandlePendingClick();
    }

    private void CastRay()
    {
        var isHit = Physics.Raycast(
            transform.position,
            transform.forward,
            out var hit,
            rayDistance,
            LayerMask.GetMask("UI"));

        if (isHit)
        {
            vrLaser.SetTarget(hit.point);
        }
        else
        {
            vrLaser.SetTarget(null);
        }
        vrLaser.UpdateLaserVisibility(isHit);

        var pointerPosition = EventCamera.WorldToScreenPoint(hit.point);

        if (pointerData == null)
        {
            pointerData = new PointerEventData(eventSystem);
            lastHeadPose = pointerPosition;
        }

        // Cast a ray into the scene
        pointerData.Reset();
        pointerData.position = pointerPosition;
        eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
        pointerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        m_RaycastResultCache.Clear();
        pointerData.delta = pointerPosition - lastHeadPose;
        lastHeadPose = hit.point;
    }

    private void UpdateCurrentObject()
    {
        // Send enter events and update the highlight.
        var go = pointerData.pointerCurrentRaycast.gameObject;
        HandlePointerExitAndEnter(pointerData, go);
        // Update the current selection, or clear if it is no longer the current object.
        var selected = ExecuteEvents.GetEventHandler<ISelectHandler>(go);
        if (selected == eventSystem.currentSelectedGameObject)
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(),
                ExecuteEvents.updateSelectedHandler);
        else
            eventSystem.SetSelectedGameObject(null, pointerData);
    }

    private void HandlePendingClick()
    {
        if (!pointerData.eligibleForClick) return;

        var go = pointerData.pointerCurrentRaycast.gameObject;

        // Send pointer up and click events.
        ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerClickHandler);

        if (pointerData.pointerDrag != null)
            ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.dropHandler);

        if (pointerData.pointerDrag != null && pointerData.dragging)
            ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.endDragHandler);

        // Clear the click state.
        pointerData.pointerPress = null;
        pointerData.rawPointerPress = null;
        pointerData.eligibleForClick = false;
        pointerData.clickCount = 0;
        pointerData.pointerDrag = null;
        pointerData.dragging = false;
    }

    private void HandleTrigger()
    {
        var go = pointerData.pointerCurrentRaycast.gameObject;
        
        // Send pointer down event.
        pointerData.pressPosition = pointerData.position;
        pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
        pointerData.pointerPress =
            ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.pointerDownHandler)
            ?? ExecuteEvents.GetEventHandler<IPointerClickHandler>(go);

        // Save the drag handler as well
        pointerData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(go);
        if (pointerData.pointerDrag != null)
            ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.initializePotentialDrag);

        // Save the pending click state.
        pointerData.rawPointerPress = go;
        pointerData.eligibleForClick = true;
        pointerData.delta = Vector2.zero;
        pointerData.dragging = false;
        pointerData.useDragThreshold = true;
        pointerData.clickCount = 1;
        pointerData.clickTime = Time.unscaledTime;
    }
}