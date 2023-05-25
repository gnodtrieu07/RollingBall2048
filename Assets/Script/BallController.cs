using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    //[SerializeField] private GameObject winPanel;
    [SerializeField] private Material[] materials;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float horizontalForce = 2f;
    //[SerializeField] private GameObject targetLinePrefab;
    [SerializeField] private Transform targetLine;
    [SerializeField] private GameObject[] cubes;

    private Vector3 scaleParameter = new Vector3(0.1f, 0.1f, 0.1f);

    private bool isBoosting;
    private GameObject targetCube;

    private int health = 2;
    //private int currentScore = 2;
    private bool isJumping;
    private bool isStopped;


    [SerializeField] Rigidbody rb;

    private void Start()
    {
        isJumping = false;
        isStopped = false;
        rb.velocity = Vector3.forward * speed;
        //targetLine = GameObject.Find("FinishLine").transform;

        isBoosting = false;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, speed);
        //Set initial horizontal velocity to 0
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.x = 0f;
        rb.velocity = horizontalVelocity;

        //Locate the screen touch position
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                //Calculation of horizontal thrust based on touch position
                float force = touch.deltaPosition.x * horizontalForce;

                //Apply horizontal thrust to the ball
                rb.AddForce(new Vector3(force, 0f, 0f));  
            }
        }

        UpdateScoreText();

        if (!isBoosting && transform.position.z >= 100f)
        {
            isBoosting = true;
            Boost();
        }

        if (isJumping || isStopped)
        {
            return;
        }
    }

    private void Resize(Vector3 scale)
    {
        transform.localScale += scale;
    }


    //check if primaryBall's health value is equal to pointBall
    public bool TryMerge(int pointBall)
    {
        if (health == pointBall)
        {
            //if true, increase health value by adding pointBall
            health += pointBall;
            //then call the SetMaterial method to update the material based on the new health value
            SetMaterial();
            //change position of ParentBall by calling Resize method with scaleParameter
            Resize(scaleParameter);

            return true;
        }
        //if health is not equal to pointBall, the method will return false and not collision.
        return false;
    }

    private void UpdateScoreText()
    {
        scoreText.text = health.ToString();
    }

    public void DamageTraps(int damage) {
        //if damage is 64, health will be halved.
        health /= damage;
        //update the material of the BC based on the current health value after reduce
        SetMaterial();
        //decrease the size base on health
        Resize(-scaleParameter);

        if (health >= 1)
        {
            // Instantiate a new ball from the Prefab
            GameObject newBall = Instantiate(gameObject, transform.position, Quaternion.identity);

            // Get the BallController component of the new ball
            BallController newBallController = newBall.GetComponent<BallController>();

            // Set the health of the new ball to the remaining health
            newBallController.health = health;

            // Call SetMaterial on the new ball to update its material
            newBallController.SetMaterial();

            // Reduce the size of the new ball based on its health
            newBallController.Resize(-newBallController.scaleParameter);

            // Remove the Rigidbody component from the new ball to make it fall freely
            Destroy(newBall.GetComponent<Rigidbody>());

            // Remove the BallController component from the new ball to stop it from following the original ball
            Destroy(newBall.GetComponent<BallController>());
        }
        else
        {
            // If health is less than 1, destroy the ball
            Destroy(gameObject);
        }
    }

    private void SetMaterial()
    {
        //using Log to make the value of health is a power of 2
        //RoundToInt convert float logarit to ~= int
        int materialIndex = Mathf.RoundToInt(Mathf.Log(health, 2)) - 1; //-1 because the index in the materials array starts at 0
        //                                                              // decrease 1 allow we use value in logarit as a array index
        
        //If the materialIndex exceeds the size of the array so there is no material corresponding to health.
        if (materialIndex >= 0 && materialIndex < materials.Length)
        {
            //get the material corresponding to the materialIndex from the materials array and set the Renderer of the current object.
            GetComponent<Renderer>().material = materials[materialIndex];
        }
        else
        {
            Debug.LogError("Error: Incorrect ParentBall health");
        }
    }

    private void Boost()
    {
        // Increase the velocity of the Rigidbody along the z-axis by a factor of 1.5
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, speed * 1.5f);

        // Find a cube with the same color as the current object's Renderer color
        this.targetCube = FindCubeByColor(GetComponent<Renderer>().material.color);

        // If a target cube is found
        if (this.targetCube != null)
        {
            // Set the jump force value
            float jumpForce = 20f;

            // Apply an impulse force to the Rigidbody for jumping
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);

            // Start a coroutine to move the object to the target position
            StartCoroutine(MoveToTarget(targetCube.transform.position, this.targetCube));
        }
    }


    // Coroutine to move the object to the target position
    private IEnumerator MoveToTarget(Vector3 targetPosition, GameObject targetCube)
    {
        // Set the duration of the movement
        float moveDuration = 4f;

        // Track the elapsed time during the movement
        float elapsedTime = 0f;

        // Store the initial position of the object
        Vector3 startPosition = transform.position;

        // While the elapsed time is less than the move duration
        while (elapsedTime < moveDuration)
        {
            // Increase the elapsed time by the fixed delta time
            elapsedTime += Time.fixedDeltaTime;

            // Calculate the normalized progress of the movement
            float t = elapsedTime / moveDuration;

            // Move the object towards the target position using Rigidbody.MovePosition
            rb.MovePosition(Vector3.Lerp(startPosition, new Vector3(targetPosition.x, transform.position.y, targetPosition.z), t));

            // Wait for the next frame
            yield return null;
        }

        // Check if the color of the current object matches the color of the target cube
        if (GetComponent<Renderer>().material.color == targetCube.GetComponent<Renderer>().material.color)
        {
            // Call the end game function
            EndGame();
        }
        else
        {
            // Set the jumping flag to false
            isJumping = false;
        }
    }


    // Find a cube with a specific color
    private GameObject FindCubeByColor(Color color)
    {
        // Iterate through the array of cubes
        for (int i = 0; i < cubes.Length; i++)
        {
            // Get the Renderer component of the current cube
            Renderer cubeRenderer = cubes[i].GetComponent<Renderer>();

            // Check if the color of the cube matches the specified color
            if (cubeRenderer.material.color.Equals(color))
            {
                // Log a debug message indicating the cube was found
                Debug.Log("Found");

                // Return the cube if a match is found
                return cubes[i];
            }
        }

        // Return null if no cube with a matching color is found
        return null;
    }

    private void EndGame()
    {
        Time.timeScale = 0f;
    }
}
