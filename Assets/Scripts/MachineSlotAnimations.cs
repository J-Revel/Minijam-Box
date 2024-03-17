using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MachineSlotAnimations : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MachineRoot root;
    public Color hovered_color;
    public Image hover_image;
    public float grab_anim_duration = 1;
    public bool open_mouth = false;
    public Animator animator;

    private void Start()
    {
        root = GetComponentInParent<MachineRoot>();
        DragSystem.instance.drag_start_delegate += OnDragStart;
        MachineSlot slot = GetComponent<MachineSlot>();
        slot.eat_coroutine += () => { OnEat(); };
        root.yield_prepare_delegate += () =>
        {
            animator.SetTrigger("Produce");
        };
        DragSystem.instance.release_drag_delegate += OnDragEnd;
        DragSystem.instance.drop_delegate += (Draggable draggable, MachineSlot slot) => { OnDragEnd(draggable); };
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hover_image.color = hovered_color;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hover_image.color = Color.white;
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
        open_mouth = false;
        animator.SetBool("Open_Mouth", false);
    }

    public void OnEat()
    {
        animator.SetTrigger("Eat");
    }
}
