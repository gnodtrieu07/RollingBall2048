using UnityEngine;

public class CubeCheck : MonoBehaviour
{
    //Point of cube
    [SerializeField] private int point; 

    public int GetPoint()
    {
        return point;
    }
}