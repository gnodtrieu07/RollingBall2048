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
    //[SerializeField] private Renderer[] cubesss;

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

        isBoosting = false; // Khởi tạo biến isBoosting là false
        rb.velocity = Vector3.forward * speed;
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

        if (!isBoosting && transform.position.z >= 100f) // Thay 50f bằng vị trí Z của băng rôn
        {
            isBoosting = true;
            Boost(); // Gọi phương thức Boost để tăng tốc chạy
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

    private void Boost()
    {
        // Tăng tốc chạy của quả banh
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, speed * 2f); // Tăng gấp đôi tốc độ chạy

        // Tìm ô màu tương ứng và nhảy đến
        this.targetCube = FindCubeByColor(GetComponent<Renderer>().material.color); // Gọi phương thức FindCubeByColor để tìm ô màu tương ứng
        if (this.targetCube != null)
        {
            Vector3 targetPosition = this.targetCube.transform.position;
            float jumpForce = 500f; // Điều chỉnh lực nhảy tùy theo yêu cầu
            rb.AddForce(new Vector3(0f, jumpForce, 0f)); // Áp dụng lực nhảy lên quả banh
            StartCoroutine(MoveToTarget(targetPosition)); // Gọi Coroutine để di chuyển quả banh đến vị trí của ô màu tương ứng
        }
    }

    private IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        // Di chuyển quả banh đến vị trí của ô màu tương ứng
        float moveDuration = 2f; // Thời gian di chuyển
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(startPosition, new Vector3(targetPosition.x, transform.position.y,targetPosition.z ), t);
            yield return null;
        }

        // Dừng lại và kết thúc game nếu quả banh trùng màu
        if (GetComponent<Renderer>().material.color == targetCube.GetComponent<Renderer>().material.color)
        {
            EndGame(); // Gọi phương thức để kết thúc game
        }
        else
        {
            // Tiếp tục chạy nếu không trùng màu
            isJumping = false;
        }
    }

    private GameObject FindCubeByColor(Color color)
    {
        // Tìm ô màu tương ứng với quả banh
        foreach (GameObject cube in cubes)
        {
            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            if (cubeRenderer.material.color == color)
            {
                return cube;
            }
        }
        return null;
    }

    private void EndGame()
    {
        Time.timeScale = 0f;
    }
}
