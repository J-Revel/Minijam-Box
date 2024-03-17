using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MachineSlot : MonoBehaviour
{
    public MachineRoot root;
    public System.Action<Draggable> eat_element_delegate;
    public float eat_anim_duration = 0.3f;
    public bool on_target = false;
    public System.Action eat_coroutine;
    public delegate Vector3 PositionDelegate();
    public delegate float ScaleDelegate();
    private DragSlot slot;

    private void Awake()
    {
        slot = GetComponent<DragSlot>();
        slot.drag_position_delegate = (Draggable draggable) => { return transform.position;  };
        slot.drag_scale_delegate = (Draggable draggable) => { return 1; };
        slot.drop_delegate = (Draggable draggable) => {
            StartCoroutine(EatCoroutine(draggable));
        };
    }

    private void Start()
    {
        root = GetComponentInParent<MachineRoot>();
        DragSystem.instance.drag_start_delegate += (Draggable draggable) => { on_target = false; };
        GetComponent<DragSlot>().can_drag_delegate += CanDrop;
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

    private IEnumerator EatCoroutine(Draggable draggable)
    {
        while (!draggable.attach_animation_finished)
            yield return null;
        eat_coroutine?.Invoke();
        ResourceConfig resource = draggable.GetComponent<ResourceItem>().resource;
        root.resource_received_delegate?.Invoke(resource);
        yield return draggable.AttachToSlotCoroutine(slot, 0);
        Destroy(draggable.gameObject);
    }
}
