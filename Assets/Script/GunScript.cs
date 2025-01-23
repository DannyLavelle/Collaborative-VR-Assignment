using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using XRMultiplayer;

public class GunScript : MonoBehaviour
{
    public List<GameObject> AmmoIndicators;
    private int maxAmmo, currentAmmo;
    public InputActionProperty shoot;
    public Transform Barrel;
    public GameObject ammoPool;
    Pooler pool;
    public float bulletSpeed = 20f;
    void Start()
    {
       pool = ammoPool.GetComponent<Pooler>();
    }

    private void Update()
    {

        
        if(shoot.action.IsPressed())
        {
            Debug.Log("Shoot");
        }
    }

    public void Shoot()
    {

        //if (!IsOwner) return; 
        GameObject bullet = pool.GetItem();
        NetworkObject networkObject = bullet.GetComponent<NetworkObject>();
        networkObject.Spawn();
        
        bullet.transform.position = Barrel.position;
        bullet.transform.rotation = Barrel.rotation;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = Barrel.forward * bulletSpeed;

        BulletSctipt bs = bullet.GetComponent<BulletSctipt>();

        bs.setGun(gameObject);
    }

    public void ReturnBullet(GameObject obj,Pooler pool )
    {
        GameManager.Instance.ReturnBullet(obj, pool);
    }



}
