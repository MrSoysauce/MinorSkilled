using System.Collections.Generic;
using UnityEngine;

public class Waypoints : ScriptableObject
{
    [SerializeField] public bool loop;
    [SerializeField] public bool ordered;
    public List<Vector3> waypoints;
}