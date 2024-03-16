using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MachineChecklist : MonoBehaviour
{
    public MachineStockElement stock_element_prefab;
    private MachineRoot root;

    private List<MachineStockElement> elements = new List<MachineStockElement>();

    void Start()
    {
        root = GetComponentInParent<MachineRoot>();
        int count = root.config.inputs.Length;
        for (int i = 0; i < count; i++)
        {
            elements.Add(Instantiate(stock_element_prefab, transform));
            elements[i].checkbox.enabled = false;
        } 
    }

    private void Update()
    {
        for (int i = 0; i < elements.Count; i++)
        {
            if (root.stocks[i] > 0)
            {
                elements[i].checkbox.enabled = true;
            }
        }
    }
}
