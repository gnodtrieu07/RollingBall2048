using System;
using UnityEngine;
using UnityEngine.UI;

public class PointBall : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Ball ballController;
    [SerializeField] int health;
    private bool isMerged;

    private void Update()
    {
        UpdateScoreText();
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