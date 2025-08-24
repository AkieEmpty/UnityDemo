using JKFrame;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AkieEmpty.InputSystem
{
    public static class PlayerInput
    {
        private static InputAction moveInputAction;
        static PlayerInput() 
        {
            Init();
        }

        private static void Init()
        {
            InputActionAsset actionAsset = ResSystem.LoadAsset<InputActionAsset>(nameof(InputActionAsset));
            moveInputAction = actionAsset.FindAction("Move");
        }

        public static Vector2 GetMoveInput()
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
    }
}
