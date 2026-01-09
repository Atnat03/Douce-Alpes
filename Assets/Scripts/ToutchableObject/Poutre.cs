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
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float impulseForce = 500f;

    private Vector3 startPos;
    private Quaternion startRotation;
    private bool hasSwipe = false;

    private void Awake()
    {
        startPos = transform.position;
        startRotation = transform.rotation;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        // Rigidbody prêt mais inactif
        rb.isKinematic = true;
        rb.useGravity = true;

        GetComponent<Animator>().enabled = false;
    }

    private void Update()
    {
        if (CanSwipe() && !hasSwipe)
        {
            GetComponent<Animator>().enabled = true;
            grange.hand.SetActive(true);
        }
    }

    public async Task GetOffPoutre(SwipeType swipe)
    {
        if (swipe != SwipeType.Up) return;
        if (!CanSwipe()) return;

        hasSwipe = true;

        Animator animator = GetComponent<Animator>();
        animator.enabled = false;

        // Active la physique
        rb.isKinematic = false;

        // Laisse Unity appliquer l'état avant l'impulsion
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

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = startPos;
        transform.rotation = startRotation;

        hasSwipe = false;
        GetComponent<Animator>().enabled = false;
    }

    private bool CanSwipe()
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
