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
        targetLine = GameObject.Find("FinishLine").transform;
    }

    private void Update()
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

        if (isStopped)
        {
            return;
        }
    }

    private void Resize(Vector3 scale)
    {
        transform.localScale += scale;
    }
    //check if primaryBall's health value is equal to pointBall(pointBall)
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
    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu quả banh va chạm với một trong các cube
        if (other.CompareTag("Cube"))
        {
            CubeController cubeController = other.GetComponent<CubeController>();
            if (cubeController != null)
            {
                int pointBall = cubeController.GetPoint(); // Lấy điểm của cube

                if (TryMerge(pointBall))
                {
                    // Quả banh nảy dính vào giữa tâm bề mặt ô điểm cube
                    // Tăng tốc độ của quả banh
                    rb.velocity = Vector3.zero;

                    // Di chuyển quả banh đến giữa tâm bề mặt ô điểm cube
                    Vector3 newPosition = other.transform.position;
                    newPosition.y = transform.position.y;
                    transform.position = newPosition;
                }
                else
                {
                    // Quả banh không nảy dính, tiếp tục di chuyển tiếp theo
                    rb.velocity = Vector3.forward * speed;
                }
            }
        }
    }
}
