using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftPosState : ICleaningState
{
    private StateMachineClean manager;
    private Vector3 camPos = new Vector3(0, 0.25f, 2.5f);
    private int cleanValueToChange = 40;

    private int leftLayer;
    
    public void EnterState(StateMachineClean managerC)
    {
        manager = managerC;

        leftLayer = LayerMask.NameToLayer("LeftSide");
        manager.cleanManager.currentCleaningLayer = leftLayer;
        
        manager.cleanManager.currentCleaningSide = CleaningSide.Left;

        manager.cleanManager.StartCoroutine(ChangePositionCamera(manager.cleanManager.camera.transform.position, camPos, 1f));
        if(manager.cleanManager.currentTool == CleaningTool.Shampoo)
            manager.cleanManager.ResetValueClean();
    }

    public void UpdateState()
    {
        manager.cleanManager.camera.transform.LookAt(manager.cleanManager.sheepTarget);

        if (IsEnought() && !((manager.cleanManager.currentTool == CleaningTool.Shower) && manager.cleanManager.allCleaned))
        {
            manager.SetState(manager.frontPosState);
        }
    }

    private IEnumerator ChangePositionCamera(Vector3 start, Vector3 end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            manager.cleanManager.camera.transform.position = Vector3.Slerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public bool IsEnought()
    {
        if(manager.cleanManager.currentTool == CleaningTool.Shampoo)
            return manager.cleanManager.GetCleanValue() >= cleanValueToChange;

        int remaining = 0;
        foreach(var s in manager.cleanManager.shampooList)
        {
            if(s.layer == leftLayer)
                remaining++;
        }

        return remaining <= 0;
    }
}