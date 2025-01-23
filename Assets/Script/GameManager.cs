using UnityEngine;
using Unity.Netcode;
using XRMultiplayer;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
 
    public Pooler pool;

    private float timer = 0;
    public float spawnInterval = 10;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pool = gameObject.GetComponent<Pooler>();

        // Ensure only Host initializes spawn logic
        if (IsHost)
        {
            // Subscribe to player connection events
            NetworkManager.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void Update()
    {
        // Ensure only the Host handles the spawning timer
        if (!IsHost) return;

        // Only proceed if at least one other player has joined
        if (NetworkManager.Singleton.ConnectedClients.Count > 1)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0;
                SpawnTarget();
                Debug.Log("Spawning Item");
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // If a new player connects, spawn the first target
        if (NetworkManager.Singleton.ConnectedClients.Count > 1)
        {
            Debug.Log("Another player has joined, spawning the first target.");
            SpawnTarget();
        }
    }

    public override void OnDestroy()
    {
        // Unsubscribe to avoid potential memory leaks
        if (IsHost)
        {
            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    [ClientRpc]
    public void DestroyTargetClientRpc()
    {
        // Destroy the target locally
        GameObject target = GameObject.FindWithTag("Target");
        if (target != null)
        {
            Destroy(target);
        }

        // Spawn a new target
        SpawnTarget();
    }

    private void SpawnTarget()
    {
        if (!IsHost) return; // Ensure only Host can spawn targets
        GameObject obj = pool.GetItem();
        NetworkObject networkObject = obj.GetComponent<NetworkObject>();
        networkObject.Spawn();
    }

    public void ReturnToPool(GameObject obj)
    {
       if(IsHost)
        {
            var networkObject = obj.GetComponent<NetworkObject>();
            if (networkObject != null && networkObject.IsSpawned)
            {
                networkObject.Despawn(false); // NOTE TO SELF PUT FALSE HERE SO OBJECT DOESN'T DESPAWN THIS ERROR TOOK ME 2 HOURS TO FIX!!!
                //obj.SetActive(true);
            }
        }

        pool.ReturnItem(obj); // Return to the pool
    }

    public void ReturnBullet(GameObject obj, Pooler pooler)
    {
        if (IsOwner)
        {
            var networkObject = obj.GetComponent<NetworkObject>();
            if (networkObject != null && networkObject.IsSpawned)
            {
                networkObject.Despawn(false);

            }
        }
        Debug.Log("Despawn bullet");
        pooler.ReturnItem(obj);
    }


}
