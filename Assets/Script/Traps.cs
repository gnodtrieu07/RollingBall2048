using UnityEngine;

public class Traps : MonoBehaviour
{
    [SerializeField] private Ball ballController;
    //time between subsequent attacks
    private float reloadTime = 1f;
    //counts the time elapsed since the last attack
    private float timeLeft = 0f;
    private const int damage = 2;

    private void Update()
    {
        timeLeft += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Parent"))
        {
            //check if there is enough time between attacks
            if (timeLeft >= reloadTime)
            {
                //call the BallController DamageTraps method and pass in the damage -> make dame & -health
                ballController.DamageTraps(damage);
                timeLeft = 0f;
            }
        }
    }
}
