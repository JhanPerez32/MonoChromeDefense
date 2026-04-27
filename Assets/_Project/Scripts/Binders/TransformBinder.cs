using UnityEngine;

public class TransformBinder : MonoBehaviour
{
    [SerializeField] private TransformValue _transformValue;

    private void Awake()
    {
        _transformValue.Value = transform;
    }
}
