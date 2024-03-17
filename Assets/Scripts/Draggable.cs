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
    public DragSlot attached_slot;
    public bool attach_animation_finished = false;
    public bool draggable_through_ui = false;

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

    public void AttachToSlot(DragSlot slot)
    {
        StopAllCoroutines();
        attached_slot = slot;
        StartCoroutine(AttachToSlotCoroutine(slot, 1));
    }

    public void ReleaseFromSlot(DragSlot slot)
    {
        attached_slot = null;
        StopAllCoroutines();
        StartCoroutine(ReleaseFromSlotCoroutine());
    }

    public IEnumerator GoToSlotCoroutine(DragSlot slot, float target_scale_multiplier)
    {
        attach_animation_finished = false;
        Vector3 start_position = transform.position;
        Vector3 start_scale = transform.localScale;
        
        for(float time=0; time < attach_animation_duration; time += Time.deltaTime)
        {
            Vector3 target_position = slot.drag_position_delegate.Invoke(this);
            float target_scale = slot.drag_scale_delegate.Invoke(this) * target_scale_multiplier;
            transform.position = Vector3.Lerp(start_position, target_position, time / attach_animation_duration);
            transform.localScale = Vector3.Lerp(start_scale, Vector3.one * target_scale, time / attach_animation_duration);
            yield return null;
        }
        attach_animation_finished = true;

    }

    public IEnumerator AttachToSlotCoroutine(DragSlot slot, float target_scale_multiplier)
    {
        yield return GoToSlotCoroutine(slot, target_scale_multiplier);
        yield return FixToSlotCoroutine(slot, target_scale_multiplier);
    }

    public void FixToSlot(DragSlot slot, float scale_multiplier)
    {
        StartCoroutine(FixToSlotCoroutine(slot, scale_multiplier));
    }

    public IEnumerator FixToSlotCoroutine(DragSlot slot, float target_scale_multiplier)
    {
        while(true)
        {
            transform.position = slot.drag_position_delegate.Invoke(this);
            float target_scale = slot.drag_scale_delegate.Invoke(this) * target_scale_multiplier;
            transform.localScale = Vector3.one * target_scale;
            yield return null;
        }
    }
    public IEnumerator ReleaseFromSlotCoroutine()
    {
        attach_animation_finished = false;
        Vector3 start_scale = transform.localScale;
        Quaternion rotation = transform.rotation;
        
        for(float time=0; time < attach_animation_duration; time += Time.deltaTime)
        {
            float target_scale = 1;
            transform.localScale = Vector3.Lerp(start_scale, Vector3.one * target_scale, time / attach_animation_duration);
            yield return null;
        }
        while(true)
        {
            float target_scale = 1;
            transform.localScale = Vector3.one * target_scale;
            yield return null;

        }
    }

    public IEnumerator WaitReachSlotCoroutine()
    {
        while (!attach_animation_finished)
            yield return null;
    }
}
