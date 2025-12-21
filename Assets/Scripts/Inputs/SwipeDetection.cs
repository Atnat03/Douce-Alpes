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
    
    private HashSet<Sheep> currentlyCaressedSheep = new HashSet<Sheep>();
    
    [Header("Trail")]
    public TrailRenderer swipeTrail;
    private bool trailActive = false;
    
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
        
        if (swipeTrail != null)
        {
            swipeTrail.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, 10f));
            swipeTrail.Clear();
            trailActive = true;
        }
        
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
                OnFingerPositionUpdated?.Invoke(pos);
                
                if (trailActive && swipeTrail != null)
                {
                    swipeTrail.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10f));
                }
            }

            yield return null;
        }
    }
    
    public Vector2 GetCurrentFingerPosition() {
        return touchManager.PrimaryPosition();
    }
    
    public event System.Action<Vector2> OnFingerPositionUpdated;
    public event System.Action OnSwipeEnded;
    
    private void SwipeEnd(Vector2 position, float time)
    {
        if (swipeCoroutine != null) StopCoroutine(swipeCoroutine);
        endPosition = position;
        endTime = time;

        DetectSwipe();
        OnSwipeFinished?.Invoke(new List<Vector2>(swipePoints));
        OnFingerPositionUpdated?.Invoke(Vector2.zero);
        
        trailActive = false;
    }
    
    private void DetectCleanObject(Vector2 screenPosition) 
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        float maxDistance = 2.6f;
        float offset = (GameData.instance.dicoAmélioration[TypeAmelioration.Nettoyage].Item2 > 1) ? 0.25f : 0.1f;

        if (SwapSceneManager.instance.currentSceneId == 3) {
            bool cleaned = false;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                if (hit.collider.CompareTag("CleanSheep"))
                {
                    Vector3 cleanPoint = hit.point;
                    Vector3 center = GetCleaningCenter();

                    if (Vector3.Distance(cleanPoint, center) <= CleanManager.instance.maxDistanceFromCenter)
                    {
                        Transform head = CleanManager.instance.currentSheep
                            .GetComponent<SheepCleanningModel>().head;
                        
                        Vector3 headCenter = head.TransformPoint(CleanManager.instance.headDetectionOffset);

                        bool isHead = Vector3.Distance(cleanPoint, headCenter)
                                      <= CleanManager.instance.GetHeadDetectionRadius();

                        CleanManager.instance.ApplyClean(cleanPoint, isHead);
                        return;
                    }
                }
            }

            else if (Physics.SphereCast(ray, offset, out hit, maxDistance))
            {
                if (!hit.collider.CompareTag("CleanSheep"))
                    return;

                Vector3 cleanPoint = hit.collider.ClosestPoint(hit.point);
                Vector3 center = GetCleaningCenter();

                if (Vector3.Distance(cleanPoint, center) > CleanManager.instance.maxDistanceFromCenter)
                    return;

                Transform head = CleanManager.instance.currentSheep
                    .GetComponent<SheepCleanningModel>().head;

                bool isHead = Vector3.Distance(cleanPoint, head.position)
                              <= CleanManager.instance.GetHeadDetectionRadius();

                CleanManager.instance.ApplyClean(cleanPoint, isHead);
                return;
            }

            if (cleaned) return;
        }

        if (SwapSceneManager.instance.currentSceneId != 3 && Physics.Raycast(ray, out hit)) {
            Sheep sheep = hit.collider.GetComponent<Sheep>();
            if (sheep != null) {
                if (!currentlyCaressedSheep.Contains(sheep)) {
                    sheep.AddCaresse();
                    currentlyCaressedSheep.Add(sheep);
                }
            }
            List<Sheep> toRemove = new List<Sheep>();
            foreach (Sheep s in currentlyCaressedSheep) {
                if (s != hit.collider?.GetComponent<Sheep>()) {
                    toRemove.Add(s);
                }
            }
            foreach (Sheep s in toRemove) {
                currentlyCaressedSheep.Remove(s);
            }
        }

        if (SwapSceneManager.instance.currentSceneId == 0 && GameManager.instance.currentCameraState == CamState.Dog &&
            Physics.Raycast(ray, out hit))
        {
            Chien chien = hit.collider.GetComponent<Chien>();
            if (chien != null) {
                chien.Carresse();
            }
        }
    }

    private Vector3 GetCleaningCenter() {
        switch (CleanManager.instance.currentCleaningSide) {
            case CleaningSide.Left: return CleanManager.instance.leftCenter.position;
            case CleaningSide.Front: return CleanManager.instance.frontCenter.position;
            case CleaningSide.Right: return CleanManager.instance.rightCenter.position;
            default: return Vector3.zero;
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
