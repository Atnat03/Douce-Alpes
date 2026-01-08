using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarnetTricot : MonoBehaviour
{
    public List<GameObject> pages = new();
    
    [SerializeField] int indexCurrentPage = 0;
    [SerializeField] Button nextButton;
    [SerializeField] Button previousButton;
    [SerializeField] Animator animator;
    [SerializeField] TricotManager tricotManager;

    public void AddNewPage(GameObject page)
    {
        pages.Add(page);
    }

    public void UpdateCarnet()
    {
        AudioManager.instance.PlaySound(13);
        
        nextButton.gameObject.SetActive(indexCurrentPage < pages.Count - 1);
        previousButton.gameObject.SetActive(indexCurrentPage > 0);
        
        for (int i = 0; i < pages.Count; i++)
        {
            if (i == indexCurrentPage)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }
    }

    public void NextPage()
    {
        animator.SetTrigger("Next");
        indexCurrentPage++;
        UpdateCarnet();
    }

    public void PreviousPage()
    {
        animator.SetTrigger("Previous");
        indexCurrentPage--;
        UpdateCarnet();
    }
}
