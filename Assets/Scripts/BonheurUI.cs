using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BonheurUI : MonoBehaviour
{
    [FormerlySerializedAs("valueImahe")] [FormerlySerializedAs("backgroundImage")] [Header("UI Elements")]
    public Image valueImage;

    public Image heartImage;
    
    private bool spawnAnimationFinished = false;
    
    [Header("State Values")]
    [Range(0, 1)] public float currentValue = 0f;
    [Range(0, 1)] public float overflowValue = 0f;
    public bool isOverflow = false;

    [Header("Pos UI")]
    [SerializeField] RectTransform posVisible;
    [SerializeField] RectTransform posInvisible;
    [SerializeField] RectTransform canvaPlayer;
    [SerializeField] bool isDropped = false;
    
    [Header("Colors")]
    [SerializeField] private Color32 veryLowColor;
    [SerializeField] private Color32 lowColor;
    [SerializeField] private Color32 midColor;
    [SerializeField] private Color32 highColor;
    [SerializeField] private Color32 veryHighColor;
    [SerializeField] private Color32 overflowColor;
    [SerializeField] private Sprite[] heartSprites;

    private void Update()
    {
        UpdateCursorAndColor();
        
        if (SwapSceneManager.instance.currentSceneId == 0 ||
            SwapSceneManager.instance.currentSceneId == 4)
        {
            isDropped = false;
        }
        
        if (isDropped)
            return;
        
        bool shouldBeVisible =
            SwapSceneManager.instance.currentSceneId == 0 ||
            SwapSceneManager.instance.currentSceneId == 4;

        Vector2 pos = shouldBeVisible ? posVisible.position : posInvisible.position;
        canvaPlayer.position = pos;

    }

    private void UpdateCursorAndColor()
    {
        valueImage.color = GetColorForValue(currentValue, isOverflow);
        valueImage.fillAmount = currentValue;
        heartImage.sprite = GetSprite();
    }


    public Sprite GetSprite()
    {
        return currentValue <= 0.5f ? heartSprites[0] : heartSprites[1];
    }
    
    private Color32 GetColorForValue(float value, bool overflow)
    {
        if (overflow) return overflowColor;
        if (value <= 0.1f) return veryLowColor;
        if (value <= 0.25f) return lowColor;
        if (value <= 0.5f) return midColor;
        if (value <= 0.75f) return highColor;
        return veryHighColor;
    }

    public void DropCanva(Vector2 posSpawnSprite, int value, GameObject sprite, Vector2 targetPosition)
    {
        print("Drop Canva");
        isDropped = true;
        spawnAnimationFinished = false;
        
        if(SwapSceneManager.instance.currentSceneId != 0 && SwapSceneManager.instance.currentSceneId != 4)
            StartCoroutine(AnimatedCanvaTranslation(posInvisible, posVisible, posSpawnSprite, value, sprite, targetPosition));
    }

    public void RemonteCanva()
    {
        print("Remonte Canva");
        isDropped = false;

        if (SwapSceneManager.instance.currentSceneId != 0&& SwapSceneManager.instance.currentSceneId != 4)
            StartCoroutine(AnimatedCanvaTranslation(
                posVisible,
                posInvisible, 
                Vector2.zero,
                0, 
                null, 
                Vector2.zero, 
                1f, 
                false
            ));
    }

    

    IEnumerator AnimatedCanvaTranslation(RectTransform startPos, RectTransform target, Vector2 posSpawnSprite, int value, GameObject sprite, Vector2 targetAnimPosition,float duration = 1f, bool spawnAnim = true)
    {
        Vector3 initialPosition = startPos.position;
        Vector3 targetPosition = target.position;    
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            canvaPlayer.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null; 
        }

        canvaPlayer.position = targetPosition;
        
        if(spawnAnim)
            StartCoroutine(SpawnAnimatedSprite(posSpawnSprite, value, sprite, targetAnimPosition, true));
    }

    public void StartAnimatedSprite(Vector2 posSpawnSprite, int value, GameObject spritePrefab, Vector2 targetPosition)
    {
        Debug.Log(posSpawnSprite);
        StartCoroutine(SpawnAnimatedSprite(posSpawnSprite, value, spritePrefab, targetPosition));
    }
    
    IEnumerator SpawnAnimatedSprite(Vector2 posSpawnSprite, int value, GameObject spritePrefab, Vector2 targetPosition, bool undropUI = false)
    {
        List<GameObject> sprites = new List<GameObject>();
        int count = value / 10;
        float moveDuration = 0.3f;
        float spawnRadius = 100f;

        for (int i = 0; i < count; i++)
        {
            GameObject newSprite = Instantiate(spritePrefab, posSpawnSprite, Quaternion.identity, canvaPlayer.parent);
            sprites.Add(newSprite);
        }

        List<Vector2> targetPositions = new List<Vector2>();
        foreach (GameObject s in sprites)
        {
            Vector2 randomTarget = posSpawnSprite + Random.insideUnitCircle * spawnRadius;
            targetPositions.Add(randomTarget);
        }

        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / moveDuration);

            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].transform.position = Vector2.Lerp(posSpawnSprite, targetPositions[i], t);
            }

            yield return null;
        }

        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].transform.position = targetPositions[i];
        }

        foreach (GameObject s in sprites)
        {
            AnimatedSprite anim = s.GetComponent<AnimatedSprite>();
            anim.targetPosition = targetPosition;
            if (anim != null)
                anim.enabled = true;
        }
        
        spawnAnimationFinished = true;

        yield return new WaitForSeconds(1.5f);
        
        /*
         if(undropUI)
            StartCoroutine(AnimatedCanvaTranslation(posVisible, posInvisible, posSpawnSprite, value, spritePrefab, targetPosition, 1f, false));
        */
    }
    
    public bool IsSpawnAnimationFinished()
    {
        return spawnAnimationFinished;
    }
}