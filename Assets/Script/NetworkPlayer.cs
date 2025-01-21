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
        base.OnNetworkSpawn();
        if (IsOwner)
        {

            foreach (var item in meshToDisable)
            {
                item.enabled = false;
            }

            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if(IsOwner)
        {
            root.position = VRRigReferences.singleton.root.position;
            head.position = VRRigReferences.singleton.head.position;
            leftHand.position = VRRigReferences.singleton.leftHand.position;
            rightHand.position = VRRigReferences.singleton.rightHand.position;


            root.rotation = VRRigReferences.singleton.root.rotation;
            head.rotation = VRRigReferences.singleton.head.rotation;
            leftHand.rotation = VRRigReferences.singleton.leftHand.rotation;
            rightHand.rotation = VRRigReferences.singleton.rightHand.rotation;
        }

    }
}
