using System.Linq;
using Lean.Touch;
using UnityEditor.Rendering;
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

    public Animator animator;

    public GameObject flameCircleSpell;
    
    const float MinDistance = 0.5f;
    Vector3 velocity = new Vector3();

    Vector3 targetPosition;

    static readonly int Running = Animator.StringToHash("isRunning");
    bool IsRunning
    {
        set => animator?.SetBool(Running, value);
    }

    bool _arrived;

    bool Arrived
    {
        get { return _arrived; }
        set
        {
            _arrived = value;
            IsRunning = !value;
        }
    }

    void OnEnable()
    {
        Arrived = true;
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
        Arrived = false;
    }

    void Update()
    {
        HandleInputUpdate();
        HandleKeyboardUpdate();
    }

    void HandleKeyboardUpdate()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!GetMouseRayInfo(out RaycastHit info)) return;

            CastFireCircle(info.point);
        }
    }
    public void CastFireCircleCallBack(LeanFinger finger)
    {
        var position = new Vector3();
        var hits = new Vector3[finger.Snapshots.Count];
        var i = 0;

        foreach (LeanSnapshot fingerSnapshot in finger.Snapshots)
        {
            if (!CastTerrainRay(out RaycastHit info, fingerSnapshot.ScreenPosition)) return;
            hits[i++] = info.point;
            position += info.point;
        }

        position /= finger.Snapshots.Count;

        float radius = hits.Average(hit => Vector3.Distance(hit, position));
        float angle = Vector3.SignedAngle(Vector3.right, hits[0] - position, Vector3.up);

        CastFireCircle(position, radius, angle);
    }

    public void CastFireCircle(Vector3 position, float radius = 4f, float angle = 0f)
    {
        var fc = Instantiate(flameCircleSpell, position, flameCircleSpell.transform.rotation)
            .GetComponent<FlameCircle>();
        fc.radius = radius;
        fc.angle = angle;
    }

    bool GetMouseRayInfo(out RaycastHit info)
    {
        Vector2 position = Input.mousePosition;
        return CastTerrainRay(out info, position);
    }

    bool CastTerrainRay(out RaycastHit info, Vector2 position)
    {
        Ray ray = mainCamera.ScreenPointToRay(position);

        return Physics.Raycast(ray, out info, Mathf.Infinity, groundLayer);
    }

    void HandleInputUpdate()
    {
        if (!Input.GetMouseButton(1) || Input.touchCount != 0) return;
        
        if (!GetMouseRayInfo(out RaycastHit info)) return;

        if (Vector3.Distance(info.point, transform.position) < MinDistance) return;
        
        targetPosition = info.point;
        Arrived = false;
    }

    void FixedUpdate()
    {
		bool ground = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

		if(ground)
		{
			velocity.y = -2f;
		}

        if (!Arrived)
        {
            Vector3 direction = targetPosition - transform.position;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);

            float moveAmount = speed;
            if (direction.magnitude <= speed * Time.fixedDeltaTime*2)
            {
                moveAmount = direction.magnitude / Time.fixedDeltaTime;
                Arrived = true;
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
