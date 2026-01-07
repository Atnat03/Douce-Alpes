using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HandAnimation : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private Image handImage;

    [SerializeField] private Vector3[] targets;

    void OnEnable()
    {
        handImage.gameObject.SetActive(true);
    }
    
    void Start()
    {
        handImage.sprite = sprite;

        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        yield return new WaitForSeconds(1f);
        
        
    }
    
    
}
