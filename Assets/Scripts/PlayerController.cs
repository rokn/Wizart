using Lean.Touch;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    
    public float speed;
    public float turnSmoothTime = 0.2f;
    public Camera mainCamera;
    float turnSmoothVelocity;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;

    public float gravityForce = 9.81f;

    const float MinDistance = 0.5f;
    Vector3 velocity = new Vector3();

    Vector3 targetPosition;
    bool arrived = true;

    void OnEnable()
    {
        LeanTouch.OnFingerTap += HandleFingerTap;
    }

    void OnDisable()
    {
        LeanTouch.OnFingerTap -= HandleFingerTap;
    }

    void HandleFingerTap(LeanFinger finger)
    {
        Ray ray = mainCamera.ScreenPointToRay(finger.ScreenPosition);

        bool hit = Physics.Raycast(ray, out RaycastHit info, Mathf.Infinity, groundLayer);
        if (!hit) return;
        
        targetPosition = info.point;
        arrived = false;
    }

    void Update()
    {
        HandleInputUpdate();
    }

    void HandleInputUpdate()
    {
        if (!Input.GetMouseButton(1) || Input.touchCount != 0) return;
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        bool hit = Physics.Raycast(ray, out RaycastHit info, Mathf.Infinity, groundLayer);
        if (!hit) return;

        if (Vector3.Distance(info.point, transform.position) < MinDistance) return;
        
        targetPosition = info.point;
        arrived = false;
    }

    void FixedUpdate()
    {
		bool ground = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

		if(ground)
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
