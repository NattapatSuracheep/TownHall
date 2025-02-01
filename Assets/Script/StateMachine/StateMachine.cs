using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StateMachine<T> where T : Enum
{
    private Dictionary<T, State<T>> stateDict;

    private State<T> currentState;

    private T fallbackState;

    public T CurrentStateEnum => currentState.CurrentState;

    public StateMachine(State<T>[] states)
    {
        fallbackState = states[0].CurrentState;
        InitializeStateDictionary(states);
    }

    public async UniTask InitializeAsync()
    {
        Log.Call();

        await TransitionToStateAsync(fallbackState);
    }

    private void InitializeStateDictionary(State<T>[] states)
    {
        Log.Call();

        stateDict = new();
        for (var i = 0; i < states.Length; i++)
        {
            var state = states[i];
            stateDict.Add(state.CurrentState, state);
        }
    }

    public async UniTask TransitionToStateAsync(T state)
    {
        if (currentState != null)
        {
            Debug.Log($"--Exiting state: {currentState.CurrentState}");
            await currentState.OnExit();
        }

        if (!stateDict.ContainsKey(state))
        {
            Debug.LogError($"State {state} does not exist. Fallback to {fallbackState} state instead.");
            currentState = stateDict[fallbackState];
        }
        else
        {
            currentState = stateDict[state];
        }

        Debug.Log($">>Entering state: {currentState.CurrentState}");
        await currentState.OnEnter();
    }

    public void Update()
    {
        currentState?.OnUpdate();
    }
}