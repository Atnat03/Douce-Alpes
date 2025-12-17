using System.Collections;
using UnityEngine;

public class FrontPosState : ICleaningState
{
    private StateMachineClean manager;
    private Vector3 camPos = new Vector3(2.5f, 0.75f, 0f);
    private const int cleanValueToChange = 20;
    private int frontLayer;

    public void EnterState(StateMachineClean managerC)
    {        

        manager = managerC;
        frontLayer = LayerMask.NameToLayer("FrontSide");
        manager.cleanManager.currentCleaningLayer = frontLayer;
        manager.cleanManager.currentCleaningSide = CleaningSide.Front;

        manager.cleanManager.canAddShampoo = false;
        manager.cleanManager.StartCoroutine(ChangePositionCamera(camPos, 1f));

        if (manager.cleanManager.currentTool == CleaningTool.Shampoo)
            manager.cleanManager.ResetValueClean();
    }

    public void UpdateState()
    {
        if (manager.cleanManager.sheepIsMoving)
            return;
        
        var cam = manager.cleanManager.camera.transform;
        Vector3 direction = manager.cleanManager.sheepTarget.position - cam.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        cam.rotation = Quaternion.Slerp(cam.rotation, targetRotation, Time.deltaTime * 5f);
        
        if (IsEnought() && !(manager.cleanManager.currentTool == CleaningTool.Shower && manager.cleanManager.allCleaned))
        {
            manager.SetState(manager.rightPosState);
        }
    }
    
    private IEnumerator ChangePositionCamera(Vector3 end, float duration)
    {
        if (manager.cleanManager.sheepIsMoving)
            yield break;
        
        Transform cam = manager.cleanManager.camera.transform;
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;
        Vector3 targetDir = manager.cleanManager.sheepTarget.position - end;
        Quaternion endRot = Quaternion.LookRotation(targetDir);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            cam.position = Vector3.Slerp(startPos, end, t);
            cam.rotation = Quaternion.Slerp(startRot, endRot, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cam.position = end;
        cam.rotation = endRot;
        manager.cleanManager.canAddShampoo = true;
    }

    public bool IsEnought()
    {
        if (manager.cleanManager.currentTool == CleaningTool.Shampoo)
            return manager.cleanManager.GetCleanValue() >= cleanValueToChange;

        int remaining = 0;
        foreach (var s in manager.cleanManager.shampooList)
        {
            if (s.layer == frontLayer) remaining++;
        }
        return remaining <= 0;
    }
}
