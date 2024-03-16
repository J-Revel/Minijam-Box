using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineOutput : MonoBehaviour
{
    public ResourceConfig resource;
    public Vector3 initial_velocity = Vector3.forward * 10;
    public bool register_rejection;
    public bool register_yield;

    private void Start()
    {
        MachineRoot root = GetComponentInParent<MachineRoot>();
        if (register_rejection)
            root.reject_delegate += OnReject;
        if (register_yield)
            root.yield_delegate += OnYield;
    }

    public void OnYield()
    {
        Transform instance = Instantiate(resource.prefab, transform.position, transform.rotation);
        instance.GetComponent<Rigidbody>().velocity = transform.right * initial_velocity.x + transform.up * initial_velocity.y + transform.forward * initial_velocity.z;
    }

    public void OnReject(ResourceConfig resource)
    {
        Transform instance = Instantiate(resource.prefab, transform.position, transform.rotation);
        instance.GetComponent<Rigidbody>().velocity = transform.right * initial_velocity.x + transform.up * initial_velocity.y + transform.forward * initial_velocity.z;
    }
}
