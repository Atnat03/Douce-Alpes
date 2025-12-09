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

        if (isDropped)
            return;
        
        Vector2 pos = SwapSceneManager.instance.currentSceneId == 0 ? posVisible.position : posInvisible.position;
        canvaPlayer.transform.position = pos;
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
        
        if(SwapSceneManager.instance.currentSceneId != 0)
            StartCoroutine(AnimatedCanvaTranslation(posInvisible, posVisible));

        StartCoroutine(SpawnAnimatedSprite(posSpawnSprite, value, sprite, targetPosition));
    }

    IEnumerator AnimatedCanvaTranslation(RectTransform startPos, RectTransform target, float duration = 1f)
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
    }

    IEnumerator SpawnAnimatedSprite(Vector2 posSpawnSprite, int value, GameObject sprite, Vector2 targetPosition)
    {
        for (int i = 0; i < value/10; i++)
        {
            Vector2 randomPos = Random.insideUnitCircle*100 + posSpawnSprite;
            
            GameObject newSprite = Instantiate(sprite, randomPos, Quaternion.identity, canvaPlayer.parent);
            newSprite.GetComponent<AnimatedSprite>().targetPosition = targetPosition;
        }
        
        yield return new WaitForSeconds(2f);
        
        StartCoroutine(AnimatedCanvaTranslation(posVisible, posInvisible));
    }
}