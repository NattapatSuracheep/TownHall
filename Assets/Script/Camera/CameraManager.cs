using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private Camera uiCam;
    [SerializeField] private CinemachineBrain mainCamBrain;

    private CameraFollowerController cameraFollowerController = new();

    public Camera MainCamera => mainCam;
    public Camera UiCamera => uiCam;

    public void ChangeCinemachineBlendSetting(CinemachineBlenderSettings blendSettings)
    {
        mainCamBrain.CustomBlends = blendSettings;
    }

    public float GetBlendTime(string from, string to)
    {
        if (mainCamBrain.CustomBlends == null)
            return 0f;

        var setting = mainCamBrain.CustomBlends.CustomBlends.FirstOrDefault(x => x.From == from && x.To == to);
        return setting.Blend.Time;
    }

    public void ChangeCameraTarget(CinemachineCamera targetedCamera) => cameraFollowerController.ChangeCameraTarget(targetedCamera);
    public void ChangeFollowTarget(Transform target) => cameraFollowerController.ChangeFollowTarget(target);
    public void ChangeLookAtTarget(Transform target) => cameraFollowerController.ChangeLookAtTarget(target);
    public void ResetLookAt() => cameraFollowerController.ResetLookAt();
}