using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    PlayerController player;

    [Header("Object References")]
    public GameObject projectile;
    public Transform firePoint;
    public Camera FiringDirection;

    [Header("Meta Attributes")]
    public bool canFire = true;
    public bool holdToAttack = true;
    public bool reloading = false;
    public int weaponID;
    public string weaponName;

    [Header("Weapon Stats")]
    public float projLifespan;
    public float projVelocity;
    public float reloadCooldown;
    public float rof;
    public int fireModes;
    public int currentFireMode;
    public int mag;
    public int magSize;

    [Header("Ammo Stats")]
    public int ammo;
    public int maxAmmo;
    public int ammoRefill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firePoint = transform.GetChild(0);
        FiringDirection = Camera.main;
    }

    public void equip(PlayerController p)
    {
        player = p;

        player.currentWeapon = this;
        
        transform.SetPositionAndRotation(player.weaponSlot.position, player.weaponSlot.rotation);
        transform.SetParent(player.weaponSlot);

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
    }

    public void unequip()
    {
        player.currentWeapon = null;

        transform.SetParent(null);

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().isTrigger = false;

        player = null;
    }

    public void reload()
    {
        if (mag >= magSize)
            return;

        int reloadCount = magSize - mag;

        if (ammo < reloadCount)
        {
            mag += ammo;
            ammo = 0;
        }
        else
        {
            mag += reloadCount;
            ammo -= reloadCount;
        }

        reloading = true;
        canFire = false;
        StartCoroutine("reloadingCooldown");
    }

    public void fire()
    {
        if (mag > 0 && canFire && !reloading)
        {
            mag--;

            GameObject p = Instantiate(projectile, firePoint.position, firePoint.rotation);
            p.GetComponent<Rigidbody>().AddForce(FiringDirection.transform.forward * projVelocity);
            Destroy(p, projLifespan);
            canFire = false;
            StartCoroutine("cooldownFire");

        }
    }
    
    IEnumerator cooldownFire()
    {
        yield return new WaitForSeconds(rof);

        if(mag > 0)
        {
            canFire = true;
        }
    }

    IEnumerator reloadingCooldown()
    {
        yield return new WaitForSeconds(reloadCooldown);

        reloading = false;
        canFire = true;
    }
    
}
