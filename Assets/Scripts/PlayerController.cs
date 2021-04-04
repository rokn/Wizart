using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    //public Transform cam;
    public float speed;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;

    public float gravityForce = 9.81f;

    Vector3 velocity = new Vector3();

    Vector3 targetPosition;
    bool arrived = true;

	void Update()
	{
        if(Input.GetMouseButtonDown(1) && Input.touchCount == 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit info;
            if(Physics.Raycast(ray, out info, Mathf.Infinity, groundLayer))
            {
                targetPosition = info.point;
                arrived = false;
            }
        }
    }
	
	void FixedUpdate()
    {
		bool onGround = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

		if(onGround)
		{
			velocity.y = -2f;
		}

        if (!arrived)
        {
            Vector3 direction = targetPosition - transform.position;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);

            float moveAmount = speed;
            if (direction.magnitude <= speed * Time.fixedDeltaTime*2)
            {
                moveAmount = direction.magnitude / Time.fixedDeltaTime;
                arrived = true;
			}
			Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * moveAmount;
            velocity = new Vector3(moveDirection.x, velocity.y, moveDirection.z);
        }

        controller.Move(velocity * Time.fixedDeltaTime);
        velocity.x = 0f;
		velocity.y += -gravityForce * Time.fixedDeltaTime;
		velocity.z = 0f;
    }
}
