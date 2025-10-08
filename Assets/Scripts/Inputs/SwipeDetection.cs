using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SwipeType
{
    Up,
    Down,
    Left,
    Right,
    Circle,
    Square
}

public class SwipeDetection : MonoBehaviour
{
    public static SwipeDetection instance;
    public Action<SwipeType> OnSwipeDetected;
    public event Action<List<Vector2>> OnSwipeUpdated;
    public event Action<List<Vector2>> OnSwipeFinished;

    [HideInInspector] public float miniDistance = 50f;

    private TouchManager touchManager;
    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;
    private Coroutine swipeCoroutine;
    private bool isSwipe = false;
    private List<Vector2> swipePoints = new List<Vector2>();
    
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        touchManager = TouchManager.instance;
        
        raycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = EventSystem.current;
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
        startPosition = position;
        startTime = time;
        endPosition = position;
        isSwipe = false;
        swipePoints.Clear();
        swipePoints.Add(startPosition);
        swipeCoroutine = StartCoroutine(SwipeProgress());
    }

    private IEnumerator SwipeProgress()
    {
        while (true)
        {
            Vector2 pos = touchManager.PrimaryPosition();
            if (!isSwipe && Vector2.Distance(startPosition, pos) >= miniDistance)
                isSwipe = true;

            if (isSwipe)
            {
                swipePoints.Add(pos);
                OnSwipeUpdated?.Invoke(new List<Vector2>(swipePoints));
                
                DetectCleanObject(pos);
                DetectUIButton(pos);
            }

            yield return null;
        }
    }
    
    private void SwipeEnd(Vector2 position, float time)
    {
        if (swipeCoroutine != null) StopCoroutine(swipeCoroutine);
        endPosition = position;
        endTime = time;

        DetectSwipe();
        OnSwipeFinished?.Invoke(new List<Vector2>(swipePoints));
    }
    
    private void DetectCleanObject(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("CleanSheep"))
            {
                if (CleanManager.instance != null)
                {
                    CleanManager.instance.PerformClean(hit.point);
                }
            }
        }
    }
    
    private void DetectUIButton(Vector2 screenPosition)
    {
        if (raycaster == null || eventSystem == null)
            return;

        pointerEventData = new PointerEventData(eventSystem)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);
    }


    private void DetectSwipe()
    {
        if (Vector2.Distance(startPosition, endPosition) < miniDistance ||
            (endTime - startTime) > 1f) return;

        Vector2 dir = (endPosition - startPosition).normalized;
        if (Vector2.Dot(Vector2.up, dir) > 0.8f) OnSwipeDetected?.Invoke(SwipeType.Up);
        else if (Vector2.Dot(Vector2.down, dir) > 0.8f) OnSwipeDetected?.Invoke(SwipeType.Down);
        else if (Vector2.Dot(Vector2.right, dir) > 0.8f) OnSwipeDetected?.Invoke(SwipeType.Right);
        else if (Vector2.Dot(Vector2.left, dir) > 0.8f) OnSwipeDetected?.Invoke(SwipeType.Left);
    }
}
