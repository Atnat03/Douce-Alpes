using System.Collections;
using UnityEngine;

public class RightPosState : ICleaningState
{
    private StateMachineClean manager;
    private Vector3 camPos = new Vector3(0.2f, 0.75f, -3f);
    private const int cleanValueToChange = 40;
    private int rightLayer;

    
    public void EnterState(StateMachineClean managerC)
    {
        manager = managerC;
        rightLayer = LayerMask.NameToLayer("RightSide");
        manager.cleanManager.currentCleaningLayer = rightLayer;
        manager.cleanManager.currentCleaningSide = CleaningSide.Right;

        manager.cleanManager.canAddShampoo = false;
        manager.cleanManager.StartCoroutine(ChangePositionCamera(camPos, 1f));

        if (manager.cleanManager.currentTool == CleaningTool.Shampoo)
            manager.cleanManager.ResetValueClean();
    }

    public void UpdateState()
    {
        if (manager.cleanManager.sheepIsMoving)
            return;

        if (manager.cleanManager.camera != null && manager.cleanManager.sheepTarget != null)
        {
            var cam = manager.cleanManager.camera.transform;
            Vector3 direction = manager.cleanManager.sheepTarget.position - cam.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            cam.rotation = Quaternion.Slerp(cam.rotation, targetRotation, Time.deltaTime * 5f);
        }

        if (IsEnought())
        {
            if (manager.cleanManager.currentTool == CleaningTool.Shampoo)
            {
                manager.cleanManager.ResetValueClean();
                manager.cleanManager.SetShower();
                manager.SetState(manager.leftPosState);
            }
            else if (manager.cleanManager.currentTool == CleaningTool.Shower)
            {
                if (manager.cleanManager.shampooList.Count == 0 && !manager.cleanManager.allCleaned)
                {
                    manager.SetState(manager.leftPosState);
                    manager.cleanManager.allCleaned = true;
                    manager.cleanManager.OnAllCleaned();
                }
                else if (manager.cleanManager.shampooList.Count > 0)
                {
                    manager.SetState(manager.leftPosState);
                }
            }
        }
    }
    
    private IEnumerator ChangePositionCamera(Vector3 end, float duration)
    {
        if (manager.cleanManager.camera == null || manager.cleanManager.sheepTarget == null)
            yield break;

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
            if (cam == null) yield break;
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
            if (s.layer == rightLayer) remaining++;
        }
        return remaining <= 0;
    }
}
