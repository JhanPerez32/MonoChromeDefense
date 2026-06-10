using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Events")]
    [SerializeField]
    private BuildPlatformClickedEvent buildPlatformClickedEvent;

    [Header("Raycast")]
    [SerializeField]
    private LayerMask buildPlatformLayer;

    [SerializeField]
    private Camera targetCamera;

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, buildPlatformLayer)) return;

        BuildPlatform platform = hit.collider.GetComponent<BuildPlatform>();

        if (!platform) return;

        buildPlatformClickedEvent.Raise(platform);
    }
}
