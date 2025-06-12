using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinesManager : MonoBehaviour
{
    [SerializeField] GameObject lines;
    [SerializeField] GameObject[] numbers;

    public static LinesManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        HideLines();
        SetNumbersInactive();
    }

    public void ShowHideLines()
    {
        lines.SetActive(!lines.activeInHierarchy);
    }

    public void HideLines()
    {
        lines.SetActive(false);
    }

    public void SetNumbersInactive()
    {
        BroadcastMessage("ShowInactive");
    }

    public void HighlightNumber(int number)
    {
        numbers[number - 1].GetComponent<NumberHighlighter>().ShowActive();
    }
}
