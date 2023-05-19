using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    //[SerializeField] private GameObject winPanel;


    [SerializeField] private Material[] materials;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float horizontalForce = 2f;

    private Vector3 scaleParameter = new Vector3(0.1f, 0.1f, 0.1f);

    private int health = 2;

    [SerializeField] Rigidbody rb;

    private void Start()
    {
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
}
