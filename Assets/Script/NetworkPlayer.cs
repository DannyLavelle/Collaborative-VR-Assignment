using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public Renderer[] meshToDisable;
    


    public override void OnNetworkSpawn()
    {
        Debug.Log($"Spawned Network Vr rig: {VRRigReferences.singleton}");

        base.OnNetworkSpawn();
        if (IsOwner)
        {

            foreach (var item in meshToDisable)
            {
                item.enabled = false;
            }

            
        }
    }

    void Start()
    {
        if (root == null) Debug.LogError("root is not assigned");
        if (head == null) Debug.LogError("head is not assigned");
        //if (leftHand == null) Debug.LogError("leftHand is not assigned");
        if (rightHand == null) Debug.LogError("rightHand is not assigned");
    }


    // Update is called once per frame
    void Update()
    {
        if (VRRigReferences.singleton == null)
        {
            Debug.LogError("VRRigReferences.singleton is null");
        }

            if (IsOwner)
        {
            root.position = VRRigReferences.singleton.root.position;
            head.position = VRRigReferences.singleton.head.position;
            //leftHand.position = VRRigReferences.singleton.leftHand.position;
            rightHand.position = VRRigReferences.singleton.rightHand.position;


            root.rotation = VRRigReferences.singleton.root.rotation;
            head.rotation = VRRigReferences.singleton.head.rotation;
            //leftHand.rotation = VRRigReferences.singleton.leftHand.rotation;
            rightHand.rotation = VRRigReferences.singleton.rightHand.rotation;
        }

    }
}
