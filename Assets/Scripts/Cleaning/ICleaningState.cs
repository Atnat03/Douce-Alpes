using UnityEngine;

public interface ICleaningState
{
    void EnterState(StateMachineClean manager);
    void UpdateState();
}
