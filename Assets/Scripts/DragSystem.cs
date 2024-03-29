using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSystem : MonoBehaviour
{
    public static DragSystem instance;
    public System.Action<Draggable> drag_start_delegate;
    public System.Action<Draggable, DragSlot> drop_delegate;
    public System.Action<Draggable> release_drag_delegate;

    private void Awake()
    {
        instance = this;
    }

    public int UILayer;
    public float target_height = -5;

    public float attraction_strength = 50;
    public AnimationCurve attraction_curve;
    public Vector2 attraction_curve_bounds;
    public float damping = 0.1f;

    private Draggable hovered_draggable;
    public bool dragging = false;
    public Transform debug_cursor;
    private DragSlot hovered_slot;

    public Draggable dragged_element { get { return dragging ? hovered_draggable : null; } }

    public void Start()
    {
        
    }

    public void OnHover(Draggable draggable, bool hovered)
    {
        if (dragging)
            return;
        if(hovered && draggable != hovered_draggable)
        {
            if (hovered_draggable != null)
                hovered_draggable.highlighted= false;
            hovered_draggable = draggable;
            draggable.highlighted= true;
        }
        else if(!hovered && draggable == hovered_draggable)
        {
            draggable.highlighted= false;
            hovered_draggable = null;
        }
    }


    private void Update()
    {
        if(!dragging && Input.GetMouseButtonDown(0) && hovered_draggable != null && (!IsPointerOverUIElement() || hovered_draggable.draggable_through_ui))
        {
            hovered_draggable.dragged = true;
            dragging = true;
            drag_start_delegate?.Invoke(hovered_draggable);
        }
        else if(dragging && !Input.GetMouseButton(0))
        {
            if(hovered_slot != null && hovered_slot.CanDrop(hovered_draggable))
            {
                drop_delegate?.Invoke(hovered_draggable, hovered_slot);
                hovered_slot.OnDrop(hovered_draggable);
            }
            else
                release_drag_delegate?.Invoke(hovered_draggable);
            dragging = false;
            hovered_draggable.rigidbody.useGravity = true;
        }
    }
    private void FixedUpdate()
    {
        if (hovered_draggable != null && dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, target_height);
            plane.Raycast(ray, out float enter);
            Vector3 target_point = ray.GetPoint(enter);
            Vector3 offset = (target_point - hovered_draggable.transform.position);
            Vector3 attraction_force = GetAttractionStrength(offset);
            debug_cursor.position = target_point;

            Rigidbody draggable_rigidbody = hovered_draggable.rigidbody;
            draggable_rigidbody.useGravity = false;
            draggable_rigidbody.AddForce(attraction_force, ForceMode.Acceleration);
            if (Vector3.Dot(draggable_rigidbody.velocity, offset) < 0)
                draggable_rigidbody.velocity *= damping;
        }
    }

    public Vector3 GetAttractionStrength(Vector3 offset)
    {
        float ratio = attraction_curve.Evaluate((offset.magnitude-attraction_curve_bounds.x) / (attraction_curve_bounds.y - attraction_curve_bounds.x));
        return offset.normalized * ratio * attraction_strength;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData event_data = new PointerEventData(EventSystem.current);
        event_data.position = Input.mousePosition;
        List<RaycastResult> raycast_results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(event_data, raycast_results);
        return raycast_results;
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

    public void SetHoveredSlot(DragSlot slot)
    {
        if(dragging && slot.can_drag_delegate(hovered_draggable))
        {
            hovered_slot = slot;
            hovered_slot.OnDraggingHoverStart(hovered_draggable);
        }
    }

    public void UnhoverSlot(DragSlot slot)
    {
        if(dragging)
        {
            slot.OnDraggingHoverEnd(hovered_draggable);
        }
        hovered_slot = null;
    }
}
