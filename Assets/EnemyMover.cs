using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public Transform player;
    public float speed;
    public float range;
    public CharacterController controller;
    
    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < range)
        {
            var direction = player.position - transform.position;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);
            direction.y = 0;
            controller.Move(direction.normalized * (speed * Time.deltaTime));
        }
    }
}
