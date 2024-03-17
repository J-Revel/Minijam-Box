using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachSlot : MonoBehaviour
{
    public LayerMask beach_layer_mask;
    public Vector2 height_range;
    public Vector2 scale_range;
    public float height_offset = 0.1f;
    public void Start()
    {
        DragSlot drag_slot = GetComponent<DragSlot>();
        drag_slot.drag_position_delegate = (Draggable draggable) =>
        {
            Ray ray;
            if (DragSystem.instance.dragged_element == draggable)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            }
            else ray = new Ray(draggable.transform.position, Vector3.down);
            return new Vector3(ray.origin.x, transform.position.y + height_offset, ray.origin.z);
        };
        drag_slot.drag_scale_delegate = (Draggable draggable) =>
        {
            Ray ray;
            if (DragSystem.instance.dragged_element == draggable)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            }
            else ray = new Ray(draggable.transform.position, Vector3.down);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit, 1000, beach_layer_mask))
            {
                return Mathf.Lerp(scale_range.x, scale_range.y, (hit.point.y - height_range.x) / (height_range.y - height_range.x));
            }
            return draggable.transform.localScale.x;
        };
        drag_slot.drop_delegate = (Draggable draggable) =>
        {
            StartCoroutine(AttachCoroutine(draggable));
        };
    }
    public IEnumerator AttachCoroutine(Draggable draggable)
    {
        Vector3 position = draggable.transform.position;
        Vector3 localScale = draggable.transform.localScale;
        yield return null;
        draggable.draggable_through_ui = true;
        while(DragSystem.instance.dragged_element != draggable)
        {
            draggable.transform.position = position;
            draggable.transform.localScale = localScale;
            yield return null;
        }
        draggable.draggable_through_ui = false;
    }
}
