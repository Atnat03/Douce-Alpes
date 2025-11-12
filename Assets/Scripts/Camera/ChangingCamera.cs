using System;
using System.Collections;
using UnityEngine;

public class ChangingCamera : MonoBehaviour
{
    [SerializeField] private Camera camera;
    public float timerToTransition = 1f;

    private CameraControl control;
    private CameraFollow follow;

    private Vector3 preTransitionCamPos;
    private Quaternion preTransitionCamRot;
    private Vector3 preTransitionRootPos;
    private Quaternion preTransitionRootRot;

    private void Awake()
    {
        if (camera == null) camera = Camera.main;
        control = GetComponent<CameraControl>();
        follow = GetComponent<CameraFollow>();
    }

    private void OnEnable()
    {
        GameManager.instance.SheepClicked += LockCamOnSheep;
        GameManager.instance.SheepHold += ChangeCamera;
        GameManager.instance.GrangeClicked += ChangeCamera;
        GameManager.instance.AbreuvoirClicked += ChangeCamera;
        GameManager.instance.NicheClicked += ChangeCamera;
        GameManager.instance.OnClickOnShop += ChangeCamera;
    }

    private void OnDisable()
    {
        GameManager.instance.SheepClicked -= LockCamOnSheep;
        GameManager.instance.SheepHold -= ChangeCamera;
        GameManager.instance.GrangeClicked -= ChangeCamera;
        GameManager.instance.AbreuvoirClicked -= ChangeCamera;
        GameManager.instance.NicheClicked -= ChangeCamera;
        GameManager.instance.OnClickOnShop -= ChangeCamera;
    }

    private IEnumerator SmoothTransition(Vector3 targetPos, Vector3 targetEuler, Transform target, bool reEnableControl = false)
    {
        // Sauvegarde de la position AVANT transition
        preTransitionCamPos = camera.transform.position;
        preTransitionCamRot = camera.transform.rotation;
        preTransitionRootPos = control.root.position;
        preTransitionRootRot = control.root.rotation;

        control.enabled = false;
        follow.enabled = false;

        Quaternion targetRot = Quaternion.Euler(targetEuler);

        while (Vector3.Distance(camera.transform.position, targetPos) > 0.01f ||
               Quaternion.Angle(camera.transform.rotation, targetRot) > 0.1f)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, targetPos, Time.deltaTime * 5f);
            camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, targetRot, Time.deltaTime * 5f);
            yield return null;
        }

        camera.transform.position = targetPos;
        camera.transform.rotation = targetRot;

        if (reEnableControl) control.enabled = true;
    }

    public void ChangeCamera(Vector3 newPosition, Vector3 rotation, Transform target)
    {
        StartCoroutine(SmoothTransition(newPosition, rotation, target));
    }

    public void LockCamOnSheep(Sheep sheep)
    {
        control.enabled = false;
        follow.enabled = true;
        follow.target = sheep.transform;
        follow.offset = camera.transform.position;
        sheep.ChangeOutlineState(true);
    }

    public void ResetCameraLock(Sheep sheep)
    {
        if (GameManager.instance.getCurLockSheep() != null)
            GameManager.instance.getCurLockSheep().gameObject.GetComponent<Sheep>().isOpen = false;

        control.enabled = true;
        follow.enabled = false;
        sheep.ChangeOutlineState(false);
    }

    public void ResetPosition()
    {
        StartCoroutine(SmoothReset());
    }

    private IEnumerator SmoothReset()
    {
        float elapsed = 0f;

        Vector3 startCamPos = camera.transform.position;
        Quaternion startCamRot = camera.transform.rotation;
        Vector3 startRootPos = control.root.position;
        Quaternion startRootRot = control.root.rotation;

        control.enabled = false;
        follow.enabled = false;
        
        GameManager.instance.ResetCamera();
    
        while (elapsed < timerToTransition)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / timerToTransition);

            camera.transform.position = Vector3.Lerp(startCamPos, preTransitionCamPos, t);
            camera.transform.rotation = Quaternion.Slerp(startCamRot, preTransitionCamRot, t);

            control.root.position = Vector3.Lerp(startRootPos, preTransitionRootPos, t);
            control.root.rotation = Quaternion.Slerp(startRootRot, preTransitionRootRot, t);

            yield return null;
        }

        camera.transform.position = preTransitionCamPos;
        camera.transform.rotation = preTransitionCamRot;
        control.root.position = preTransitionRootPos;
        control.root.rotation = preTransitionRootRot;

        yield return null;
        
        control.enabled = true;
    }

}
