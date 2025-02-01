using System;
using Cysharp.Threading.Tasks;

public abstract class State<T> where T : Enum
{
    public T CurrentState { get; protected set; }

    public State(T stateEnum)
    {
        CurrentState = stateEnum;
    }

    public virtual async UniTask OnEnter() { await UniTask.CompletedTask; }
    public virtual void OnUpdate() { }
    public virtual async UniTask OnExit() { await UniTask.CompletedTask; }
}