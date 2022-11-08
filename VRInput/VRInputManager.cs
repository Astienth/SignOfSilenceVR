using UnityEngine;
using Valve.VR;
using Donteco;
using System;

namespace SignOfSilenceVR
{
    public class VRInputManager
    {
        public static bool rotateLeft = false;
        public static bool rotateRight = false;
        public static Vector2 axisMove = Vector2.zero;

        static VRInputManager()
        {
            SetUpListeners();
        }

        public static void SetUpListeners()
        {
            // BOOLEANS
            SteamVR_Actions._default.TriggerRight.AddOnStateUpListener(UseItem, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.TriggerLeft.AddOnStateUpListener(Flashlight, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.grabright.AddOnStateDownListener(GrabRightDown, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.grabright.AddOnStateUpListener(GrabRightUp, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.grableft.AddOnUpdateListener(Run, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.grableft.AddOnStateUpListener(GrabLeftUp, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.confirm.AddOnStateUpListener(Confirm, SteamVR_Input_Sources.Any); 
            SteamVR_Actions._default.recenter.AddOnChangeListener(Recenter, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.actionbar.AddOnStateUpListener(ActionBar, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.decline.AddOnStateUpListener(Decline, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.throwitem.AddOnChangeListener(throwItem, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.pause.AddOnStateUpListener(Pause, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.jump.AddOnStateUpListener(Jump, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.crouch.AddOnStateUpListener(Crouch, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.SwapTurnLeft.AddOnUpdateListener(SwapTurnLeft, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.SwapTurnRight.AddOnUpdateListener(SwapTurnRight, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.highlight.AddOnStateDownListener(Map, SteamVR_Input_Sources.Any);
            // VECTOR 2Ds
            SteamVR_Actions._default.move.AddOnUpdateListener(Move, SteamVR_Input_Sources.Any);

            // POSES
            SteamVR_Actions._default.RightHandPose.AddOnUpdateListener(SteamVR_Input_Sources.Any, UpdateRightHand);
            SteamVR_Actions._default.LeftHandPose.AddOnUpdateListener(SteamVR_Input_Sources.Any, UpdateLeftHand);
        }

        //Vector1
        private static void throwItem(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
        {
            if (newAxis == 1)
            {
                InputSystem.SimulateButton(InputKeys.ThrowItem);
            }
        }

        private static void Recenter(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
        {
            if (newAxis == 1)
            {
                CameraManager.resetPlayerHeadPosition();
            }
        }

        //booleans
        private static void Run(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            if (newState)
            {
                InputSystem.SimulateButton(InputKeys.Run);
            }
        }
        private static void SwapTurnLeft(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            if (newState)
            {
                rotateLeft = true;
            }
            else
            {
                rotateLeft = false;
            }
        }
        private static void SwapTurnRight(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            if (newState)
            {
                rotateRight = true;
            }
            else
            {
                rotateRight = false;
            }
        }

        public static void Map(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            InputSystem.SimulateButton(InputKeys.OpenMiniMap);
        }
        
        public static void UseItem(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            InputSystem.SimulateButton(InputKeys.UseItem);
        }

        public static void Jump(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            InputSystem.SimulateButton(InputKeys.Jump);
        }

        public static void Pause(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            InputSystem.SimulateButton(InputKeys.OpenCloseInventory);
        }

        public static void Decline(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            VRHands.SpawnHands();
        }

        public static void ActionBar(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {

        }

        public static void Crouch(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            InputSystem.SimulateButton(InputKeys.Crouch);
        }

        public static void Confirm(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            InputSystem.SimulateButton(InputKeys.Action);
        }

        public static void GrabRightDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            
        }
        public static void GrabRightUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            
        }

        public static void Flashlight(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            InputSystem.SimulateButton(InputKeys.Flashlight);
        }

        public static void GrabLeftUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            
        }
        // VECTOR 2Ds
        public static void Move(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            if (axis != Vector2.zero)
            {
                axisMove = axis;
            }
            else
            {
                axisMove = Vector2.zero;
            }
        }
        // POSES
        public static void UpdateRightHand(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
        {
            if (VRHands.RightHand)
            {
                VRHands.RightHand.transform.localPosition = fromAction.localPosition;
                VRHands.RightHand.transform.localRotation = fromAction.localRotation;
            }
        }

        public static void UpdateLeftHand(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
        {
            if (VRHands.LeftHand)
            {
                VRHands.LeftHand.transform.localPosition = fromAction.localPosition;
                VRHands.LeftHand.transform.localRotation = fromAction.localRotation;
            }
        }
    }
}