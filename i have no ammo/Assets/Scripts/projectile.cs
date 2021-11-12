using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ProjectileBehavior(projectile projectile);

public class projectile : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    public float speedFloor;
    public float speedCap;
    public float acceleration;
    public float rotationSpeed;
    public float rotationSpeedFloor;
    public float rotationSpeedCap;
    public float rotationAcceleration;
    public Vector2 direction = Vector2.right;
    public ProjectileBehavior behavior;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (behavior != null)
        {
            behavior(this);
        }

        speed += acceleration * Time.deltaTime;
        speed = Mathf.Clamp(speed, speedFloor, speedCap);
        rotationSpeed += rotationAcceleration * Time.deltaTime;
        rotationSpeed = Mathf.Clamp(rotationSpeed, rotationSpeedFloor, rotationSpeedCap);

        direction = Quaternion.Euler(0, 0, rotationAcceleration * Time.deltaTime) * direction;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        rb.velocity = speed * direction;
    }
}
