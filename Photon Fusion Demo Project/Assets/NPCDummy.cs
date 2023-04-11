using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDummy : MonoBehaviour
{
    public Transform targetPosition;
    public float speed;
    public bool isMoving = false;

    private void Update()
    {
        MoveToPosition();
    }

    public void MovePlayer()
    {
        isMoving = true;
    }

    public void MoveToPosition()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position ,targetPosition.position, speed * Time.deltaTime);
        }

        if (transform.position == targetPosition.position)
            isMoving = false;
    }

    private void OnMouseDown()
    {
        Debug.Log("Test");
    }
}
