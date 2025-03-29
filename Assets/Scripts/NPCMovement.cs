using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform[] waypoints; // Array untuk titik jalur NPC
    private int currentWaypointIndex = 0;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movementDirection;
    private bool isStopped = false; // Variable to track if NPC is stopped

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (waypoints.Length > 0)
            transform.position = waypoints[currentWaypointIndex].position;
    }

    void Update()
    {
        if (!isStopped) // NPC only moves if not stopped
        {
            Move();
        }
    }

    void Move()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector2 direction = (target.position - transform.position).normalized;
        movementDirection = direction;

        rb.linearVelocity = direction * moveSpeed; // Use velocity instead of linearVelocity

        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    public void StopMoving()
    {
        isStopped = true;
        rb.linearVelocity = Vector2.zero; // Stop movement
        animator.SetFloat("Speed", 0);
    }

    public void ResumeMoving()
    {
        isStopped = false; // Allow movement again
    }
}
