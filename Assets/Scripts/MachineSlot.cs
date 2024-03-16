using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MachineSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MachineRoot root;
    public GameObject hovered_state;
    public System.Action<Draggable> dragged_element_delegate;
    public float grab_anim_duration = 1;

    private void Start()
    {
        root = GetComponentInParent<MachineRoot>();
        dragged_element_delegate += OnElementDragged;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DragSystem.instance.hovered_slot = this;
        hovered_state.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered_state.SetActive(false);
        DragSystem.instance.hovered_slot = null;
    }

    public void OnElementDragged(Draggable draggable)
    {
        StartCoroutine(ElementDragCoroutine(draggable.transform.position, transform.position, draggable));
    }

    public IEnumerator ElementDragCoroutine(Vector3 start_position, Vector3 target_position, Draggable draggable)
    {
        for (float time = 0; time < grab_anim_duration; time += Time.deltaTime)
        {
            draggable.transform.position = Vector3.Lerp(start_position, target_position, time / grab_anim_duration);
            yield return null;
        }
        Destroy(draggable.gameObject);
    }
}
