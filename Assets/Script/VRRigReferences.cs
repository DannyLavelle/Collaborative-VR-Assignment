using UnityEngine;

public class VRRigReferences : MonoBehaviour
{
    public static VRRigReferences singleton;

    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;


    private void Awake()
    {
        singleton = this;
    }


}
