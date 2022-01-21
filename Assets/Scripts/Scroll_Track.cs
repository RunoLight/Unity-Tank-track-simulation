using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll_Track : MonoBehaviour {

    [SerializeField]
    private float scrollSpeed = 0.05f;

    private float offset = 0.0f;
    private Renderer r;

    void Start()
    {
        r = GetComponent<Renderer>();
    }

    void Update()
    {
        offset = (offset + Time.deltaTime * scrollSpeed) % 1f;
        r.material.SetTextureOffset("_MainTex", new Vector2(offset, 0f));
    }
}
