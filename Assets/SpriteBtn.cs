using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpriteBtn : MonoBehaviour
{

    public UnityEvent OnClickEvent;

    private void OnMouseUp()
    {
        transform.localScale = Vector3.one;
    }

    private void OnMouseDown()
    {
        transform.localScale = Vector3.one * 0.9f;

        if (OnClickEvent != null)
        { OnClickEvent.Invoke(); }
    }
}
