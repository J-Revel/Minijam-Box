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
    private new Rigidbody rigidbody;

    public bool hovered;
    public bool highlighted;
    public bool dragged;
    public float target_height = 3.5f;

    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        rigidbody = GetComponent<Rigidbody>();
        renderer.material.color = originalColor;
    }

    void OnMouseEnter()
    {
        hovered = true;
    }

    void OnMouseExit()
    {
        hovered = false;
    }

    private void FixedUpdate()
    {
        if (dragged)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, target_height);
            plane.Raycast(ray, out float enter);
            Vector3 target_point = ray.GetPoint(enter);
            Vector3 offset = (target_point - transform.position);
            Vector3 attraction_force = DragSystem.instance.GetAttractionStrength(offset);

            rigidbody.AddForce(attraction_force, ForceMode.Acceleration);
            if (Vector3.Dot(rigidbody.velocity, offset) < 0)
                rigidbody.velocity *= DragSystem.instance.damping;
            /*if(rigidbody.velocity.magnitude * Time.fixedDeltaTime > offset.magnitude)
            {
                rigidbody.velocity = offset / Time.fixedDeltaTime;
            }*/
        }
    }
    void Update()
    {
        if(dragged)
        {
            renderer.material.color = mouseOverColor;
        }
        else if(hovered)
        {
            if(DragSystem.instance.IsPointerOverUIElement())
                renderer.material.color = originalColor;
            else 
                renderer.material.color = mouseOverColor;
        }
        else 
            renderer.material.color = originalColor;
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
