using System.Collections;
using UnityEngine;

public class Sheep : TouchableObject
{
    [SerializeField] public int sheepId;
    [SerializeField] public string sheepName;

    [SerializeField] public int currentSkinHat;
    [SerializeField] public int currentSkinClothe;
    [SerializeField] public bool hasLaine = true;

    [SerializeField] private bool isBeingCaressed = false;
    public bool IsBeingCaressed => isBeingCaressed;

    [SerializeField] public bool isOpen = false;

    private Vector3 lockedPosition;
    private Quaternion lockedRotation;

    [Header("Double Click Settings")]
    [SerializeField] private float doubleClickThreshold = 0.3f; 
    private float lastClickTime = -1f;

    [Header("References")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private ParticleSystem heartParticle;
    [SerializeField] private SkinListManager skinListManager;
    [SerializeField] public GameObject laine;

    private SheepBoid sheepBoid;

    private void Start()
    {
        sheepBoid = GetComponent<SheepBoid>();
        laine.GetComponent<Outline>().enabled = false;
    }

    public void Initialize(int id, string name)
    {
        sheepId  = id;
        sheepName = name;
    }

    private void Update()
    {
        laine.SetActive(hasLaine);

        if (isOpen)
        {
            // Maintenir mouton à la position et rotation verrouillées
            transform.position = lockedPosition;
            transform.rotation = lockedRotation;
        }
    }

    public void CutWhool()
    {
        hasLaine = false;
    }

    public void StopAgentAndDesactivateScript(bool state)
    {
        GetComponent<SheepBoid>().enabled = !state;
    }

    public Vector3 GetCameraPosition()
    {
        return cameraPosition.position;
    }

    public void ChangeOutlineState(bool state)
    {
        laine.GetComponent<Outline>().enabled = state;
    }

    public void AddCaresse()
    {
        if (GameManager.instance.shopOpen) return;
        if (isOpen) return;

        isBeingCaressed = true;
        heartParticle.Play();
        GameManager.instance.Caresse(this);

        CancelInvoke(nameof(StopCaresse));
        Invoke(nameof(StopCaresse), 0.2f);
    }

    private void StopCaresse()
    {
        isBeingCaressed = false;
    }

    public override void TouchEvent()
    {
        if (GameManager.instance.shopOpen) return;

        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            Debug.Log("Double Clique");

            if (!isBeingCaressed && !sheepBoid.isAfraid)
            {
                WidowOpen(); // ne touche plus au lock
            }

            lastClickTime = -1f;
            return;
        }

        lastClickTime = Time.time;

        if (GameManager.instance.getCurLockSheep() != this)
            StartCoroutine(LockCamWithDelay());
    }

    private IEnumerator LockCamWithDelay()
    {
        float startTime = Time.time;

        while (Time.time - startTime < doubleClickThreshold)
        {
            if (lastClickTime < 0f) yield break; // annule si double clic
            yield return null;
        }

        if (GameManager.instance.getCurLockSheep() != this)
            GameManager.instance.LockCamOnSheep(this);

        lastClickTime = -1f;
    }

    public void WidowOpen()
    {
        isOpen = true;

        // Verrouille la position et rotation pour que le mouton reste stable
        lockedPosition = transform.position;
        lockedRotation = Quaternion.Euler(0, 180, 0);

        //StopAgentAndDesactivateScript(true);

        transform.position = lockedPosition;
        transform.rotation = lockedRotation;

        // Lock la caméra si elle n'est pas déjà sur ce mouton
        if (GameManager.instance.getCurLockSheep() != this)
            GameManager.instance.LockCamOnSheep(this);

        GameManager.instance.ChangeCameraState(CamState.Sheep);
        GameManager.instance.ChangeCameraPos(
            cameraPosition.position,
            cameraPosition.rotation.eulerAngles,
            transform
        );

        Camera.main.GetComponent<CameraControl>().ResetFOV();

        GameManager.instance.GetSheepWindow().SetActive(true);
        SheepWindow.instance.Initialize(sheepName, currentSkinHat, currentSkinClothe, sheepId);
    }

    public void SetCurrentSkinHat(int skinId)
    {
        currentSkinHat = skinId;
        skinListManager.UpdateSkinListHat(currentSkinHat);
    }

    public void SetCurrentSkinClothe(int skinId)
    {
        currentSkinClothe = skinId;
        skinListManager.UpdateSkinListClothe(currentSkinClothe);
    }
}
