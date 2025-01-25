using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;

using UnityEngine;
using UnityEngine.InputSystem;
using XRMultiplayer;

public class GunScript : NetworkBehaviour
{
    public List<GameObject> AmmoIndicators;
    public int maxAmmo = 6;
    public int currentAmmo;
    public InputActionProperty shoot;
    public Transform firePoint;
    public GameObject ammoPool;
   
    public float bulletSpeed = 20f;
    public float timer;
    public float shotCD = 1;
    private bool hasShot;


    public float reloadTimer;
    public float timeToReload;


    public GameObject bullet;
    
    void Start()
    {
        currentAmmo = maxAmmo;
    }
 

    private void Update()
    {

        if (currentAmmo == 0)
        {
           reloadTimer += Time.deltaTime;
        }

        if(reloadTimer >= timeToReload)
        {
            foreach (GameObject go in AmmoIndicators)
            {
                Renderer renderer = go.GetComponent<Renderer>();
                renderer.material.color = Color.green;
            }
            reloadTimer = 0;
            currentAmmo = maxAmmo;
        }


        if (hasShot)
        {
            timer += Time.deltaTime;
        }

        if (timer >= shotCD)
        {
            timer = 0;
            hasShot = false;
        }

        if (shoot.action.IsPressed() && !hasShot && currentAmmo != 0)
        {
            Shoot();
           DecrementAmmo();
          
            hasShot = true;
        }
    }

    public void DecrementAmmo()
    {
    

        Renderer renderer = AmmoIndicators[currentAmmo - 1].GetComponent<Renderer>();
        
        renderer.material.color = Color.red;
       
        currentAmmo--;
    }

    public void Shoot()
    {
        Vector3 clientPosition = firePoint.transform.position;
        Quaternion clientRotation = firePoint.transform.rotation;
        Debug.Log($"Client shooting from position: {clientPosition}, rotation: {clientRotation}");
        SpawnObjectServerRpc(clientPosition, clientRotation);
    }

    public void ReturnBullet(GameObject obj, Pooler pool)
    {
        GameManager.Instance.ReturnBullet(obj, pool);
    }

   

    [ServerRpc(RequireOwnership = false)]
    private void SpawnObjectServerRpc(Vector3 clientPosition, Quaternion clientRotation, ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer)
        {
            Debug.LogError("This method should only be executed on the server.");
            return;
        }

        Debug.Log($"Server received spawn request at position: {clientPosition}, rotation: {clientRotation}");
        GameObject spawnBullet = Instantiate(bullet, clientPosition, clientRotation);

        if (spawnBullet == null)
        {
            Debug.LogError("Failed to instantiate the bullet.");
            return;
        }

        NetworkObject no = spawnBullet.GetComponent<NetworkObject>();
        no.Spawn(); 

        BulletSctipt bs = spawnBullet.GetComponent<BulletSctipt>();
        bs.SetOwner(serverRpcParams.Receive.SenderClientId);

        Debug.Log($"Bullet spawned at {clientPosition} with rotation {clientRotation}");
    }





}