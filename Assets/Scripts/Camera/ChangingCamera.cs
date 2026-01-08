using System;
using System.Collections;
using UnityEngine;

public class ChangingCamera : MonoBehaviour
{
    [SerializeField] private Camera camera;
    public float timerToTransition = 1f;
    public float transitionCooldown = 0.5f;
    [SerializeField] private float finalRotationTime = 0.5f;

    private CameraControl control;
    private CameraFollow follow;

    private Vector3 preTransitionCamPos;
    private Quaternion preTransitionCamRot;
    private Vector3 preTransitionRootPos;
    private Quaternion preTransitionRootRot;
    private bool hasPreTransition = false;

    public bool isInTransition = false;
    
    private float targetFOV = 45f;
    private float startFOV;

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
        GameManager.instance.GrangeClicked += ChangeCameraZoomGrange;
        GameManager.instance.AbreuvoirClicked += ChangeCamera;
        GameManager.instance.NicheClicked += ChangeCamera;
        GameManager.instance.OnClickOnShop += ChangeCamera;
    }

    private void OnDisable()
    {
        GameManager.instance.SheepClicked -= LockCamOnSheep;
        GameManager.instance.SheepHold -= ChangeCamera;
        GameManager.instance.GrangeClicked -= ChangeCameraZoomGrange;
        GameManager.instance.AbreuvoirClicked -= ChangeCamera;
        GameManager.instance.NicheClicked -= ChangeCamera;
        GameManager.instance.OnClickOnShop -= ChangeCamera;
    }

    public void StopAll()
    {
        StopAllCoroutines();
        isInTransition = false;
        ResetPosition();
    }

    // ----------------------------------------------------------------------
    private static Quaternion EnsureShortest(Quaternion from, Quaternion to)
    {
        if (Quaternion.Dot(from, to) < 0f)
            to = new Quaternion(-to.x, -to.y, -to.z, -to.w);
        return to;
    }

    // ----------------------------------------------------------------------
    private void SavePreTransitionState()
    {
        preTransitionCamPos = camera.transform.position;
        preTransitionCamRot = camera.transform.rotation;
        if (control != null && control.root != null)
        {
            preTransitionRootPos = control.root.position;
            preTransitionRootRot = control.root.rotation;
        }
        hasPreTransition = true;
    }

    // ----------------------------------------------------------------------
    private IEnumerator SmoothFinalCameraRotation(Quaternion targetRotation)
    {
        Quaternion startRot = camera.transform.rotation;
        targetRotation = EnsureShortest(startRot, targetRotation);

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, finalRotationTime);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            camera.transform.rotation = Quaternion.Slerp(startRot, targetRotation, t);
            yield return null;
        }

        camera.transform.rotation = targetRotation;
    }

    // ----------------------------------------------------------------------
private IEnumerator SmoothTransition(
    Vector3 targetCamPos,
    Vector3 targetEuler,
    Vector3 targetRootPos,
    Quaternion targetRootRot,
    Transform target,
    bool isGrangeZoom = false,
    bool reEnableControl = false
)
{
    isInTransition = true;

    if (!isGrangeZoom)
        SavePreTransitionState();

    control.enabled = false;
    follow.enabled = false;
    control.SetIgnoreInput(true);

    Quaternion startCamRot = camera.transform.rotation;
    Quaternion targetCamRot = Quaternion.Euler(targetEuler);
    targetCamRot = EnsureShortest(startCamRot, targetCamRot);

    Quaternion startRootRot = control.root.rotation;
    targetRootRot = EnsureShortest(startRootRot, targetRootRot);

    Vector3 startCamPos = camera.transform.position;
    Vector3 startRootPos = control.root.position;

    // -----------------------------
    // AJOUT : transition progressive du FOV vers 45
    // -----------------------------
    float startFOV = camera.fieldOfView;
    float targetFOV = 45f;
    // -----------------------------

    float elapsed = 0f;
    float duration = Mathf.Max(0.0001f, timerToTransition);

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // position caméra
        camera.transform.position = Vector3.Lerp(startCamPos, targetCamPos, t);

        Quaternion lookAtRot = Quaternion.LookRotation(target.position - camera.transform.position, Vector3.up);

        float blend = Mathf.Pow(t, 3);
        Quaternion targetRotCombined = Quaternion.Slerp(lookAtRot, targetCamRot, blend);

        camera.transform.rotation = Quaternion.Slerp(startCamRot, targetRotCombined, t);

        // -----------------------------
        // AJOUT : interpolation du FOV
        // -----------------------------
        camera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
        // -----------------------------

        // root
        control.root.position = Vector3.Lerp(startRootPos, targetRootPos, t);
        control.root.rotation = Quaternion.Slerp(startRootRot, targetRootRot, t);

        yield return null;
    }

    // garantir position, rotation et FOV finales
    camera.transform.position = targetCamPos;
    camera.transform.rotation = targetCamRot;
    camera.fieldOfView = targetFOV;

    control.root.position = targetRootPos;
    control.root.rotation = targetRootRot;

    yield return new WaitForEndOfFrame();

    if (reEnableControl)
    {
        control.enabled = true;
        control.SetIgnoreInput(false);
    }

    yield return new WaitForSeconds(transitionCooldown);
    isInTransition = false;
}

    // ----------------------------------------------------------------------
    public void ChangeCamera(Vector3 newPosition, Vector3 rotationEuler, Transform target)
    {
        if (!isInTransition)
        {
            StartCoroutine(
                SmoothTransition(
                    newPosition,
                    rotationEuler,
                    target.position,
                    target.rotation,
                    target,
                    false
                )
            );
        }
    }

    public void ChangeCameraZoomGrange(Vector3 newPosition, Vector3 rotationEuler, Transform target, bool isGrangeZoom = false)
    {
        if (!isInTransition)
        {
            StartCoroutine(
                SmoothTransition(
                    newPosition,
                    rotationEuler,
                    target.position,
                    target.rotation,
                    target,
                    isGrangeZoom
                    )
            );
        }
    }

    // ----------------------------------------------------------------------
    public void LockCamOnSheep(Sheep sheep)
    {
        if (sheep == null) return;

        SavePreTransitionState();
        control.enabled = false;
        control.SetIgnoreInput(true);

        follow.enabled = true;
        follow.target = sheep.transform;
        follow.offset = camera.transform.position - sheep.transform.position;

        sheep.ChangeOutlineState(true);
    }

    // ----------------------------------------------------------------------
    public void ResetCameraLock(Sheep sheep)
    {
        if (GameManager.instance.getCurLockSheep() != null)
            GameManager.instance.getCurLockSheep().isOpen = false;

        follow.enabled = false;
        follow.target = null;

        control.enabled = true;
        control.SetIgnoreInput(false);

        if (sheep != null)
            sheep.ChangeOutlineState(false);
    }

    // ----------------------------------------------------------------------
    public void ResetPosition()
    {
        if (!isInTransition && hasPreTransition)
            StartCoroutine(SmoothReset());
    }

    // ----------------------------------------------------------------------
private IEnumerator SmoothReset()
{
    isInTransition = true;

    if (!hasPreTransition)
    {
        isInTransition = false;
        yield break;
    }

    Vector3 startCamPos = camera.transform.position;
    Quaternion startCamRot = camera.transform.rotation;
    Vector3 startRootPos = control.root.position;
    Quaternion startRootRot = control.root.rotation;

    Vector3 targetCamPos = preTransitionCamPos;
    Quaternion targetCamRot = preTransitionCamRot;
    Vector3 targetRootPos = preTransitionRootPos;
    Quaternion targetRootRot = preTransitionRootRot;

    float currentFOV = camera.fieldOfView;

    control.enabled = false;
    follow.enabled = false;
    control.SetIgnoreInput(true);
    
    GameManager.instance.ResetCamera();

    targetCamRot = EnsureShortest(startCamRot, targetCamRot);
    targetRootRot = EnsureShortest(startRootRot, targetRootRot);

    float elapsed = 0f;
    float duration = Mathf.Max(0.0001f, timerToTransition);

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        camera.transform.position = Vector3.Lerp(startCamPos, targetCamPos, t);
        camera.transform.rotation = Quaternion.Slerp(startCamRot, targetCamRot, t);

        camera.fieldOfView = currentFOV;

        control.root.position = Vector3.Lerp(startRootPos, targetRootPos, t);
        control.root.rotation = Quaternion.Slerp(startRootRot, targetRootRot, t);

        yield return null;
    }

    camera.transform.position = targetCamPos;
    camera.transform.rotation = targetCamRot;

    camera.fieldOfView = currentFOV;

    control.root.position = targetRootPos;
    control.root.rotation = targetRootRot;

    yield return new WaitForEndOfFrame();

    control.enabled = true;
    follow.enabled = false;
    control.SetIgnoreInput(false);

    yield return new WaitForSeconds(transitionCooldown);
    isInTransition = false;
}

}
