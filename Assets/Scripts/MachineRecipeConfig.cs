using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MachineRecipeConfig : ScriptableObject 
{
    [System.Serializable]
    public struct ResourceStock
    {
        public ResourceConfig resource;
        public int count;
    }
    public ResourceStock[] inputs;
    public ResourceStock[] outputs;
    public float output_delays;

}
