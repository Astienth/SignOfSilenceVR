using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Donteco;

namespace SignOfSilenceVR;

public class VrLaser : MonoBehaviour
{
    private const float laserLength = 5f;
    public SteamVR_Input_Sources m_inputSource = SteamVR_Input_Sources.RightHand;
    public SteamVR_Action_Boolean m_clickAction = SteamVR_Actions.default_confirm;
    private bool ignoreNextInput;

    public LaserInputModule inputModule;
    private LineRenderer lineRenderer;
    private Vector3? target = null;

    public static VrLaser Create(Transform dominantHand)
    {
        var instance = new GameObject("VrHandLaser").AddComponent<VrLaser>();
        var instanceTransform = instance.transform;
        instanceTransform.SetParent(dominantHand, false);
        instanceTransform.localEulerAngles = new Vector3(39.132f, 356.9302f, 0.3666f);
        return instance;
    }

    public void SetUp(Camera camera)
    {
        inputModule.EventCamera = camera;
    }

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.SetPositions(new[] {Vector3.zero, Vector3.forward * laserLength});
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.001f;
        lineRenderer.endColor = new Color(1, 1, 1, 0.8f);
        lineRenderer.startColor = Color.white;
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.sortingOrder = 10000;
        lineRenderer.enabled = false;

        inputModule = LaserInputModule.Create(this);
    }

    private void Update()
    {
        UpdateLaserTarget();
    }

    public void SetTarget(Vector3? newTarget)
    {
        target = newTarget;
    }

    public void UpdateLaserTarget()
    {
        lineRenderer.SetPosition(1,
            target != null
                ? transform.InverseTransformPoint((Vector3) target)
                : Vector3.forward * laserLength);
    }

    public void UpdateLaserVisibility(bool enabled, GameObject target)
    {
        if(!enabled)
        {
            lineRenderer.enabled = false;
            return;
        }
        /*
        if(target && target.transform.Find("Hearts"))
        {
            Logs.WriteWarning("TARGET NAME "+target.name);
            Logs.WriteWarning("HEARTS FOUND");
            if (ElementUIHelper.IsFullyVisibleFrom(
                target.transform.Find("Hearts").GetComponent<RectTransform>(),
                VRHands.getCamera().GetComponent<Camera>()))
            {

                Logs.WriteWarning("HEARTS IN VIEW");
                lineRenderer.enabled = false;
                return;
            }
            Logs.WriteWarning("HEARTS HIDDEN");
            lineRenderer.enabled = true;
            return;
        }
        */
        lineRenderer.enabled = true;
    }

    public bool ClickDown()
    {
        if (ignoreNextInput) return false;
        return m_clickAction.GetStateDown(m_inputSource);
    }

    public bool ClickUp()
    {
        if (ignoreNextInput)
        {
            ignoreNextInput = false;
            return false;
        }

        return m_clickAction.GetStateUp(m_inputSource);
    }
}