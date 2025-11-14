using System.Collections;
using UnityEngine;

public class ChangingCamera : MonoBehaviour
{
    [SerializeField] private Camera camera;
    public float timerToTransition = 1f;
    public float transitionCooldown = 0.5f;

    private CameraControl control;
    private CameraFollow follow;

    private Vector3 preTransitionCamPos;
    private Quaternion preTransitionCamRot;
    private Vector3 preTransitionRootPos;
    private Quaternion preTransitionRootRot;
    private bool hasPreTransition = false;

    private bool isInTransition = false;

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

    // ----------------------------------------------------------------------
    // assure toujours la continuité quaternion (même hémisphère)
    // ----------------------------------------------------------------------
    private static Quaternion EnsureShortest(Quaternion from, Quaternion to)
    {
        if (Quaternion.Dot(from, to) < 0f)
            to = new Quaternion(-to.x, -to.y, -to.z, -to.w);
        return to;
    }

    // ----------------------------------------------------------------------
    // Save de l'état courant (appelé aussi depuis LockCamOnSheep)
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
    // Transition linéaire sur timer (position + rotation + root)
    // ----------------------------------------------------------------------
    private IEnumerator SmoothTransition(
        Vector3 targetCamPos,
        Vector3 targetEuler,
        Vector3 targetRootPos,
        Quaternion targetRootRot,
        bool reEnableControl = false)
    {
        isInTransition = true;

        // Sauvegarde immédiate (pour pouvoir revenir)
        SavePreTransitionState();

        control.enabled = false;
        follow.enabled = false;
        control.SetIgnoreInput(true);

        // Préparer rotations cibles et garantir chemin le plus court
        Quaternion startCamRot = camera.transform.rotation;
        Quaternion targetCamRot = Quaternion.Euler(targetEuler);
        targetCamRot = EnsureShortest(startCamRot, targetCamRot);

        Quaternion startRootRot = control.root.rotation;
        targetRootRot = EnsureShortest(startRootRot, targetRootRot);

        Vector3 startCamPos = camera.transform.position;
        Vector3 startRootPos = control.root.position;

        float elapsed = 0f;
        float duration = Mathf.Max(0.0001f, timerToTransition);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // t est linéaire; si tu veux easing utiliser une courbe ici (ex : Mathf.SmoothStep)
            camera.transform.position = Vector3.Lerp(startCamPos, targetCamPos, t);
            camera.transform.rotation = Quaternion.Slerp(startCamRot, targetCamRot, t);

            control.root.position = Vector3.Lerp(startRootPos, targetRootPos, t);
            control.root.rotation = Quaternion.Slerp(startRootRot, targetRootRot, t);

            yield return null;
        }

        // assurer valeur finale exacte
        camera.transform.position = targetCamPos;
        camera.transform.rotation = targetCamRot;
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

    public void ChangeCamera(Vector3 newPosition, Vector3 rotationEuler, Transform target)
    {
        if (!isInTransition)
        {
            StartCoroutine(
                SmoothTransition(
                    newPosition,
                    rotationEuler,
                    target.position,
                    target.rotation
                )
            );
        }
    }

    // ----------------------------------------------------------------------
    // Lock sur le mouton : IMPORTANT -> on sauvegarde l'état ici aussi
    // ----------------------------------------------------------------------
    public void LockCamOnSheep(Sheep sheep)
    {
        if (sheep == null) return;

        // 🔥 IMPORTANT : on enregistre l'état AVANT que le mouton ne bouge
        SavePreTransitionState();

        control.enabled = false;
        control.SetIgnoreInput(true);

        follow.enabled = true;
        follow.target = sheep.transform;

        follow.offset = camera.transform.position - sheep.transform.position;

        sheep.ChangeOutlineState(true);
    }


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
    // RESET : utilise la sauvegarde (si existante) et interpolation linéaire
    // ----------------------------------------------------------------------
    public void ResetPosition()
    {
        if (!isInTransition && hasPreTransition)
            StartCoroutine(SmoothReset());
    }

    private IEnumerator SmoothReset()
    {
        isInTransition = true;

        // si pas de pré-état, on sort proprement
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

        control.enabled = false;
        follow.enabled = false;
        control.SetIgnoreInput(true);

        GameManager.instance.ResetCamera();

        // forcer même hémisphère pour Slerp
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

            control.root.position = Vector3.Lerp(startRootPos, targetRootPos, t);
            control.root.rotation = Quaternion.Slerp(startRootRot, targetRootRot, t);

            yield return null;
        }

        camera.transform.position = targetCamPos;
        camera.transform.rotation = targetCamRot;
        control.root.position = targetRootPos;
        control.root.rotation = targetRootRot;

        yield return new WaitForEndOfFrame();

        control.enabled = true;
        follow.enabled = true;
        control.SetIgnoreInput(false);

        yield return new WaitForSeconds(transitionCooldown);
        isInTransition = false;
    }
}
