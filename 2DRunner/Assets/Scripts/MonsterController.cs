using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {

    Transform target;
    [SerializeField]
    float monsterSpeed = 3.0f;

	void Start ()
    {
        target = GameObject.FindGameObjectWithTag("Character").transform;		
	}
	
	void Update ()
    {
        try
        {
            Vector3 direction = new Vector3(target.position.x, transform.position.y, 0.0f);
            transform.position = Vector3.MoveTowards(transform.position, direction, monsterSpeed * Time.deltaTime);
        }
        catch { }
	}
}
