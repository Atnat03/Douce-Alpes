using System.Collections;
using UnityEngine;

public class LeftPosState : ICleaningState
{
    private StateMachineClean manager;
    private Vector3 camPos = new Vector3(0f, 0.75f, 2.5f);
    private const int cleanValueToChange = 40;
    private int leftLayer;

    public void EnterState(StateMachineClean managerC)
    {
        manager = managerC;
        leftLayer = LayerMask.NameToLayer("LeftSide");
        manager.cleanManager.currentCleaningLayer = leftLayer;
        manager.cleanManager.currentCleaningSide = CleaningSide.Left;

        var sheep = manager.cleanManager.sheepTarget;

        manager.cleanManager.canAddShampoo = false;
        manager.cleanManager.StartCoroutine(ChangePositionCamera(camPos, 2f));

        if (manager.cleanManager.currentTool == CleaningTool.Shampoo)
            manager.cleanManager.ResetValueClean();
    }

    public void UpdateState()
    {
        var cam = manager.cleanManager.camera;
        Vector3 direction = manager.cleanManager.sheepTarget.position - cam.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, Time.deltaTime * 5f);

        if (IsEnought() && !(manager.cleanManager.currentTool == CleaningTool.Shower && manager.cleanManager.allCleaned))
        {
            manager.SetState(manager.frontPosState);
        }
    }

    
    private IEnumerator ChangePositionCamera(Vector3 end, float duration)
    {
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

        // Pendant le rinçage : on compte seulement les shampoos du layer courant
        int remaining = 0;
        foreach (var s in manager.cleanManager.shampooList)
        {
            if (s.layer == leftLayer) remaining++;
        }
        return remaining <= 0;
    }
}