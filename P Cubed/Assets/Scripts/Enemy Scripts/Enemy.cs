using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    public float moveSpeed;
    public int health;
    public int pathNumber;
    private int pathIndex = 0;
    private Transform nextStep;

    //enemy constructor for certain testing purposes
    public Enemy(float moveSpeed, int health)
    {
        this.moveSpeed = moveSpeed;
        this.health = health;
    }

    //initial start for enemies to allow for testing
    void Start()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        Pathing.SetPath(pathNumber);
        nextStep = Pathing.pathPoints[0];
    }

    //currently just moves enemies along the path 
    private void Update()
    {
        Vector2 movement = nextStep.position - transform.position;
        transform.Translate(movement.normalized * moveSpeed * Time.deltaTime, Space.World);
        if (Vector2.Distance(transform.position, nextStep.position) <= .2f)
        {
            GetNextStep();
        }
    }

    //when a path point is reached next point is obtained if endpoint is reached enemies attack and die
    private void GetNextStep()
    {
        if (pathIndex >= Pathing.pathPoints.Length - 1)
        {
            Attack();
            return;
        }
        pathIndex++;
        nextStep = Pathing.pathPoints[pathIndex];
    }

    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        if(health <= 0)
        {
            Die();
        }
    }

    //enemy destroyed generic float of 1 returned for once damage is hooked up
    public float Attack()
    {
        Die();
        return 1;
    }

    public void Die()
    {
        Spawner.EnemiesAlive--;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player") 
        {
            other.GetComponent<Player>().TakeDamage();
        }
    }

}
