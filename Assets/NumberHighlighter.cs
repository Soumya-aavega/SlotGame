using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberHighlighter : MonoBehaviour
{
    [SerializeField] GameObject active;
    [SerializeField] GameObject inactive;

    void Start()
    {
        ShowInactive();
    }

    public void ShowActive()
    {
        active.SetActive(true);
        inactive.SetActive(false);
    }

    public void ShowInactive()
    {
        active.SetActive(false);
        inactive.SetActive(true);
    }
}
