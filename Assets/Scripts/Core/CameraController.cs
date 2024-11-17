using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotationSpeed = 500f;
        [SerializeField] private float scrollSpeed = 7f;
        [SerializeField] private float scrollSensitivity = 1.4f;
        [SerializeField] private float minScroll = 3f;
        [SerializeField] private float maxScroll = 13;
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

        private Vector3 targetFollowOffset;
        private CinemachineTransposer cinemachineTransposer;

        private void Start()
        {
            cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (cinemachineTransposer != null)
            {
                targetFollowOffset = cinemachineTransposer.m_FollowOffset;
            }
        }

        private void Update()
        {
            HandleMovement();
            HandleRotation();
            HandleZoom();
        }

        private void HandleMovement()
        {
            Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();
            Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
            transform.position += moveVector * moveSpeed * Time.deltaTime;
        }

        private void HandleRotation()
        {
            float mouseX = InputManager.Instance.GetCameraRotationAmount();
            Vector3 rotationVector = new Vector3(0, mouseX, 0);
            transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
        }

        private void HandleZoom()
        {
            if (cinemachineTransposer != null)
            {
                float newFollowOffset = targetFollowOffset.y - InputManager.Instance.GetCameraZoomAmount() * scrollSensitivity;
                targetFollowOffset.y = Mathf.Clamp(newFollowOffset, minScroll, maxScroll);
                cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, scrollSpeed * Time.deltaTime);
            }
        }
    }
}
