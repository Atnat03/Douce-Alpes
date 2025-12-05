using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FrontPosState : ICleaningState
{
    private StateMachineClean manager;

    private Vector3 camPos = new Vector3(2, 0.75f, 0);
    
    private int cleanValueToChange = 20;
    
    private int frontLayer;

    
    public void EnterState(StateMachineClean managerC)
    {
        manager = managerC;
        
        frontLayer = LayerMask.NameToLayer("FrontSide");
        manager.cleanManager.currentCleaningLayer = frontLayer;    
        
        manager.cleanManager.currentCleaningSide = CleaningSide.Front;
        manager.cleanManager.canAddShampoo = false; 
        manager.cleanManager.StartCoroutine(ChangePositionCamera(manager.cleanManager.camera.transform.position, camPos, 2f));
        if(manager.cleanManager.currentTool == CleaningTool.Shampoo)
            manager.cleanManager.ResetValueClean();
    }

    public void UpdateState()
    {
        manager.cleanManager.camera.transform.LookAt(manager.cleanManager.sheepTarget);
        
        if (IsEnought() && !((manager.cleanManager.currentTool == CleaningTool.Shower) && manager.cleanManager.allCleaned))
        {
            manager.SetState(manager.rightPosState);
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
            if(s.layer == frontLayer)
                remaining++;
        }

        return remaining <= 0;
    }
}
