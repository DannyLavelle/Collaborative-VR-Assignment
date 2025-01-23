using UnityEngine;

public class BulletSctipt : MonoBehaviour
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
            rb.linearVelocity = Vector3.zero;

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
}
