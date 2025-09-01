using UnityEngine;

[ExecuteAlways] // чтобы перестраивать сетку и в Edit, и в Play
public class GridBackgroundLines : MonoBehaviour
{
    [Header("Grid")]
    public int width = 12;
    public int height = 8;
    public float cellSize = 1f;

    [Header("Appearance")]
    public float lineWidth = 0.025f;
    public Color lineColor = new Color(1f, 1f, 1f, 0.35f);
    public int sortingOrder = -100;

    [Header("Placement")]
    public bool autoCenterToCamera = true;
    [Tooltip("Отступ от верхнего края экрана (в мировых ед.), чтобы не залезать под HUD")]
    public float topMarginWorld = 1.2f;
    public Vector2 customOrigin = Vector2.zero; // если autoCenterToCamera = false

    Material _mat;
    Transform _root;

    void OnEnable() { EnsureMaterial(); Rebuild(); }
    void Start()     { EnsureMaterial(); Rebuild(); }
    void OnValidate(){ EnsureMaterial(); Rebuild(); }

    void EnsureMaterial()
    {
        if (_mat == null)
        {
            var shader = Shader.Find("Unlit/Color");
            _mat = new Material(shader);
            _mat.color = lineColor;
        }
        else
        {
            _mat.color = lineColor;
        }
    }

    public void Rebuild()
    {
        if (!isActiveAndEnabled) return;

        if (_root == null)
        {
            var go = GameObject.Find("GridLines");
            if (go == null)
            {
                go = new GameObject("GridLines");
                go.transform.SetParent(transform, false);
            }
            _root = go.transform;
        }

        // очистка
#if UNITY_EDITOR
        for (int i = _root.childCount - 1; i >= 0; i--)
            if (Application.isPlaying) Destroy(_root.GetChild(i).gameObject);
            else DestroyImmediate(_root.GetChild(i).gameObject);
#else
        for (int i = _root.childCount - 1; i >= 0; i--)
            Destroy(_root.GetChild(i).gameObject);
#endif

        // вычисляем origin (левый-низ сетки)
        Vector2 origin = customOrigin;
        if (autoCenterToCamera && Camera.main)
        {
            var cam = Camera.main;
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;
            Vector2 camCenter = cam.transform.position;

            float gridW = width * cellSize;
            float gridH = height * cellSize;

            // центр по X, а по Y — прижимаем верх сетки на topMarginWorld ниже верха экрана
            origin.x = camCenter.x - gridW * 0.5f;
            origin.y = (camCenter.y + halfH - topMarginWorld) - gridH;
        }

        // вертикальные
        for (int x = 0; x <= width; x++)
            MakeLine(
                new Vector3(origin.x + x * cellSize, origin.y, 0),
                new Vector3(origin.x + x * cellSize, origin.y + height * cellSize, 0)
            );

        // горизонтальные
        for (int y = 0; y <= height; y++)
            MakeLine(
                new Vector3(origin.x, origin.y + y * cellSize, 0),
                new Vector3(origin.x + width * cellSize, origin.y + y * cellSize, 0)
            );
    }

    void MakeLine(Vector3 a, Vector3 b)
    {
        var go = new GameObject("Line");
        go.transform.SetParent(_root, false);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
        lr.material = _mat;
        lr.startWidth = lr.endWidth = lineWidth;
        lr.sortingOrder = sortingOrder;
        lr.numCapVertices = 2;
    }
}
