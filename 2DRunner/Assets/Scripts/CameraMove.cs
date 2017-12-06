using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    Transform target;

    float cameraSpeed = 1.0f;

    bool startmove = false;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Character").transform;
    }

    void Update ()
    {
	    if (startmove)
        {
            Move();
        }
	}

    private void Move()
    {
        Vector3 direction = new Vector3(target.position.x + 5.0f, transform.position.y, -10.0f);
        transform.position = Vector3.Lerp(transform.position, direction, cameraSpeed * Time.deltaTime);
    }

    public void StartMove()
    {
        startmove = true;
    }

    public void StopMove()
    {
        startmove = false;
    }
}
