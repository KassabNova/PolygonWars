using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomMove : MonoBehaviour
{
    public Vector3 axis = new Vector3(0, 1, 0);
    public float degrees = 90f;
    public float timespan = 1f;

    private float _rotated = 0;
    private Vector3 _rotationVector;

    public void Start()
    {
        axis.Normalize();
        _rotationVector = axis * degrees;
    }

    public void Update()
    {
        _rotated += degrees * (Time.deltaTime / timespan);
        transform.Rotate(_rotationVector * (Time.deltaTime / timespan));
    }
}
