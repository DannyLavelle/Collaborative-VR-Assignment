using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class BulletSctipt : NetworkBehaviour
{
    public float timer;
    public float bulletLife = 5;
    GameObject Gun;
    private void Start()
    {
        
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= bulletLife)
        {
            timer = 0;
            Rigidbody rb = GetComponent<Rigidbody>();
            GameObject obj = GameObject.FindWithTag("AmmoPool");
            Pooler pool = obj.GetComponent<Pooler>();
            DespawnSelfServerRPC();
            pool.ReturnItem(gameObject);

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Target")
        {
            Debug.LogError("Hit");
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
        no.Despawn(false);
    }

}
