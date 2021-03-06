using System;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public Transform player;
    public float speed;
    public float range;
    public CharacterController controller;
    public Transform headTarget;
    public Animator animator;
    public float Health = 100f;
    
    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
    static readonly int MoveMultiplier = Animator.StringToHash("MoveMultiplier");

    ImpactReceiver impactReceiver;

    void Start()
    {
        impactReceiver = GetComponent<ImpactReceiver>();
    }

    void Update()
    {
        animator.SetFloat(MoveMultiplier, 0f);
        
        if (!(Vector3.Distance(transform.position, player.position) < range)) return;

        var playerPosition = player.position;
        headTarget.position = new Vector3(playerPosition.x, headTarget.position.y, playerPosition.z);
        
        animator.SetFloat(MoveMultiplier, 1f);
        
        Vector3 direction = playerPosition - transform.position;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        direction.y = 0;
        controller.Move(direction.normalized * (speed * Time.deltaTime));
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        Vector3 dir = transform.position - other.transform.position;
        dir.y = 0;
        Health -= 34;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
        impactReceiver.AddImpact(dir, 10);
    }
}
