#define USE_NEW_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        private PlayerInputActions playerInputActions;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one InputManager! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.Enable();
        }

        public Vector2 GetMouseScreenPosition()
        {
#if USE_NEW_INPUT_SYSTEM
            return Mouse.current.position.ReadValue();
#else
            return Input.mousePosition;
#endif
        }

        public bool IsSelectMouseButtonDownThisFrame()
        {
#if USE_NEW_INPUT_SYSTEM
            return playerInputActions.Player.Select.WasPressedThisFrame();
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

        public Vector2 GetCameraMoveVector()
        {
#if USE_NEW_INPUT_SYSTEM
            return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
            Vector2 inputMoveDir = new Vector2(0, 0);

            if (Input.GetKey(KeyCode.W))
            {
                inputMoveDir.y += 1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputMoveDir.y -= 1f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                inputMoveDir.x += 1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputMoveDir.x -= 1f;
            }

            return inputMoveDir;
#endif
        }

        public float GetCameraRotationAmount()
        {
#if USE_NEW_INPUT_SYSTEM
            if (playerInputActions.Player.CameraRotationActive.IsPressed())
            {
                return playerInputActions.Player.CameraRotationValue.ReadValue<float>();
            }
            return 0;
#else
            if(Input.GetMouseButton(1))
            {
                return Input.GetAxis("Mouse X");
            }
            return 0;
#endif
        }

        public float GetCameraZoomAmount()
        {
#if USE_NEW_INPUT_SYSTEM
            return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
            return Input.mouseScrollDelta.y;
#endif
        }
    }
}
