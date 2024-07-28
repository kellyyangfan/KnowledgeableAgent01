using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform objFollow;
    [SerializeField] private float speedCamera;


    void Update()
    {
        this.transform.RotateAround(objFollow.transform.position, Vector3.up, speedCamera * Time.deltaTime);
    }
}
