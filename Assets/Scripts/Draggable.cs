using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour
{
    public Color mouseOverColor = Color.blue;
    public Color originalColor = Color.yellow;
    private new MeshRenderer renderer;
    public new Rigidbody rigidbody;

    public bool hovered;
    public bool highlighted;
    public GameObject highlight;
    public bool dragged;

    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void OnMouseEnter()
    {
        hovered = true;
        DragSystem.instance.OnHover(this, true);
    }

    void OnMouseExit()
    {
        hovered = false;
        DragSystem.instance.OnHover(this, false);
    }

    void Update()
    {
        if(hovered && !highlighted)
        {
            DragSystem.instance.OnHover(this, true);
        }
        highlight.SetActive(highlighted);
    }
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
