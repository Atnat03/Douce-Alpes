using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SheepBoid))]
public class SheepThirst : MonoBehaviour
{
    [Header("Soif")]
    public float maxThirst = 100f;
    public float thirstIncreaseRate = 5f;
    public float drinkDuration = 3f;

    private float thirst = 0f;
    private SheepBoid boidScript;
    private bool isDrinking = false;

    void Start()
    {
        boidScript = GetComponent<SheepBoid>();
        thirst = Random.Range(0, maxThirst * 0.5f); // soif aléatoire au départ
    }

    void Update()
    {
        if (isDrinking) return;

        thirst += thirstIncreaseRate * Time.deltaTime;
        thirst = Mathf.Clamp(thirst, 0, maxThirst);

        if (thirst >= maxThirst && Abreuvoir.instance.TryReservePlace(out Transform drinkPlace))
        {
            StartCoroutine(GoDrink(drinkPlace));
        }
    }

    private IEnumerator GoDrink(Transform drinkPlace)
    {
        isDrinking = true;

        // Désactive le boid pour reprendre le contrôle manuel
        boidScript.enabled = false;

        // Déplacement vers l’abreuvoir
        Vector3 startPos = transform.position;
        Vector3 targetPos = drinkPlace.position;
        targetPos.y = startPos.y; // conserve la hauteur

        float speed = boidScript.manager.maxSpeed;
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            transform.LookAt(targetPos);
            yield return null;
        }

        // Boire
        yield return new WaitForSeconds(drinkDuration);

        // Libérer la place à l’abreuvoir
        Abreuvoir.instance.FreePlace(drinkPlace);

        // Réactiver le boid et reprendre le mouvement normal
        boidScript.enabled = true;

        thirst = 0f;
        isDrinking = false;
    }
}