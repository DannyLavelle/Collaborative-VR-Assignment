using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class BulletSctipt : NetworkBehaviour
{
    public float timer;
    public float bulletLife = 5;
    GameObject Gun;
    public float maxSpeed = 20;
    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = gameObject.transform.forward * maxSpeed;

    }
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= bulletLife)
        {
            timer = 0;
            
           
       
            DespawnSelfServerRPC();
            Destroy(gameObject);
            //pool.ReturnItem(gameObject);

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Target")
        {
            Debug.Log("Hit");
            TargetScript targetScript = collision.gameObject.GetComponent<TargetScript>();

            Gun.GetComponent<PointScript>().IncrementPoints();

            targetScript.RemoveSelf();
        }
    }

    public void setGun(GameObject obj)
    {
        Gun = obj;
    }
    [ServerRpc(RequireOwnership = false)]
    
    void DespawnSelfServerRPC(ServerRpcParams serverRpcParams = default)
    {
        NetworkObject no = gameObject.GetComponent<NetworkObject>();
        no.Despawn();
    }

}
