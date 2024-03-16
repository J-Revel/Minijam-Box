using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineSlot : MonoBehaviour
{
    private MachineRoot root;
    private void Start()
    {
        root = GetComponentInParent<MachineRoot>();
    }

    private void OnTriggerEnter(Collider other)
    {
        ResourceItem item = other.GetComponent<ResourceItem>();
        
        if(item != null)
        {
            root.resource_received_delegate?.Invoke(item.resource);
            Destroy(item.gameObject);
        }
    }
}
