using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ResourceConfig : ScriptableObject
{
    public string name;
    public int sell_price;
    public Transform prefab;
}
