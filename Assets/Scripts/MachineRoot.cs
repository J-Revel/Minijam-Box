using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineRoot : MonoBehaviour
{
    public System.Action<ResourceConfig> resource_received_delegate;
    public System.Action yield_delegate;
    public System.Action<ResourceConfig> reject_delegate;
    public MachineRecipeConfig config;
    private int[] stocks;
    public float production_duration = 1;


    private void Awake()
    {
        resource_received_delegate = OnResourceReceived;
        stocks = new int[config.inputs.Length];
    }

    private void OnResourceReceived(ResourceConfig resource)
    {
        bool stock_found = false;
        for(int i=0; i<config.inputs.Length; i++)
        {
            if (config.inputs[i].resource == resource)
            {
                if (config.inputs[i].count > stocks[i])
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
                if (config.inputs[i].count != stocks[i])
                {
                    return;
                }
            }
            StartCoroutine(YieldCoroutine());
        }
    }

    private IEnumerator YieldCoroutine()
    {
        for(float time=0; time < production_duration; time += Time.deltaTime)
        {
            yield return null;
        }
        for(int i=0; i<stocks.Length; i++)
        {
            stocks[i] -= config.inputs[i].count;
        }
        yield_delegate?.Invoke();
    }
}
