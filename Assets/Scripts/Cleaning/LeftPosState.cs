using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class LeftPosState : ICleaningState
{
    private StateMachineClean manager;
    
    private Vector3 camPos = new Vector3(0, 0.25f, 2.5f);

    private int cleanValueToChange = 40;
    private int numberOfShampooInList;
    
    public void EnterState(StateMachineClean managerC)
    {
        manager = managerC;
        
        numberOfShampooInList = manager.cleanManager.GetCounterListShampoo();
        
        manager.cleanManager.StartCoroutine(ChangePositionCamera(manager.cleanManager.camera.transform.position, camPos, 1f));
        manager.cleanManager.ResetValueClean();
    }

    public void UpdateState()
    {
        manager.cleanManager.camera.transform.LookAt(manager.cleanManager.sheepTarget);

        if (IsEnought())
        {
            manager.SetState(manager.frontPosState);
        }
    }
    
    private IEnumerator ChangePositionCamera(Vector3 start, Vector3 end, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t =  elapsedTime / duration;
            
            manager.cleanManager.camera.transform.position = Vector3.Slerp(start, end, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    
    public bool IsEnought()
    {
        if(manager.cleanManager.currentTool == CleaningTool.Shampoo)
            return manager.cleanManager.GetCleanValue() >= cleanValueToChange;
        
        return manager.cleanManager.GetCounterListShampoo() <= numberOfShampooInList-cleanValueToChange;
    }
}
