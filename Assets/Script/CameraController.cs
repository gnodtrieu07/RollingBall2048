using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Reference to Transform of the Sphere
    [SerializeField] private Transform sphereTransform;
    //Distance between camera and Sphere
    [SerializeField] private float distance = 10f;
    //Deviation between camera position and Sphere
    private Vector3 offset; 

    private void Start()
    {
        //Calculate initial offset between camera and Sphere
        offset = transform.position - sphereTransform.position;
    }

    private void Update()
    {
        //Check if Sphere exists
        if (sphereTransform != null)
        {
            //Update camera position based on Sphere's current position and distance
            Vector3 targetPosition = sphereTransform.position + offset.normalized * distance;
            //Fixed x-axis of camera
            targetPosition.x = transform.position.x; 
            transform.position = targetPosition;
        }
    }
}
