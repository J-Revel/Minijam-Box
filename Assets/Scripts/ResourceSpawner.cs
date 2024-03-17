using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public Transform[] prefabs;
    public BoxCollider spawn_box;
    public float throw_duration = 0.5f;
    public Vector2 throw_height_range = new Vector2(5, 8);
    public DragSlot slot;

    void Start()
    {
        
    }

    public void Spawn()
    {
        StartCoroutine(SpawnCoroutine());
    }

    public IEnumerator SpawnCoroutine()
    {
        Transform prefab = prefabs[Random.Range(0, prefabs.Length)];
        Transform instance = Instantiate(prefab, transform.position, Random.rotationUniform);
        Vector2 throw_target = new Vector2(Random.Range(spawn_box.bounds.min.x, spawn_box.bounds.max.x), Random.Range(spawn_box.bounds.min.z, spawn_box.bounds.max.z));
        float throw_height = Random.Range(throw_height_range.x, throw_height_range.y);
        instance.position = transform.position;
        Draggable draggable = instance.GetComponent<Draggable>();
        draggable.FixToSlot(slot, 1);
        draggable.draggable_through_ui = true;
        for (float time = 0; time < throw_duration; time += Time.deltaTime)
        {
            
            float f = time / throw_duration;
            instance.position = new Vector3(Mathf.Lerp(transform.position.x, throw_target.x, f), transform.position.y, Mathf.Lerp(transform.position.z, throw_target.y, f) + throw_height * Mathf.Sin(f * Mathf.PI));
            yield return null;
        }
    }
}
