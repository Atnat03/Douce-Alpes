using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField] private float miniDistance = .5f;
    [SerializeField] float maxTime = 1f;
    [SerializeField, Range(0,1)] float directionThreshold = .9f;

    [SerializeField] private Vector2 swipeAreaSize = new Vector2(600, 600); 
    private Rect swipeArea;
    
    TouchManager touchManager;

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    [SerializeField] private GameObject trail;

    private Coroutine coroutine;

    private Sheep currentHoveredSheep = null;

    private bool isSwipe = false;

    [SerializeField] private Poutre poutre;

    [Header("Skins")]
    [SerializeField] private SkinManager skinManager;
    private bool isSkinMenu = false;

    private void Awake()
    {
        touchManager = TouchManager.instance;
    }
    
    private void Start()
    {
        float screenW = Screen.width;
        float screenH = Screen.height;
        swipeArea = new Rect(
            (screenW - swipeAreaSize.x) / 2f,
            (screenH - swipeAreaSize.y) / 2f,
            swipeAreaSize.x,
            swipeAreaSize.y
        );
    }

    private void Update()
    {
        isSkinMenu = skinManager.gameObject.activeSelf;
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
            
            if (!swipeArea.Contains(screenPos))
            {
                Debug.Log("Swipe ignoré : hors zone centrale");
                break;
            }

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
        if (GameManager.instance.currentCameraState != CamState.Default) return;
        
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
            SwipeDirection(direction, startPosition);
        }
    }

    private void SwipeDirection(Vector2 direction, Vector2 startPos)
    {
        if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
        {
            Debug.Log("Swipe Up");

            if (poutre != null)
                poutre.GetOffPoutre();
        }
        else if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            Debug.Log("Swipe Down");

            if (GameManager.instance.currentCameraState == CamState.Drink)
            {
                if (startPos.x > Screen.width * 0.66f)
                {
                    Debug.Log("Swipe Down sur la zone droite");
                    
                    Abreuvoir.instance.AddWater();
                }
                else
                {
                    Debug.Log("Swipe ignoré : pas sur la zone droite");
                }

            }
        }
        else if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            Debug.Log("Swipe Right");
            if (isSkinMenu && skinManager.IsInsideSwipeArea(startPos))
            {
                skinManager.RightArrow();
            }
        }
        else if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            Debug.Log("Swipe Left");
            if (isSkinMenu && skinManager.IsInsideSwipeArea(startPos))
            {
                skinManager.LeftArrow();
            }
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
    
    private void OnGUI()
    {
        if (Application.isPlaying)
        {
            Color old = GUI.color;
            GUI.color = new Color(1, 0, 0, 0.2f);
            GUI.Box(swipeArea, GUIContent.none);
            GUI.color = old;
        }
    }
}
