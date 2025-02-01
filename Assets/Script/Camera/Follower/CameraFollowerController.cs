using Unity.Cinemachine;
using UnityEngine;

public class CameraFollowerController
{
    private CinemachineCamera targetedCamera;
    private Quaternion originRotation;

    public void ChangeCameraTarget(CinemachineCamera targetedCamera)
    {
        this.targetedCamera = targetedCamera;
        originRotation = targetedCamera.transform.rotation;
    }

    public void ChangeFollowTarget(Transform target)
    {
        targetedCamera.Follow = target;
    }

    public void ChangeLookAtTarget(Transform target)
    {
        if (target == null)
        {
            ResetLookAt();
            return;
        }

        targetedCamera.LookAt = target;
    }

    public void ResetLookAt()
    {
        targetedCamera.LookAt = null;
        targetedCamera.transform.rotation = originRotation;
    }
}