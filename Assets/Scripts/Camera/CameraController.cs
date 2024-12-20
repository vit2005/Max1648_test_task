using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private EnemyAI enemyAI2;

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 20f;

    private CinemachineTransposer cinemachineTranposer;
    private Vector3 targetFollowOffset;
    private Transform targetFollow;

    private void Awake()
    {
        enemyAI.OnActionStarted += (Unit u) => { targetFollow = u.transform; };
        enemyAI.OnActionFinished += () => { targetFollow = null; };
        enemyAI2.OnActionStarted += (Unit u) => { targetFollow = u.transform; };
        enemyAI2.OnActionFinished += () => { targetFollow = null; };
    }

    private void Start()
    {
        cinemachineTranposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTranposer.m_FollowOffset;
    }
    private void Update()
    {
        if (targetFollow != null)
        {
            transform.position = targetFollow.position;
            return;
        }

        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();

        float moveSpeed = 10f;

        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;

    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();


        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;

    }

    private void HandleZoom()
    {
        //Debug.Log(InputManager.Instance.GetCameraZoomAmount());
        float zoomIncreaseAmount = 1f;
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;


        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        float zoomSpeed = 5f;
        cinemachineTranposer.m_FollowOffset = Vector3.Lerp(cinemachineTranposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);

    }
}
