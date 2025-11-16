using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Opponent))]
public class OpponentPathFromWaypoints : MonoBehaviour
{
    public Transform[] waypoints;

    private void Start()
    {
        var opponent = GetComponent<Opponent>();
        if (opponent == null || waypoints == null || waypoints.Length == 0) return;

        var pts = new List<Vector3>();
        foreach (var t in waypoints)
            pts.Add(t.position);

        opponent.SetPath(pts);
    }
}
