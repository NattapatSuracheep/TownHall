using System;
using Cysharp.Threading.Tasks;

public class CommonState : State<Enum>
{
    private Action onEnter, onExit;

    public CommonState(Enum stateEnum, Action onEnter, Action onExit) : base(stateEnum)
    {
        this.onEnter = onEnter;
        this.onExit = onExit;
    }

    public override async UniTask OnEnter()
    {
        await base.OnEnter();

        onEnter?.Invoke();
    }

    public override async UniTask OnExit()
    {
        await base.OnExit();

        onExit?.Invoke();
    }
}