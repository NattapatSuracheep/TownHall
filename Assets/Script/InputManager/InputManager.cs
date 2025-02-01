using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class InputManager : MonoBehaviour
{
    protected InputSystemActions inputActions;
    private InputController[] inputControllerList = new InputController[]
    {
        new UiInputController(),
    };

    public Vector2 CurrentPointPosition { get; protected set; }
    public InputSystemActions InputActionsAssets => inputActions;
    public event Action OnUiLeftClickStartedEvent;
    public event Action OnUiLeftClickCanceledEvent;
    public event Action OnUiCancelStartedEvent;

    public virtual void Initialize()
    {
        Log.Call();

        inputActions = new();

        InitializeController();

        EnhancedTouchSupport.Enable(); //Enable this to get more information about touch point on screen
        TouchSimulation.Enable(); //Enable this to support multiple touch on mobile ¯\_(ツ)_/¯
    }

    // private void Update()
    // {
    //     InputSystem.Update();
    // }

    private void InitializeController()
    {
        for (var i = 0; i < inputControllerList.Length; i++)
        {
            var inputController = inputControllerList[i];
            inputController.Initialize();
            inputController.EnableInputController();
        }
    }

    public void EnableInputController(Type type) => inputControllerList.Single(x => x.GetType() == type).EnableInputController();
    public void DisableInputController(Type type) => inputControllerList.Single(x => x.GetType() == type).EnableInputController(false);
    public void EnableInputAction(Type type, string key) => inputControllerList.Single(x => x.GetType() == type).EnableInputAction(key);
    public void DisableInputAction(Type type, string key) => inputControllerList.Single(x => x.GetType() == type).EnableInputAction(key, false);

    public void OnUILeftClickStarted(InputAction.CallbackContext context) => OnUiLeftClickStartedEvent?.Invoke();
    public void OnUILeftClickCanceled(InputAction.CallbackContext context) => OnUiLeftClickCanceledEvent?.Invoke();
    public void OnUICancelStarted(InputAction.CallbackContext context) => OnUiCancelStartedEvent?.Invoke();
    public void OnUIPointerPerformed(InputAction.CallbackContext context)
    {
        var pos = context.ReadValue<Vector2>();
        CurrentPointPosition = pos;
    }

    public void OnTestEventStarted(InputAction.CallbackContext context) => Log.Call();
    public void OnTestEventPerformed(InputAction.CallbackContext context) => Log.Call();
    public void OnTestEventCanceled(InputAction.CallbackContext context) => Log.Call();
}