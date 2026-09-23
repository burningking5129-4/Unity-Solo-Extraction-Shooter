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

    public float distanceToStopX = 2;
    public float distanceToStopY = 2;
    public float targetDistance;
    public float dmgTime = 2;

    public int health = 100;
    public int maxHealth = 100;

    public int detectionrange = 5;

    public NavMeshAgent agent;
    public PlayerController player;

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
    }
    
        private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.hp -= 10;
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;

            isAttacking = true;
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isAttacking && collision.gameObject.tag == "Player")
        {
            player.hp -= 10;
            StartCoroutine("dmgCooldown");
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
        
        yield return new WaitForSeconds(dmgTime);
        isAttacking = false;
        //player.GetComponent<Rigidbody>().AddForce(player.playerCam)
    }
}

   

