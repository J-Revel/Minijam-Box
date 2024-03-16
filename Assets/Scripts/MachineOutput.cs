using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineOutput : MonoBehaviour
{
    public ResourceConfig resource;
    public Transform target_position;
    public float appear_duration;
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
        StartCoroutine(YieldAnimationCoroutine());
    }

    public void OnReject(ResourceConfig resource)
    {
    }

    private IEnumerator YieldAnimationCoroutine()
    {
        Transform instance = Instantiate(resource.prefab, transform.position, Quaternion.identity);
        for(float time=0; time < appear_duration; time += Time.deltaTime)
        {
            instance.transform.position = Vector3.Lerp(transform.position, target_position.position, time / appear_duration);
            yield return null;
        }

    }
}
