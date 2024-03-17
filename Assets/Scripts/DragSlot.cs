using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSlot: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float eat_anim_duration = 0.3f;
    public bool on_target = false;
    public delegate Vector3 PositionDelegate(Draggable draggable);
    public delegate float ScaleDelegate(Draggable draggable);
    public delegate bool CanDragDelegate(Draggable draggable);

    public PositionDelegate drag_position_delegate;
    public ScaleDelegate drag_scale_delegate;
    public CanDragDelegate can_drag_delegate;

    public System.Action<Draggable> drop_delegate;

    private void Awake()
    {
        drag_position_delegate = (Draggable draggable) => { return transform.position; };
        drag_scale_delegate = (Draggable draggable) => { return 1; };
        can_drag_delegate = (Draggable draggable) => { return true; };
    }

    private void Start()
    {
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
        return can_drag_delegate(draggable);
    }

    public void OnDrop(Draggable draggable)
    {
        drop_delegate?.Invoke(draggable);
    }

    public void OnDraggingHoverStart(Draggable draggable)
    {
        draggable.AttachToSlot(this);
    }

    public void OnDraggingHoverEnd(Draggable draggable)
    {
        draggable.ReleaseFromSlot(this);
    }
}
