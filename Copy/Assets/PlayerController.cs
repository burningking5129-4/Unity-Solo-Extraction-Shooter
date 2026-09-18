using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpHeight = 4.0f;
    public float jumpDetectDistance = 1f;
    public float interactDistance = 5f;

    public bool attacking = false;

    Ray jumpRay;
    Ray interactRay;
    RaycastHit interactHit;
    Vector2 moveInput = Vector2.zero;

    public Weapon currentWeapon;

    Camera playerCam;
    public Transform weaponSlot;
    PlayerInput input;
    Rigidbody rb;
    GameObject pickupObj;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        jumpRay = new Ray();
        playerCam = Camera.main;

        interactRay = new Ray();
        weaponSlot = playerCam.transform.GetChild(0);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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

        interactRay.origin = playerCam.transform.position;
        interactRay.direction = playerCam.transform.forward;

        if (Physics.Raycast(interactRay, out interactHit, interactDistance))
        {
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

            if (currentWeapon)
                if(currentWeapon.holdToAttack && attacking)
            {
                currentWeapon.fire();
            }

            Vector3 tempMove = rb.linearVelocity;

            tempMove.x = moveInput.x * speed;
            tempMove.z = moveInput.y * speed;

            rb.linearVelocity = (tempMove.x * transform.right) +
                                (tempMove.y * transform.up) +
                                (tempMove.z * transform.forward);

        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }


    public void Jump()
    {
        if (Physics.Raycast(jumpRay, jumpDetectDistance))
        rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            if(pickupObj)
            {
                if(pickupObj.tag == "Weapon")
                {
                    pickupObj.GetComponent<Weapon>().equip(this);
                }

                pickupObj = null;
            }
        }
    }
    public void Reload()
    {
        if (currentWeapon)
            if (!currentWeapon.reloading)
                currentWeapon.reload();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (currentWeapon)
        {
            if (currentWeapon.holdToAttack)
            {
                if (context.ReadValueAsButton())
                    attacking = true;
                else
                
                    attacking = false;
                }




                else if (context.ReadValueAsButton())
                    currentWeapon.fire();
                

            }
        }
    }

