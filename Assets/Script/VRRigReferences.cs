using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class VRRigReferences : MonoBehaviour
{
    public static VRRigReferences singleton;

    public Transform root;
    public Transform head;
    //public Transform leftHand;
    public Transform rightHand;
    public TMP_Text pointText;


    private void Awake()
    {
        singleton = this;
    }

    

}
