using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField] private int point; // Điểm của cube

    public int GetPoint()
    {
        return point;
    }
}