using System;
using System.Collections;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField] private float miniDistance = .5f;
    [SerializeField] float maxTime = 1f;
    [SerializeField, Range(0,1)] float directionThreshold = .9f;

    TouchManager touchManager;

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    [SerializeField] private GameObject trail;

    private Coroutine coroutine;

    private Sheep currentHoveredSheep = null;

    private bool isSwipe = false;

    private void Awake()
    {
        touchManager = TouchManager.instance;
    }

    private void OnEnable()
    {
        touchManager.OnStartEvent += SwipeStart;
        touchManager.OnEndEvent += SwipeEnd;
    }

    private void OnDisable()
    {
        touchManager.OnStartEvent -= SwipeStart;
        touchManager.OnEndEvent -= SwipeEnd;
    }

    private void SwipeStart(Vector2 position, float time)
    {
        if (TouchManager.instance.isHolding) return;
        
        startPosition = position;
        startTime = time;
        endPosition = position;

        isSwipe = false;

        trail.SetActive(false); 
        currentHoveredSheep = null;

        coroutine = StartCoroutine(CheckSwipeProgress());
    }

    private IEnumerator CheckSwipeProgress()
    {
        while (true)
        {
            Vector2 screenPos = TouchManager.instance.PrimaryPosition();

            if (!isSwipe && Vector2.Distance(startPosition, screenPos) >= miniDistance)
            {
                isSwipe = true;
                trail.SetActive(true);
            }

            if (isSwipe)
            {
                trail.transform.position = ScreenToWorld(screenPos);
                DetectHover(screenPos);
            }

            yield return null;
        }
    }

    private void DetectHover(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Sheep sheep = hit.collider.gameObject.GetComponent<Sheep>();

            if (sheep != null)
            {
                if (currentHoveredSheep != sheep)
                {
                    sheep.AddCaresse();
                    currentHoveredSheep = sheep;
                    Debug.Log("Caresse ajoutée à " + sheep.name);
                }
            }
            else
            {
                currentHoveredSheep = null;
            }
        }
        else
        {
            currentHoveredSheep = null;
        }
    }

    private void SwipeEnd(Vector2 position, float time)
    {
        if (coroutine != null) StopCoroutine(coroutine);

        trail.SetActive(false);

        endPosition = position;
        endTime = time;

        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Vector2.Distance(startPosition, endPosition) >= miniDistance &&
            (endTime - startTime) <= maxTime)
        {
            Debug.Log("SwipeDetected");
            Vector2 direction = (endPosition - startPosition).normalized;
            SwipeDirection(direction);
        }
    }

    private void SwipeDirection(Vector2 direction)
    {
        if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
        {
            Debug.Log("Swipe Up");
        }
        else if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            Debug.Log("Swipe Down");
        }
        else if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            Debug.Log("Swipe Right");
        }
        else if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            Debug.Log("Swipe Left");
        }
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            return hit.point + Vector3.up * 0.1f;
        }

        return ray.GetPoint(9f);
    }
}
