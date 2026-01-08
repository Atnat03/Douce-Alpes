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
        thirst = Random.Range(0, maxThirst * 0.5f);
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

        boidScript.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 targetPos = drinkPlace.position;
        targetPos.y = startPos.y; 

        float speed = boidScript.manager.maxSpeed;
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            transform.LookAt(targetPos);
            yield return null;
        }

        yield return new WaitForSeconds(drinkDuration);

        Abreuvoir.instance.FreePlace(drinkPlace);

        boidScript.enabled = true;

        thirst = 0f;
        isDrinking = false;
    }
}