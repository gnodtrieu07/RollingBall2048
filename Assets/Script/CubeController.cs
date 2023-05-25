using UnityEngine;

public class CubeController : MonoBehaviour
{
    //Point of cube
    [SerializeField] private int point; 

    public int GetPoint()
    {
        return point;
    }
}