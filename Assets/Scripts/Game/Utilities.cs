﻿using UnityEngine;

public class Transf
{

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;

    public Transf(Vector3 newPosition, Quaternion newRotation, Vector3 newLocalScale)
    {
        position = newPosition;
        rotation = newRotation;
        localScale = newLocalScale;
    }

    public Transf()
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;
        localScale = Vector3.one;
    }

    public Transf(Transform transform)
    {
        copyFrom(transform);
    }

    public void copyFrom(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        localScale = transform.localScale;
    }

    public void copyTo(Transform transform)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = localScale;
    }

}