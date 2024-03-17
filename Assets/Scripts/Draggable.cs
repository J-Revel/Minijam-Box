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

    public float attach_animation_duration = 0.3f;
    public MachineSlot attached_slot;
    public bool attach_animation_finished = false;

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

    public void AttachToSlot(MachineSlot slot)
    {
        StopAllCoroutines();
        attached_slot = slot;
        StartCoroutine(AttachToSlotCoroutine(slot.transform.position, 1));
    }

    public void ReleaseFromSlot(MachineSlot slot)
    {
        attached_slot = null;
        StopAllCoroutines();
    }

    public IEnumerator AttachToSlotCoroutine(Vector3 target_position, float target_scale)
    {
        attach_animation_finished = false;
        Vector3 start_position = transform.position;
        Vector3 start_scale = transform.localScale;
        for(float time=0; time < attach_animation_duration; time += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(start_position, target_position, time / attach_animation_duration);
            transform.localScale = Vector3.Lerp(start_scale, Vector3.one * target_scale, time / attach_animation_duration);
            yield return null;
        }
        attach_animation_finished = true;
        while(true)
        {
            transform.position = target_position;
            yield return null;

        }
    }

    public IEnumerator WaitReachSlotCoroutine()
    {
        while (!attach_animation_finished)
            yield return null;
    }
}
