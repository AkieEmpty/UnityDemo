using System.Collections.Generic;
using JKFrame;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AkieEmpty.InputSystem
{
    public enum InputKey
    {
        LeftShift
    }
    public static class PlayerInput
    {
        private static InputAction moveInputAction;
        private static InputAction walkAndRunAction;
        static PlayerInput() 
        {
            Init();
        }

        private static void Init()
        {
            InputActionAsset actionAsset = ResSystem.LoadAsset<InputActionAsset>(nameof(InputActionAsset));
            moveInputAction = actionAsset.FindAction("Move");
            walkAndRunAction = actionAsset.FindAction("WalkAndRun");
        }

        public static Vector2 GetMoveAxis()
        {
            return moveInputAction.ReadValue<Vector2>();
        }
        public static float GetHorizontalAxis()
        {
            return moveInputAction.ReadValue<Vector2>().x;
        }
        public static float GetVerticalAxis()
        {
            return moveInputAction.ReadValue<Vector2>().y;
        }

        public static bool GetKeyDown(InputKey keyCode)
        {
            switch (keyCode)
            {
                case InputKey.LeftShift:
                    return walkAndRunAction.WasPressedThisFrame();
                default:
                     return false;
            }
        }
        public static bool GetKeyUp(InputKey keyCode)
        {
            switch (keyCode)
            {
                case InputKey.LeftShift:
                    return walkAndRunAction.WasReleasedThisFrame();
                default:
                    return false;
            }
        }
    }
}
