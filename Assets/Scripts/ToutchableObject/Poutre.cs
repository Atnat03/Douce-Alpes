using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Poutre : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Cadenas[] cadenas;
    [SerializeField] private Grange grange;
    [SerializeField] private ParticleSystem touchGroundEffect;

    [Header("Physics")]
    [SerializeField] private float impulseForce = 500f;

    private Rigidbody rb;
    private Vector3 startPos;
    private Quaternion startRotation;
    private bool hasSwipe = false;

    private void Awake()
    {
        startPos = transform.position;
        startRotation = transform.rotation;

        GetComponent<Animator>().enabled = false;
    }

    private void Update()
    {
        if (CanSwipe() && !hasSwipe)
        {
            GetComponent<Animator>().enabled = true;
            grange.handZommed.SetActive(true);
        }
    }

    public async Task GetOffPoutre(SwipeType swipe)
    {
        if (swipe != SwipeType.Up) return;
        if (!CanSwipe()) return;

        hasSwipe = true;
        
        grange.handZommed.SetActive(false);

        GetComponent<Animator>().enabled = false;
        
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        AudioManager.instance.PlaySound(38);
        
        await Task.Yield();

        rb.AddForce(Vector3.up * impulseForce, ForceMode.Impulse);

        StartCoroutine(WaitALittle());
    }

    IEnumerator WaitALittle()
    {
        yield return new WaitForSeconds(2f);

        grange.OpenDoors();
        GameManager.instance.SheepGetOutGrange();
    }

    public void ResetPoutre()
    {
        foreach (Cadenas cadena in cadenas)
        {
            cadena.gameObject.SetActive(true);
            cadena.hp = cadena.maxHp[GameData.instance.dicoAmélioration[TypeAmelioration.Sortie].Item2];
            cadena.ResetCadenas();
        }

        if (rb != null)
        {
            Destroy(rb);
            rb = null;
        }

        transform.position = startPos;
        transform.rotation = startRotation;

        hasSwipe = false;
        GetComponent<Animator>().enabled = false;
    }

    public bool CanSwipe()
    {
        foreach (Cadenas cadena in cadenas)
        {
            if (cadena.gameObject.activeSelf)
                return false;
        }
        return true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Ground")) return;

        ContactPoint contact = other.contacts[0];
        touchGroundEffect.transform.position = contact.point;
        touchGroundEffect.Play();
    }

    private void OnEnable()
    {
        if (SwipeDetection.instance != null)
            SwipeDetection.instance.OnSwipeDetected += OnSwipe;
    }

    private void OnDisable()
    {
        if (SwipeDetection.instance != null)
            SwipeDetection.instance.OnSwipeDetected -= OnSwipe;
    }

    private void OnSwipe(SwipeType swipe)
    {
        _ = GetOffPoutre(swipe);
    }
}
