using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolMB : MonoBehaviour
{
    [SerializeField] GameObject frame;

    void OnEnable()
    {
        Idle();
    }

    public void Highlight()
    {
        frame.SetActive(true);
    }

    public void Idle()
    {
        frame.SetActive(false);
    }
}
