using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineRoot : MonoBehaviour
{
    public System.Action<ResourceConfig> resource_received_delegate;
    public System.Action yield_prepare_delegate;
    public System.Action yield_delegate;
    public System.Action<ResourceConfig> reject_delegate;
    public MachineRecipeConfig config;
    public int[] stocks;
    public float production_duration = 1;


    private void Awake()
    {
        resource_received_delegate = OnResourceReceived;
        stocks = new int[config.inputs.Length];
    }

    public bool DoesAcceptResource(ResourceConfig resource)
    {
        for(int i=0; i<config.inputs.Length; i++)
        {
            if (config.inputs[i].resource == resource)
            {
                if (1 > stocks[i])
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnResourceReceived(ResourceConfig resource)
    {
        bool stock_found = false;
        for(int i=0; i<config.inputs.Length; i++)
        {
            if (config.inputs[i].resource == resource)
            {
                if (1 > stocks[i])
                {
                    stocks[i]++;
                    stock_found = true;
                    break;
                }
            }
        }
        if(!stock_found)
        {
            reject_delegate?.Invoke(resource);
        }
        else
        {
            for (int i = 0; i < stocks.Length; i++)
            {
                if (1 != stocks[i])
                {
                    return;
                }
            }
            StartCoroutine(YieldCoroutine());
        }
    }

    private IEnumerator YieldCoroutine()
    {
        yield_prepare_delegate?.Invoke();
        for(float time=0; time < production_duration; time += Time.deltaTime)
        {
            yield return null;
        }
        for(int i=0; i<stocks.Length; i++)
        {
            stocks[i] -= 1;
        }
        yield_delegate?.Invoke();
    }
}
