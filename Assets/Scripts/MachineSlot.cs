using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MachineSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MachineRoot root;
    public System.Action<Draggable> eat_element_delegate;
    public float eat_anim_duration = 0.3f;
    public bool on_target = false;
    public System.Action eat_coroutine;

    private void Start()
    {
        root = GetComponentInParent<MachineRoot>();
        DragSystem.instance.drag_start_delegate += (Draggable draggable) => { on_target = false; };
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DragSystem.instance.SetHoveredSlot(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DragSystem.instance.UnhoverSlot(this);
    }

    public bool CanDrop(Draggable draggable)
    {
        ResourceConfig resource = draggable.GetComponent<ResourceItem>().resource;
        return root.DoesAcceptResource(resource);
    }

    public void OnDrop(Draggable draggable)
    {
        StartCoroutine(EatCoroutine(draggable));
    }

    public void OnDraggingHover(Draggable draggable)
    {
        draggable.AttachToSlot(this);
    }

    public void EatDraggable(Draggable draggable)
    {
        StartCoroutine(EatCoroutine(draggable));
    }

    private IEnumerator EatCoroutine(Draggable draggable)
    {
        while (!draggable.attach_animation_finished)
            yield return null;
        eat_coroutine?.Invoke();
        /*
        drop_coroutines.Clear();
        collect_drop_coroutines_delegate?.Invoke();
        foreach (IEnumerator coroutine in drop_coroutines)
            yield return coroutine;*/
        ResourceConfig resource = draggable.GetComponent<ResourceItem>().resource;
        root.resource_received_delegate?.Invoke(resource);
        yield return draggable.AttachToSlotCoroutine(transform.position, 0);
        Destroy(draggable.gameObject);
    }
}
