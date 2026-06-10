using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private Transform centerPoint;

    public Transform CenterPoint => centerPoint;
}
