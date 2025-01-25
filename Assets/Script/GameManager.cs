using UnityEngine;
using Unity.Netcode;
using XRMultiplayer;
using UnityEngine.UIElements;
using System.Collections.Generic;

using UnityEngine.SocialPlatforms.Impl;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
 
    public Pooler pool;
    GameObject bulletSpawned;
    private float timer = 0;
    public float spawnInterval = 10;
    Pooler ammoPool;
    GameObject bullet;

    public Dictionary<ulong, int> playerScores = new Dictionary<ulong, int>();

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
        if (!playerScores.ContainsKey(clientId))
        {
            playerScores[clientId] = 0; // Initialize score if not already present
        }

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




    public void UpdateScores(ulong clientId, int points)
    {
        if (!playerScores.ContainsKey(clientId))
        {
            playerScores[clientId] = 0; // Initialize score if not already present
        }


        playerScores[clientId] = points;

        foreach(var scores in playerScores)
        {
            Debug.Log($"Player {scores.Key} Score: {scores.Value}");
            if(scores.Value >= 10)
            {
                Debug.Log($"Player {scores.Key} Wins");
            }

        }


        UpdateOverhead(points, clientId);
    }


    void UpdateOverhead(int score, ulong clientId)
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

       if (score >= 10)
        {
            UpdateOverheadClientRpc("Winner",clientRpcParams);
        }
        else
        {
            UpdateOverheadClientRpc(score.ToString(),clientRpcParams);
        }
    }

    [ClientRpc]
    void UpdateOverheadClientRpc(string score,ClientRpcParams clientRpcParams = default)
    {

       
        //VRRigReferences.singleton.SetText(score);
    }



}
