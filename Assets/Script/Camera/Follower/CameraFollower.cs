using System;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraFollower : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private CinemachineFollow followComponent;

    [Header("Invert Damping Setting")]
    [SerializeField] private bool isInvertDampingX = false;
    [SerializeField] private bool isResetPositionIfFollowTargetStopMovingX = false;
    [SerializeField] private bool isInvertDampingY = true;
    [SerializeField] private bool isResetPositionIfFollowTargetStopMovingY = true;

    [Header("Damping X")]
    [SerializeField] private float lerpDurationX = 0.5f;
    [SerializeField] private float offSetX = 1f;

    [Header("Damping Y")]
    [SerializeField] private float lerpDurationY = 0.5f;
    [SerializeField] private float offSetY = 0.2f;

    private Vector3 originOffset;
    private Vector3 previousPosition;
    private float startOffsetX;
    private float targetOffsetX;
    private float offsetXIncrease;
    private float elapsedXTime;

    private float startOffsetY;
    private float targetOffsetY;
    private float offsetYIncrease;
    private float elapsedYTime;

    private Transform followTarget => cinemachineCamera.Follow;

    public CinemachineCamera CinemachineCamera => cinemachineCamera;

    private void Start()
    {
        originOffset = followComponent.FollowOffset;
        previousPosition = Vector3.zero;
    }

    private void Update()
    {
        if (followTarget == null)
            return;

        if (isInvertDampingX)
            HandleInvertHorizontalOffset();
        if (isInvertDampingY)
            HandleInvertVerticalOffset();
    }

    private void HandleInvertHorizontalOffset()
    {
        var currentPositionX = (float)Math.Round(followTarget.position.x, 2);
        if (currentPositionX > previousPosition.x)
        {
            offsetXIncrease = offSetX;
        }
        else if (currentPositionX < previousPosition.x)
        {
            offsetXIncrease = -offSetX;
        }
        else
        {
            if (isResetPositionIfFollowTargetStopMovingX)
                offsetXIncrease = 0;
        }

        if (targetOffsetX != offsetXIncrease)
        {
            elapsedXTime = 0;
            startOffsetX = followComponent.FollowOffset.x;
            targetOffsetX = offsetXIncrease;
        }

        previousPosition.x = currentPositionX;

        elapsedXTime += Time.deltaTime;
        var percentToComplete = elapsedXTime / lerpDurationX;

        var lerp = Mathf.Lerp(startOffsetX, originOffset.x + targetOffsetX, Mathf.SmoothStep(0, 1, percentToComplete));
        lerp = (float)Math.Round(lerp, 2);
        followComponent.FollowOffset.x = lerp;
    }

    private void HandleInvertVerticalOffset()
    {
        var currentPositionY = (float)Math.Round(followTarget.position.y, 2);
        if (currentPositionY > previousPosition.y)
        {
            offsetYIncrease = offSetY;
        }
        else if (currentPositionY < previousPosition.y)
        {
            offsetYIncrease = -offSetY;
        }
        else
        {
            if (isResetPositionIfFollowTargetStopMovingY)
                offsetYIncrease = 0;
        }

        if (targetOffsetY != offsetYIncrease)
        {
            elapsedYTime = 0;
            startOffsetY = followComponent.FollowOffset.y;
            targetOffsetY = offsetYIncrease;
        }

        previousPosition.y = currentPositionY;

        elapsedYTime += Time.deltaTime;
        var percentToComplete = elapsedYTime / lerpDurationY;

        var lerp = Mathf.Lerp(startOffsetY, originOffset.y + targetOffsetY, Mathf.SmoothStep(0, 1, percentToComplete));
        lerp = (float)Math.Round(lerp, 2);
        followComponent.FollowOffset.y = lerp;
    }
}