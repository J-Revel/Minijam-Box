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

    public int UILayer;
    public float target_height = -5;

    public float attraction_strength = 50;
    public AnimationCurve attraction_curve;
    public Vector2 attraction_curve_bounds;
    public float damping = 0.1f;

    private Draggable hovered_draggable;
    private bool dragging = false;
    public Transform debug_cursor;
    public MachineSlot hovered_slot;

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
        if(!dragging && Input.GetMouseButtonDown(0) && !IsPointerOverUIElement() && hovered_draggable != null)
        {
            hovered_draggable.dragged = true;
            dragging = true;
        }
        else if(dragging && !Input.GetMouseButton(0))
        {
            if(hovered_slot != null)
            {
                hovered_slot.dragged_element_delegate?.Invoke(hovered_draggable);
            }
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
            /*if(draggable_rigidbody.velocity.magnitude * Time.fixedDeltaTime > offset.magnitude)
            {
                draggable_rigidbody.velocity = offset / Time.fixedDeltaTime;
            }*/
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
}
