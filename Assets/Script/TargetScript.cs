using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TargetScript : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The maximum height, expressed as a multiplier of the character's height.")]
    public int MaxHeightMultiplier = 2;
    public bool showDebugMessages = true;
    public int numberOfModes = 4;
    public float lifespan;
    GameObject playArea;
    GameObject player;
    Renderer playRenderer;
    public int Mode;
    List<Vector3> points;
    bool isMoving = false;
    public float moveSpeed = 10;
    float objectHeight;
    float characterHeight;
    float minX, minY, maxX, maxY,minZ,maxZ;
    private float timer;
    Vector3 areaSize, areaCentre;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        

        DebugMessage("Start function called");

        playArea = FindObjectByTag("Play Area");
        player = FindObjectByTag("Player");

        playRenderer = playArea.GetComponent<Renderer>();


        characterHeight = GetColliderHeight(player);
        objectHeight =GetColliderHeight(gameObject);


        areaSize = playRenderer.bounds.size;
        areaCentre = playRenderer.transform.position;

        CalcuateSpawnRange();


        HandleSpawning();
    }

  

    public void HandleSpawning()
    {

        Mode = Random.Range(1, numberOfModes + 1);
        DebugMessage($"Mode Chosen: {Mode}");
        switch (Mode)
        {
            case 1://default
                DisableRigidBody();
            SetRandomPosition();

            break;

            case 2://Phsyics drop
            SetRandomPosition(1f, 1.5f);//Gives a Y axis boost to allow target room to drop 
            EnableRigidBody();


            break;

            case 3://Random Scale
            DisableRigidBody();
            SetRandomPosition();
            RandomizeScale();
            break;

            case 4: //Moving to points
            DisableRigidBody();
            SetRandomPosition();
            points = GenerateRandomPoints(5);
            isMoving = true;


            break;


            default:
            DebugMessage("Mode Selector didn't select a number between 1 and 3", "Warning");
            break;
        }

    }

    void Update()
    {
        if (isMoving)
        {
            MoveThroughPoints();
        }
        DecayTarget();

    }

    private void EnableRigidBody()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Enable physics interactions by disabling kinematic mode
            DebugMessage("Rigidbody enabled.");
            DebugMessage($"Mode: {Mode}, isKinematic: {rb.isKinematic}");

        }
        else
        {
            DebugMessage("No Rigidbody component found.", "Warning");
        }

    }

    private void DisableRigidBody()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Enable physics interactions by disabling kinematic mode
            DebugMessage("Rigidbody disabled.");
            DebugMessage($"Mode: {Mode}, isKinematic: {rb.isKinematic}");

        }
        else
        {
            DebugMessage("No Rigidbody component found.", "Warning");
        }
    }

    private void ToggleRigidBody()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = !rb.isKinematic; // Toggles phsyics interactions
            DebugMessage("Rigidbody Toggled.");
        }
        else
        {
            DebugMessage("No Rigidbody component found.", "Warning");
        }

    }
    private GameObject FindObjectByTag(string tag)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);
        if (obj == null)
        {
            DebugMessage($"No GameObject found with tag '{tag}'", "warning");
        }
        return obj;
    }


    private float GetColliderHeight(GameObject obj)
    {
        Collider collider = obj.GetComponent<Collider>();
        if (collider == null)
        {          
            DebugMessage($"Collider component is missing on {obj.name}.", "warning");
            return 0f; 
        }
        return collider.bounds.size.y; 
    }

    private float GetRendererHeight(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if(renderer == null)
        {           
            DebugMessage($"Renderer component is missing on {obj.name}.", "warning");
            return 0f;
        }
        return renderer.bounds.size.y;
    }

    private void CalcuateSpawnRange()
    {
        minX = areaCentre.x - areaSize.x / 2;
        maxX = areaCentre.x + areaSize.x / 2;

        minZ = areaCentre.z - areaSize.z / 2;
        maxZ = areaCentre.z + areaSize.z / 2;

        minY = areaCentre.y + objectHeight / 2;
        maxY = characterHeight * MaxHeightMultiplier;
    }
   
    public void SetRandomPosition()
    {
        Vector3 target = GeneratePoint();
        transform.position = target;        
        DebugMessage($"New position set: {transform.position}");
    }
    public void SetRandomPosition(float xmod = 1f,float ymod = 1f,float zmod = 1f)
    {
        
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        float randomZ = Random.Range(minZ, maxZ);

        randomX = ModPosition(randomX, xmod, maxX);
        randomY = ModPosition(randomY, ymod, maxY);
        randomZ = ModPosition(randomZ, zmod, maxY);


        transform.position = new Vector3(randomX, randomY, randomZ);

       
        DebugMessage($"New position set: {transform.position}");
    }
    public float ModPosition(float position,float modifier, float max)
    {
        position *= modifier;

        if (position > max)
        {
            position = max;
        }

        return position;
    }
    void DebugMessage(string message)
    {
        if(showDebugMessages)
        {
            Debug.Log(message);
        }
    }
    void DebugMessage(string message,string warningOrError)
    {
        warningOrError.ToLower();

        if (showDebugMessages && warningOrError == "warning")
        {
            Debug.LogWarning(message);
        }
        else if (showDebugMessages && warningOrError == "warning")
        {
            Debug.LogError(message);
        }
    }

    public void RandomizeScale(float minMultiplier = 0.5f, float maxMultiplier = 1.5f)
    {
        
        Vector3 currentScale = transform.localScale;

        
        float randomX = Random.Range(minMultiplier, maxMultiplier);
        float randomY = Random.Range(minMultiplier, maxMultiplier);
        float randomZ = Random.Range(minMultiplier, maxMultiplier);

       
        transform.localScale = new Vector3(currentScale.x * randomX, currentScale.y * randomY, currentScale.z * randomZ);

       
        DebugMessage($"New scale set: {transform.localScale}");
    }
    public void RandomizeScale(GameObject obj,float minMultiplier = 0.5f, float maxMultiplier = 1.5f)
    {

        Vector3 currentScale = obj.transform.localScale;


        float randomX = Random.Range(minMultiplier, maxMultiplier);
        float randomY = Random.Range(minMultiplier, maxMultiplier);
        float randomZ = Random.Range(minMultiplier, maxMultiplier);


        obj.transform.localScale = new Vector3(currentScale.x * randomX, currentScale.y * randomY, currentScale.z * randomZ);


        DebugMessage($"New scale set: {transform.localScale}");
    }

    public List<Vector3> GenerateRandomPoints(int numberOfPoints)
    {
        List<Vector3> randomPoints = new List<Vector3>();

        
        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector3 pointToAdd = GeneratePoint();
            randomPoints.Add(pointToAdd);
        }

       
        DebugMessage($"Generated {numberOfPoints} random points.");

        return randomPoints;
    }

    public Vector3 GeneratePoint()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        float randomZ = Random.Range(minZ, maxZ);

        return new Vector3(randomX, randomY, randomZ);
    }

    private void MoveThroughPoints()
    {
        if (points.Count > 0)
        {
            Vector3 target = points[0];
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                points.RemoveAt(0);
            }
        }
    }

    public void DecayTarget()
    {
        if(Mode == 4 && points.Count == 0)
        {
            RemoveSelf();
        }
        else
        {
            timer += Time.deltaTime;
        }
        if (timer >=lifespan )
        {
            timer = 0;
            RemoveSelf();
        }
    }

    public void RemoveSelf()
    {

        DisableRigidBody();
        isMoving = false;
        DebugMessage("Destroy Target");
        GameManager.Instance.ReturnToPool(gameObject);
    }




}
