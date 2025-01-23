using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;

using UnityEngine;
using UnityEngine.InputSystem;
using XRMultiplayer;

public class GunScript : NetworkBehaviour
{
    public List<GameObject> AmmoIndicators;
    private int maxAmmo, currentAmmo;
    public InputActionProperty shoot;
    public Transform Barrel;
    public GameObject ammoPool;
    Pooler pool;
    public float bulletSpeed = 20f;
    public float timer;
    public float shotCD = 1;
    private bool hasShot;
    GameObject bullet;
    void Start()
    {
        pool = ammoPool.GetComponent<Pooler>();
        //NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

    }
    //private void OnConnectedToServer()
    //{
    //   if(!IsHost)
    //    {
    //        Debug.Log("Seitching");
    //        NetworkObject no = gameObject.GetComponent<NetworkObject>();
    //        no.ChangeOwnership(NetworkManager.Singleton.LocalClientId);
    //    }
    //}
    //private void OnClientConnected(ulong clientId)
    //{
    //    Debug.Log($"Client connected: {clientId}");

    //    if (!IsHost)
    //    {
    //        Debug.Log("Seitching");
    //        NetworkObject no = gameObject.GetComponent<NetworkObject>();
           
    //        Debug.Log($"Switched ownere now is owner: {IsOwner}");
    //        GameManager.Instance.Ownership(clientId,no);
    //    }
    //}

    private void Update()
    {
        if (hasShot)
        {
            timer += Time.deltaTime;
        }
        if (timer >= shotCD)
        {
            timer = 0;
            hasShot = false;
        }

        if (shoot.action.IsPressed() && !hasShot)
        {
            Shoot();
            Debug.Log("Shoot");
            hasShot = true;
        }
    }

    public void Shoot()
    {

        //if (!IsOwner)
        //{
        //    Debug.Log("Not Owner");
        //    return;
        //}


        SpawnObjectServerRpc(Barrel.transform.position,Barrel.transform.rotation);
        
        if (!IsHost)
        {
            
        }

        //bullet.transform.position = Barrel.position;
        //bullet.transform.rotation = Barrel.rotation;

        //Rigidbody rb = bullet.GetComponent<Rigidbody>();
        //rb.linearVelocity = Barrel.forward * bulletSpeed;

        //BulletSctipt bs = bullet.GetComponent<BulletSctipt>();

        //bs.setGun(gameObject);
    }

    public void ReturnBullet(GameObject obj, Pooler pool)
    {
        GameManager.Instance.ReturnBullet(obj, pool);
    }

    [ServerRpc (RequireOwnership = false)]
    private void SpawnObjectServerRpc(Vector3 position, Quaternion rotation, ServerRpcParams serverRpcParams = default)
    {
        // Ensure this method is only called on the server
        if (!IsServer)
        {
            Debug.LogError("This method should only be executed on the server.");
            return;
        }

        // Get a bullet from the pool
        bullet = pool.GetItem();

        if (bullet == null)
        {
            Debug.LogError("Failed to get a bullet from the pool.");
            return;
        }

        // Get the NetworkObject component
        NetworkObject no = bullet.GetComponent<NetworkObject>();

        // Spawn the bullet with ownership assigned to the client that called this method
        no.SpawnWithOwnership(OwnerClientId);

        // Set the position and rotation of the bullet
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        Debug.Log($"Barrel transform = {position}");
        Debug.Log($"Bullet rotation = {rotation}");

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = Barrel.forward * bulletSpeed;


        //Vector3 velocity = Barrel.forward * bulletSpeed;
        //SetBulletVelocityClientRpc(bullet.GetComponent<NetworkObject>(), velocity);

        // Optional: Additional initialization for the bullet
        BulletSctipt bs = bullet.GetComponent<BulletSctipt>();
        bs.setGun(gameObject);

        Debug.Log($"Bullet spawned at {position} with rotation {rotation}");
    }

   


}