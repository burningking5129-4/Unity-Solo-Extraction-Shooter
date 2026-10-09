using System.Collections;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int hp = 100;
    public int maxHp = 100;

    public float speed = 5.0f;
    public float jumpHeight = 4.0f;
    public float sprintBoost = 2.0f;
    public float stam = 100f;
    public float stamCost = 10f;
    public float stamRegen = 5f;
    public float maxStam = 100f;
    public float jumpDetectDistance = 1f;
    public float interactDistance = 5f;
    public float trapDmgInterval = 1f;
    public float sprintCooldown = 5f;

    public bool toggleSprint = true;
    public bool isSprinting = false;
    public bool canSprint = true;
    public bool sprintLock = false;
    public bool regenStam = false;
    public bool onGround = true;

    public bool attacking = false;
    public bool trapDmg = false;

    Ray jumpRay;
    Ray interactRay;
    RaycastHit interactHit;
    Vector2 moveInput = Vector2.zero;

    public Weapon Weapon;

    public Camera playerCam;
    public Transform weaponSlot;
    PlayerInput input;
    Rigidbody rb;
    GameObject pickupObj;
    public GameManager gameManager;

    public Transform start;
    public Transform Player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        jumpRay = new Ray();
        playerCam = Camera.main;

        interactRay = new Ray();
        weaponSlot = playerCam.transform.GetChild(0);


    }

    private void FixedUpdate()
    {
        Quaternion playerRotation = Quaternion.identity;
        playerRotation.y = playerCam.transform.rotation.y;
        playerRotation.w = playerCam.transform.rotation.w;
        transform.rotation = playerRotation;
    }

    // Update is called once per frame
    void Update()
    {
        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;

        onGround = Physics.Raycast(jumpRay, jumpDetectDistance);

        interactRay.origin = playerCam.transform.position;
        interactRay.direction = playerCam.transform.forward;

        if (Physics.Raycast(interactRay, out interactHit, interactDistance))
        {
            if (interactHit.collider.tag == "Weapon")
            {
                pickupObj = interactHit.collider.gameObject;
            }
            else
                pickupObj = null;
        }
        else
            pickupObj = null;

        if (Weapon)
            if (Weapon.holdToAttack && attacking)
            {
                Weapon.fire();
            }



        Vector3 tempMove = rb.linearVelocity;

        tempMove.x = moveInput.x * speed;
        tempMove.z = moveInput.y * speed;

        if (isSprinting)
        {
            if (stam > 0)
            {
                tempMove.z *= sprintBoost;
                stam -= stamCost * Time.deltaTime;

                StopCoroutine("sprintReset");
                regenStam = false;

                if (stam <= 0)
                {
                    canSprint = false;
                    isSprinting = false;
                    stam = 0;
                }
            }
        }

        if (!isSprinting)
        {
            if (!canSprint && !sprintLock)
            {
                StartCoroutine("sprintReset");
            }

            if (canSprint && !regenStam)
            {
                regenStam = true;
            }

            if (regenStam)
            {
                stam += stamRegen * Time.deltaTime;

                if (stam >= maxStam)
                {
                    stam = maxStam;
                    regenStam = false;
                }
            }
        }

        rb.linearVelocity = (tempMove.x * transform.right) +
                                (tempMove.y * transform.up) +
                                (tempMove.z * transform.forward);

    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (canSprint && onGround)
        {
            if (!toggleSprint)
            {
                if (context.ReadValueAsButton())
                {
                    isSprinting = true;
                }
                else
                {
                    isSprinting = false;
                    canSprint = false;
                }
            }
        }
        else
        {
            if (context.performed)
            {
                /*
                if (isSprinting == true)
                {
                    isSprinting = false;
                }
                else
                {
                    isSprinting = true;
                }
                */
                isSprinting = !isSprinting;
            }
        }
    }

    public void Jump()
    {
        if (onGround)
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            if (pickupObj)
            {
                if (pickupObj.tag == "Weapon")
                {
                    pickupObj.GetComponent<Weapon>().equip(this);
                }

                pickupObj = null;
            }
        }
    }
    public void Reload()
    {
        if (Weapon)
            if (!Weapon.reloading)
                Weapon.reload();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (Weapon)
        {
            if (Weapon.holdToAttack)
            {
                if (context.ReadValueAsButton())
                    attacking = true;
                else

                    attacking = false;
            }

            else if (context.ReadValueAsButton())
                Weapon.fire();
        }
    }
    public void DropWeapon()
    {
        if (Weapon)
        {
            Weapon.GetComponent<Weapon>().unequip();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Trap")
        {
            hp -= 15;
        }
        if (collision.gameObject.tag == "Exit")
        {
            gameManager.LoadNextLevel();
        }
        if (collision.gameObject.tag == "Heals")
        {
            hp += 30;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "Ammo")
        {
            Weapon.ammo += 60;
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Trap")
        {
            if (!trapDmg)
            {
                StartCoroutine("trapBleed");
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Trap")
        {
            if (!trapDmg)
            {
                StopCoroutine("trapBleed");

            }
        }
    }
    IEnumerator trapBleed()
    {
        trapDmg = true;

        yield return new WaitForSeconds(trapDmgInterval);

        hp -= 15;
        trapDmg = false;
    }


    IEnumerator sprintReset()
    {
        sprintLock = true;
        regenStam = false;

        yield return new WaitForSeconds(sprintCooldown);

        canSprint = true;
        regenStam = true;
        sprintLock = false;
    }
}

