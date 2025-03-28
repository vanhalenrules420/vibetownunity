using UnityEngine;

public class FloatingNPC : MonoBehaviour
{
    public float floatStrength = 0.5f; 
    public float floatSpeed = 2f;
    public float moveSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Gerakan naik-turun seperti melayang
        float floatY = Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.position = startPos + new Vector3(0, floatY, 0);
    }
}
