using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed;
    void Update()
    {
        Vector2 offset = new Vector2(Time.time * scrollSpeed, scrollSpeed);
        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }
}
