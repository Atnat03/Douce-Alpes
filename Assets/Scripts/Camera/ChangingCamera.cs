using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChangingCamera : MonoBehaviour
{
    [SerializeField] private Button quitButton;
    [SerializeField] private float timerToTransition = 1f;

    private CameraControl control;
    private CameraFollow follow;
    private float elapseTime;

    private Vector3 savedRootPos;
    private Quaternion savedPivotRot;
    private Vector3 savedTargetPos;

    private Vector3 savedCamPos;
    private Quaternion savedCamRot;

    private void OnEnable()
    {
        GameManager.instance.SheepClicked += LockCamOnSheep;
        GameManager.instance.SheepHold += ChangeCamera;
        GameManager.instance.GrangeClicked += ChangeCamera;
        GameManager.instance.AbreuvoirClicked += ChangeCamera;
        GameManager.instance.NicheClicked += ChangeCamera;

        control = GetComponent<CameraControl>();
        follow = GetComponent<CameraFollow>();

        quitButton.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        GameManager.instance.SheepClicked -= LockCamOnSheep;
        GameManager.instance.SheepHold -= ChangeCamera;
        GameManager.instance.GrangeClicked -= ChangeCamera;
        GameManager.instance.AbreuvoirClicked -= ChangeCamera;
        GameManager.instance.NicheClicked -= ChangeCamera;
    }

    private IEnumerator SmoothTransition(Vector3 targetPos, Vector3 targetEuler, Transform target, bool reEnableControl = false, bool hideQuitButton = false)
    {
        elapseTime = 0f;
        Quaternion finalRotation = Quaternion.Euler(targetEuler);

        savedRootPos = control.root.position;
        savedPivotRot = control.root.localRotation;

        savedCamPos = Camera.main.transform.position;
        savedCamRot = Camera.main.transform.rotation;

        control.enabled = false;
        follow.enabled = false;

        if (!hideQuitButton)
            quitButton.gameObject.SetActive(true);

        while (elapseTime < timerToTransition)
        {
            elapseTime += Time.deltaTime;
            float t = elapseTime / timerToTransition;
            
            Camera.main.transform.LookAt(target);
            Camera.main.transform.position = Vector3.Lerp(savedCamPos, targetPos, t);
            //Camera.main.transform.rotation = Quaternion.Slerp(savedCamRot, finalRotation, t);

            yield return null;
        }

        if (reEnableControl)
            control.enabled = true;

        if (hideQuitButton)
            quitButton.gameObject.SetActive(false);
    }

    public void ChangeCamera(Vector3 newPosition, Vector3 rotation, Transform target)
    {
        Debug.Log("ChangeCamera");
        //StopAllCoroutines();
        StartCoroutine(SmoothTransition(newPosition, rotation, target));
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
        StopAllCoroutines();
        StartCoroutine(SmoothReset());
        GameManager.instance.ResetCamera();
    }

    private IEnumerator SmoothReset()
    {
        float elapsed = 0f;

        Vector3 currentCamPos = Camera.main.transform.position;
        Quaternion currentCamRot = Camera.main.transform.rotation;

        Vector3 currentRootPos = control.root.position;

        control.enabled = false;
        follow.enabled = false;

        Vector3 endCamPos = savedRootPos;
        Quaternion endCamRot = savedPivotRot;

        while (elapsed < timerToTransition)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / timerToTransition;

            Camera.main.transform.position = Vector3.Lerp(currentCamPos, savedRootPos, t);
            Camera.main.transform.rotation = Quaternion.Slerp(currentCamRot, savedPivotRot, t);

            control.root.position = Vector3.Lerp(currentRootPos, savedRootPos, t);

            yield return null;
        }

        Camera.main.transform.position = endCamPos;
        Camera.main.transform.rotation = endCamRot;

        control.root.position = savedRootPos;
        control.root.rotation = savedPivotRot;

        control.enabled = true;
        quitButton.gameObject.SetActive(false);
    }
}
