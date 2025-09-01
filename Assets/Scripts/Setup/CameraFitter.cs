using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFitter : MonoBehaviour
{
    public Vector2Int gridSize = new(12, 8);
    public float cell = 1f;
    public float topMargin = 1.2f;   // отступ под HUD
    public float sideMargin = 1.0f;

    void Start()
    {
        var cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

        float w = gridSize.x * cell + sideMargin * 2f;
        float h = gridSize.y * cell + topMargin  * 2f;

        float sizeByH = h * 0.5f;
        float sizeByW = (w * 0.5f) / cam.aspect;
        cam.orthographicSize = Mathf.Max(sizeByH, sizeByW);

        // позиционируем так, чтобы сетка была чуть ниже верхнего края
        var center = new Vector3(gridSize.x * 0.5f, gridSize.y * 0.5f - topMargin * 0.5f, -10f);
        transform.position = center;
    }
}
