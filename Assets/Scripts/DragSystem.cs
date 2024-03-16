using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSystem : MonoBehaviour
{
    public static DragSystem instance;

    private void Awake()
    {
        instance = this;
    }

    private new MeshRenderer renderer;
    public int UILayer;
    public float target_height = 5;
    private new Rigidbody rigidbody;

    public float attraction_strength = 50;
    public AnimationCurve attraction_curve;
    public Vector2 attraction_curve_bounds;
    public float damping = 0.1f;

    private Draggable hovered_draggable;
    private bool dragging = false;

    public void Start()
    {
        
    }

    public void OnHover(Draggable draggable, bool hovered)
    {
        if(hovered && draggable != hovered_draggable)
        {
            if(!dragging)
            {
                if (hovered_draggable != null)
                    hovered_draggable.highlighted= false;
                hovered_draggable = draggable;
                draggable.highlighted= true;
            }
        }
        else if(!hovered && draggable == hovered_draggable)
        {
            draggable.highlighted= false;
            hovered_draggable = null;
        }
    }


    private void Update()
    {
        if(!dragging && Input.GetMouseButtonDown(0) && !IsPointerOverUIElement() && hovered_draggable != null)
        {
            hovered_draggable.dragged = true;
            dragging = true;
        }
    }

    public Vector3 GetAttractionStrength(Vector3 offset)
    {
        float ratio = attraction_curve.Evaluate((offset.magnitude-attraction_curve_bounds.x) / (attraction_curve_bounds.y - attraction_curve_bounds.x));
        return offset.normalized * ratio;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }
}
