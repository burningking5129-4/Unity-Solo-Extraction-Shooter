using JetBrains.Annotations;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public bool isFollowing = false;
    public bool isAttacking = false;

    public float kbDistance = 2;
    public float targetDistance;
    public float dmgTime = 2;

    public int health = 100;
    public int maxHealth = 100;

    public int detectionrange = 5;

    public NavMeshAgent agent;
    public PlayerController player;
    public Weapon Weapon;
    public Proj Proj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float targetDistance = Vector3.Distance(player.transform.position, transform.position);

        isFollowing = targetDistance <= detectionrange;

        if (isFollowing)
        {
            agent.destination = player.transform.position;
        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    
        private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.hp -= 10;
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;

            isAttacking = true;
        }
        if (collision.gameObject.tag == "Projectile")
        {
            health -= 50;
        }

    }
    
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine("dmgCooldown");
            isAttacking = true;
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            gameObject.GetComponent <NavMeshAgent>().isStopped = false;
            isAttacking = false;
            StopCoroutine("dmgCooldown");
        }
    }

    IEnumerator dmgCooldown()
    {
        yield return new WaitForSeconds(1);
        if (isAttacking == true)
        {
            player.hp -= 10;
        }
        isAttacking = false;
        StopCoroutine("dmgCooldown");
    }
}

   

