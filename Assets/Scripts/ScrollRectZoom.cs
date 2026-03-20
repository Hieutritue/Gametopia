using UnityEngine;
using UnityEngine.UI;

public class ScrollRectZoom : MonoBehaviour
{
    public ScrollRect scrollRect;

    public float zoomSpeed = 0.2f;
    public float minZoom = 0.5f;
    public float maxZoom = 2f;

    RectTransform content;

    void Awake()
    {
        content = scrollRect.content;
    }

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll == 0) return;

        Zoom(scroll);
    }

    void Zoom(float delta)
    {
        float scale = content.localScale.x;

        scale += delta * zoomSpeed;
        scale = Mathf.Clamp(scale, minZoom, maxZoom);

        content.localScale = Vector3.one * scale;
    }
}