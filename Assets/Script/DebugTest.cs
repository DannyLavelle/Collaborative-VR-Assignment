using UnityEngine;

public class DebugTest : MonoBehaviour
{

    public string Message;

   public void SendMessage()
    {
        Debug.Log(Message);
    }
}
