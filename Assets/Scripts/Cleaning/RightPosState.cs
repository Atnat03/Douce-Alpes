using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightPosState : ICleaningState
{
    private StateMachineClean manager;
    
    private Vector3 camPos = new Vector3(0.1f, 0.75f, -2.5f);
    
    private int cleanValueToChange = 40;
    
    private int rightLayer;

    
    public void EnterState(StateMachineClean managerC)
    {
        manager = managerC;
        
        rightLayer = LayerMask.NameToLayer("RightSide");
        manager.cleanManager.currentCleaningLayer = rightLayer;
        
        manager.cleanManager.currentCleaningSide = CleaningSide.Right;
        manager.cleanManager.canAddShampoo = false;

        manager.cleanManager.StartCoroutine(ChangePositionCamera(manager.cleanManager.camera.transform.position, camPos, 2f));
        if(manager.cleanManager.currentTool == CleaningTool.Shampoo)
        {
            manager.cleanManager.ResetValueClean();
        }
    }

    public void UpdateState()
    {
        manager.cleanManager.camera.transform.LookAt(manager.cleanManager.sheepTarget);

        if (IsEnought() && !((manager.cleanManager.currentTool == CleaningTool.Shower) && manager.cleanManager.allCleaned))
        {
            manager.cleanManager.SetShampoo();
            manager.SetState(manager.leftPosState);
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
        manager.cleanManager.canAddShampoo = true;

    }
    
    public bool IsEnought()
    {
        if(manager.cleanManager.currentTool == CleaningTool.Shampoo)
            return manager.cleanManager.GetCleanValue() >= cleanValueToChange;

        int remaining = 0;
        foreach(var s in manager.cleanManager.shampooList)
        {
            if(s.layer == rightLayer)
                remaining++;
        }

        return remaining <= 0;
    }
    
    

}
