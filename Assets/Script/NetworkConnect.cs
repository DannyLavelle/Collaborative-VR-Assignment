using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;

public class NetworkConnect : MonoBehaviour
{

    public int maxConnection = 20;
    public UnityTransport transport;
    //public string joinCode;
    private Lobby currentLobby;
   
    public float heartBeatTimer;


    private float reconnectionTimer;
    private const float reconnectionInterval = 10f; //In Seconds

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        JoinOrCreate();
    }

    public async void JoinOrCreate()
    {
        try
        {
            currentLobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = currentLobby.Data["Join"].Value;

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

            transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
              allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

            NetworkManager.Singleton.StartClient();
        }
        catch
        {
            Create();
        }
        



    }



    public async void Create()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
        string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        //only player with join code can join
        Debug.LogError(newJoinCode);




        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);


        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
        lobbyOptions.IsPrivate = false;
        lobbyOptions.Data = new Dictionary<string,DataObject>();
        DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, newJoinCode);
        lobbyOptions.Data.Add("Join",dataObject);

        currentLobby = await Lobbies.Instance.CreateLobbyAsync("Lobby Name", maxConnection,lobbyOptions);

        NetworkManager.Singleton.StartHost();

    }
    public async void Join()
    {

        currentLobby = await Lobbies.Instance.QuickJoinLobbyAsync();
      
        string relayJoinCode = currentLobby.Data["Join"].Value;

        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
          allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData,allocation.HostConnectionData);

        NetworkManager.Singleton.StartClient();

    }


    private void Update()
    {
        if(heartBeatTimer > 15)
        {
            heartBeatTimer -= 15;
            if (currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
        }

        heartBeatTimer += Time.deltaTime;

        // Check for reconnection
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
        {
            reconnectionTimer += Time.deltaTime;
            if (reconnectionTimer >= reconnectionInterval)
            {
                reconnectionTimer = 0f;

                Debug.Log("Not connected to any server as a client.");
                //JoinOrCreate();
            }
        }
        else
        {
            reconnectionTimer = 0f; // Reset the timer if connected
        }




    }
}
