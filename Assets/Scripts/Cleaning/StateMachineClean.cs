using System;
using UnityEngine;

public class StateMachineClean : MonoBehaviour
{
    [HideInInspector] public LeftPosState leftPosState;
    [HideInInspector] public FrontPosState frontPosState;
    [HideInInspector] public RightPosState rightPosState;
    
    public ICleaningState currentState;
    
    public CleanManager cleanManager;

    private void Start()
    {
        InitializedStates();
    }

    void InitializedStates()
    {
        leftPosState = new LeftPosState();
        frontPosState = new FrontPosState();
        rightPosState = new RightPosState();
        
        SetState(leftPosState);
    }
    
    private void Update()
    {
        currentState.UpdateState();
    }

    public void SetState(ICleaningState state)
    {
        Debug.Log("Change State to " + state.GetType().Name);
        
        currentState = state;
        state.EnterState(this);
    }
}
