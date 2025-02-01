using System;

public class InputController
{
    protected InputManager InputManager => GameManager.Instance.InputManager;

    public virtual void Initialize()
    {

    }

    public virtual void EnableInputController(bool value = true)
    {

    }

    public virtual void EnableInputAction(string key, bool value = true)
    {

    }
}