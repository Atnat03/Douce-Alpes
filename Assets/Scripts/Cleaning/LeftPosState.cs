using System.Collections;
using UnityEngine;

public class LeftPosState : ICleaningState
{
    private StateMachineClean manager;

    private Vector3 camPos = new Vector3(0.2f, 0.75f, 3f);

    private const int cleanValueToChange = 40;
    private int leftLayer;

    public void EnterState(StateMachineClean managerC)
    {
        manager = managerC;

        leftLayer = LayerMask.NameToLayer("LeftSide");
        manager.cleanManager.currentCleaningLayer = leftLayer;
        manager.cleanManager.currentCleaningSide = CleaningSide.Left;

        manager.cleanManager.canAddShampoo = false;
        manager.cleanManager.canRotateCamera = false;
        
        AudioManager.instance.PlaySound(25, 1f, 0.25f);

        manager.cleanManager.StartCoroutine(
            RotateAroundSheep(
                manager.cleanManager.camera.transform.position,
                camPos,
                1f
            )
        );

        if (manager.cleanManager.currentTool == CleaningTool.Shampoo)
            manager.cleanManager.ResetValueClean();

        manager.cleanManager.StartNewCycle();
    }

    public void UpdateState()
    {
        if (manager.cleanManager.sheepIsMoving)
            return;
        
        if (IsEnought() &&
            !(manager.cleanManager.currentTool == CleaningTool.Shower &&
              manager.cleanManager.allCleaned))
        {
            manager.SetState(manager.frontPosState);
        }
    }

    // ==========================
    // 🎥 CAMERA ROTATION FLUIDE
    // ==========================
    private IEnumerator RotateAroundSheep(
        Vector3 startPos,
        Vector3 endPos,
        float duration)
    {
        Transform cam = manager.cleanManager.camera.transform;
        Transform focus = manager.cleanManager.focusCam;

        if (cam == null || focus == null)
            yield break;

        Vector3 center = focus.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            Vector3 dirStart = startPos - center;
            Vector3 dirEnd = endPos - center;

            Vector3 dir = Vector3.Slerp(dirStart, dirEnd, t);
            cam.position = center + dir;

            // Toujours regarder le point de nettoyage
            cam.rotation = Quaternion.LookRotation(center - cam.position);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.position = endPos;
        cam.rotation = Quaternion.LookRotation(center - cam.position);

        manager.cleanManager.canAddShampoo = true;
        manager.cleanManager.canRotateCamera = true;
    }

    public bool IsEnought()
    {
        if (manager.cleanManager.currentTool == CleaningTool.Shampoo)
            return manager.cleanManager.GetCleanValue() >= cleanValueToChange;

        int remaining = 0;
        foreach (var s in manager.cleanManager.shampooList)
        {
            if (s.layer == leftLayer)
                remaining++;
        }
        return remaining <= 0;
    }
}
