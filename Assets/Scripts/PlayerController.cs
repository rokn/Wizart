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
    public float jumpAmount;

    Vector3 velocity = new Vector3();
    bool jumpPressed = false;

    // Update is called once per frame
    void FixedUpdate()
    {
		bool onGround = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

		if(onGround)
		{
			velocity.y = -2f;
		}


		//if (Input.GetAxis("Jump") > 0 && onGround)
		//{
		//velocity.y += jumpAmount;
		//jumpPressed = true;
		//}
		//      if (Input.GetAxis("Jump") < Mathf.Epsilon)
		//{
		//          jumpPressed = false;
		//}

		float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(-vertical, 0, horizontal).normalized;

        if (direction.magnitude > 0.1)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * speed;
            velocity = new Vector3(moveDirection.x, velocity.y, moveDirection.z);
        }

        controller.Move(velocity * Time.fixedDeltaTime);
        velocity.x = 0f;
		velocity.y += -gravityForce * Time.deltaTime;
		velocity.z = 0f;
    }
}
