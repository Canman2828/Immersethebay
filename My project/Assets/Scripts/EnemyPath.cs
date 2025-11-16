using UnityEngine;
using System.Collections.Generic;

public class EnemyPath : MonoBehaviour
{
    public Transform[] waypoints;

    void Start()
    {
        var spawner = FindObjectOfType<EnemySpawner>();
        if (spawner == null)
        {
            Debug.LogError("EnemySpawner not found in scene!");
            return;
        }

        List<Vector3> points = new List<Vector3>();
        foreach (var wp in waypoints)
        {
            if (wp != null)
                points.Add(wp.position);
        }

        spawner.SetEnemyPath(points);
    }
}
