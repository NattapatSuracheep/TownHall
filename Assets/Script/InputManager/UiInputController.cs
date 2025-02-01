using UnityEngine.InputSystem;

public class UiInputController : InputController
{
    private InputAction uiLeftClickAction;
    private InputAction uiPointAction;
    private InputAction uiCancelAction;

    public override void Initialize()
    {
        GetInputActionMap();

        uiLeftClickAction.started += InputManager.OnUILeftClickStarted;
        uiLeftClickAction.canceled += InputManager.OnUILeftClickCanceled;

        uiPointAction.performed += InputManager.OnUIPointerPerformed;
        uiCancelAction.started += InputManager.OnUICancelStarted;
    }

    public override void EnableInputController(bool value = true)
    {
        base.EnableInputController(value);

        if (value)
            InputManager.InputActionsAssets.UI.Enable();
        else
            InputManager.InputActionsAssets.UI.Disable();
    }

    private void GetInputActionMap()
    {
        uiLeftClickAction = InputManager.InputActionsAssets.UI.Click;
        uiPointAction = InputManager.InputActionsAssets.UI.Point;
        uiCancelAction = InputManager.InputActionsAssets.UI.Cancel;
    }
}