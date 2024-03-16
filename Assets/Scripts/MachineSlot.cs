using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MachineSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MachineRoot root;
    public Color hovered_color;
    public Image hover_image;
    public System.Action<Draggable> dragged_element_delegate;
    public float grab_anim_duration = 1;
    public bool open_mouth = false;
    public Animator animator;
    private bool eating = false;

    private void Start()
    {
        root = GetComponentInParent<MachineRoot>();
        dragged_element_delegate += OnElementDragged;
        DragSystem.instance.drag_start_delegate += OnDragStart;
        DragSystem.instance.drag_end_delegate += OnDragEnd;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DragSystem.instance.hovered_slot = this;
        hover_image.color = hovered_color;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DragSystem.instance.hovered_slot = null;
        hover_image.color = Color.white;
    }

    public void OnElementDragged(Draggable draggable)
    {
        StartCoroutine(ElementDragCoroutine(draggable.transform.position, transform.position, draggable));
    }

    public IEnumerator ElementDragCoroutine(Vector3 start_position, Vector3 target_position, Draggable draggable)
    {
        open_mouth = true;
        eating = true;
        for (float time = 0; time < grab_anim_duration; time += Time.deltaTime)
        {
            draggable.transform.position = Vector3.Lerp(start_position, target_position, time / grab_anim_duration);
            yield return null;
        }
        eating = false;
        animator.SetBool("Open_Mouth", false);
        open_mouth = false;
        for (float time = 0; time < 0.3f; time += Time.deltaTime)
        {
            yield return null;
        }
        root.resource_received_delegate?.Invoke(draggable.GetComponent<ResourceItem>().resource);
        Destroy(draggable.gameObject);
    }
    public void OnDragStart(Draggable draggable)
    {
        if(root.DoesAcceptResource(draggable.GetComponent<ResourceItem>().resource))
        {
            open_mouth = true;
            animator.SetBool("Open_Mouth", true);
        }
    }

    public void OnDragEnd(Draggable draggable)
    {
        if(!eating)
        {
            open_mouth = false;
            animator.SetBool("Open_Mouth", false);
        }
    }
}
