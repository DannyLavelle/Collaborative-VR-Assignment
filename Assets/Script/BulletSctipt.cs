using Unity.Netcode;

using UnityEngine;
using XRMultiplayer;

public class BulletSctipt : NetworkBehaviour
{
    public float timer;
    public float bulletLife = 5;
    GameObject Gun;
    public float maxSpeed = 20;

    private ulong ownerClientId;
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
            //Destroy(gameObject);
            //pool.ReturnItem(gameObject);

        }
    }

    public void SetOwner(ulong clientId)
    {
        ownerClientId = clientId;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Target")
        {
            Debug.Log("Hit");
            TargetScript targetScript = collision.gameObject.GetComponent<TargetScript>();


            addPoint(ownerClientId);

            targetScript.RemoveSelf();
        }
    }

    public void setGun(GameObject obj)
    {
        Gun = obj;
    }

    public GameObject FindGun()
    {
        return GameObject.FindGameObjectWithTag("Gun");
    }
    [ServerRpc(RequireOwnership = false)]
    
    void DespawnSelfServerRPC(ServerRpcParams serverRpcParams = default)
    {
        NetworkObject no = gameObject.GetComponent<NetworkObject>();
        no.Despawn();
    }


    public void addPoint(ulong clientId)
    {
        if (!IsHost)
        {
            return;
        }
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        addpointClientRPC( clientId,clientRpcParams);

    }

    [ClientRpc]
    public void addpointClientRPC(ulong clientId,ClientRpcParams clientRpcParams = default)
    {

        int score = GameObject.FindGameObjectWithTag("Gun").GetComponent<PointScript>().IncrementPoints();
        UpdateScoringServerRPC(clientId, score);

    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateScoringServerRPC(ulong clientId, int score, ServerRpcParams serverRpcParams = default)
    {
        GameManager.Instance.UpdateScores(clientId, score);
    }



}
