using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChangingCamera : MonoBehaviour
{
    [SerializeField] private float timerToTransition = 1f;
    [SerializeField] private Camera camera;

    private CameraControl control;
    private CameraFollow follow;

    private float elapseTime;
    private Vector3 savedRootPos;
    private Quaternion savedPivotRot;
    private Vector3 savedTargetPos;
    private Vector3 savedCamPos;
    private Quaternion savedCamRot;
    
    private void Awake()
    {
        if (camera == null) camera = Camera.main;
        if (control == null) control = GetComponent<CameraControl>();
        if (follow == null) follow = GetComponent<CameraFollow>();

        savedCamPos = camera.transform.position;
        savedCamRot = camera.transform.rotation;
        savedRootPos = control.root.position;
        savedPivotRot = control.root.rotation;
    }


    private void OnEnable()
    {
        GameManager.instance.SheepClicked += LockCamOnSheep;
        GameManager.instance.SheepHold += ChangeCamera;
        GameManager.instance.GrangeClicked += ChangeCamera;
        GameManager.instance.AbreuvoirClicked += ChangeCamera;
        GameManager.instance.NicheClicked += ChangeCamera;
        GameManager.instance.OnClickOnShop += ChangeCamera;

        control = GetComponent<CameraControl>();
        follow = GetComponent<CameraFollow>();

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

    private IEnumerator SmoothTransition(Vector3 targetPos, Vector3 targetEuler, Transform target, bool reEnableControl = false, bool hideQuitButton = false)
    {
        elapseTime = 0f;
        camera.fieldOfView = 45f;

        savedRootPos = control.root.position;
        savedPivotRot = control.root.localRotation;
        savedCamPos = camera.transform.position;
        savedCamRot = camera.transform.rotation;

        control.enabled = false;
        follow.enabled = false;

        while (elapseTime < timerToTransition)
        {
            elapseTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapseTime / timerToTransition);

            Vector3 newPos = Vector3.Lerp(savedCamPos, targetPos, t);
            camera.transform.position = newPos;

            Vector3 direction = (target.position - camera.transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(direction);
            camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, lookRot, Time.deltaTime * 10f);

            yield return null;
        }

        camera.transform.position = targetPos;
        camera.transform.LookAt(target);

        if (reEnableControl)
            control.enabled = true;
    }

    public void ChangeCamera(Vector3 newPosition, Vector3 rotation, Transform target)
    {
        Debug.Log("ChangeCamera");
        StartCoroutine(SmoothTransition(newPosition, rotation, target));
    }
    
    public void ChangeCamera(Vector3 newPosition, Vector3 rotation, Transform target, bool cameraVisible)
    {
        Debug.Log("ChangeCamera");
        StartCoroutine(SmoothTransition(newPosition, rotation, target, false, cameraVisible));
    }

    public void LockCamOnSheep(Sheep sheep)
    {
        control.enabled = false;
        follow.enabled = true;
        follow.target = sheep.transform;
        follow.offset = Camera.main.transform.position;
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

            camera.transform.position = Vector3.Lerp(startCamPos, savedCamPos, t);
            camera.transform.rotation = Quaternion.Slerp(startCamRot, savedCamRot, t);

            control.root.position = Vector3.Lerp(startRootPos, savedRootPos, t);
            control.root.rotation = Quaternion.Slerp(startRootRot, savedPivotRot, t);

            yield return null;
        }

        camera.transform.position = savedCamPos;
        camera.transform.rotation = savedCamRot;
        control.root.position = savedRootPos;
        control.root.rotation = savedPivotRot;

        control.enabled = true;
    }



}
