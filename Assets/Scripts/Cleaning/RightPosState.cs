using System.Collections;
using UnityEngine;

public class RightPosState : ICleaningState
{
    private StateMachineClean manager;
    private Vector3 camPos = new Vector3(0.1f, 0.75f, -2.5f);
    private const int cleanValueToChange = 40;
    private int rightLayer;

    public void EnterState(StateMachineClean managerC)
    {
        manager = managerC;
        rightLayer = LayerMask.NameToLayer("RightSide");
        manager.cleanManager.currentCleaningLayer = rightLayer;
        manager.cleanManager.currentCleaningSide = CleaningSide.Right;

        manager.cleanManager.canAddShampoo = false;
        manager.cleanManager.StartCoroutine(ChangePositionCamera(camPos, 2f));

        if (manager.cleanManager.currentTool == CleaningTool.Shampoo)
            manager.cleanManager.ResetValueClean();
    }

    public void UpdateState()
    {
        // Smooth rotation vers le target
        var cam = manager.cleanManager.camera.transform;
        Vector3 direction = manager.cleanManager.sheepTarget.position - cam.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        cam.rotation = Quaternion.Slerp(cam.rotation, targetRotation, Time.deltaTime * 5f);

        if (IsEnought() && !(manager.cleanManager.currentTool == CleaningTool.Shower && manager.cleanManager.allCleaned))
        {
            if (manager.cleanManager.currentTool == CleaningTool.Shampoo)
            {
                manager.cleanManager.ResetValueClean();
                manager.cleanManager.SetShower();
                Debug.Log("🚿 Auto-switch vers Douche ! Débute rinçage sur Gauche...");
            }
            manager.SetState(manager.leftPosState);
        }
    }

    public void ExitState() { }

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

        int remaining = 0;
        foreach (var s in manager.cleanManager.shampooList)
        {
            if (s.layer == rightLayer) remaining++;
        }
        return remaining <= 0;
    }
}
