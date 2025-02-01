using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CanvasScreenCamera : MonoBehaviour
{
    [SerializeField] private int sortOrder = 1;
    [SerializeField, ReadOnly] Canvas canvas;
    [SerializeField, ReadOnly] CanvasScaler canvasScaler;

    private void OnValidate()
    {
        if (canvas == null)
            canvas = GetComponent<Canvas>();

        if (canvasScaler == null)
            canvasScaler = GetComponent<CanvasScaler>();
    }

    private void Start()
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = GameManager.Instance.UiCamera;
        canvas.planeDistance = 1;
        canvas.sortingOrder = sortOrder;

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvasScaler.matchWidthOrHeight = 1;
    }
}