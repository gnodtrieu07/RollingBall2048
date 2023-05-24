using System;
using UnityEngine;
using UnityEngine.UI;

public class PointBall : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private BallController ballController;
    [SerializeField] int health;
    private bool isMerged;

    private void Update()
    {
        UpdateScoreText();
    }

    private void OnValidate()
    {
        ValidateHealth();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Parent"))
        {
            TryMergeWithParent();
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = health.ToString();
    }

    //check the validity of the health value.
    private void ValidateHealth()
    {
        //checks if the value health is in the list of valid numbers (2, 4, 8, ...) or not.
        int[] validNumbers = { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 };

        //Check if health is valid by calling IsValidHealth() method. If invalid, it sets health to 0
        if (!IsValidHealth())
        {
            health = 0;
            Debug.LogError("Error");
        }

        //check if the value health is in the validNumbers array.
        bool IsValidHealth()
        {
            //Find value health in validNumbers
            //If the value health is found in the array, the method returns the index of that value.
            //If not found, the method will return -1.
            return Array.IndexOf(validNumbers, health) != -1; //!= -1 is used to check if the value health exists in the array.
        }
    }

    private void TryMergeWithParent()
    {
        isMerged = ballController.TryMerge(health); //pass in the current health value of the ball.
                                                    //try merging the current ball with the parent ball
                                                    //finally the result of the merge is assigned to the isMerged.
        if (isMerged)
        {
            //destroy because the ball has been merged and no longer exists.
            Destroy(gameObject);
            //after destroy & merge, the isMerged variable will be reset in preparation for the next merge if necessary
            isMerged = false;
        }
    }
}