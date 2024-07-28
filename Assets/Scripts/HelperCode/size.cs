using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class size : MonoBehaviour
{
    [SerializeField] private GameObject ToMeasure;

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = ToMeasure.GetComponent<MeshFilter>().mesh;
        Vector3 size = mesh.bounds.size;
        Debug.Log(size);
    }

}
