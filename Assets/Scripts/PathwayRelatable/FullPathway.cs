using System.Collections.Generic;
using UnityEngine;

public static class FullPathway
{
    public static List<GameObject> Pathway = new List<GameObject>();

    public static void AddTarget(GameObject target, int index)
    {
        if (index >= Pathway.Count)
            Pathway.Add(target);
        else
            Pathway.Insert(index, target);
    }
}