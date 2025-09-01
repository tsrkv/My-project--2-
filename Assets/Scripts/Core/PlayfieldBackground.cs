using UnityEngine;

[ExecuteAlways]
public class PlayfieldBackground : MonoBehaviour
{
    [Header("Size (cells)")]
    public int width = 12;
    public int height = 8;
    public float cell = 1f;

    [Header("Layout")]
    public bool autoCenterToCamera = true;
    public float topMarginWorld = 1.2f;

    [Header("Style")]
    public Color fillColor = new Color(0.08f, 0.08f, 0.08f, 1f);
    public Color borderColor = new Color(1f, 1f, 1f, 0.85f);
    public float borderWidth = 0.04f;

    Transform _root;
    SpriteRenderer _fill;
    LineRenderer _border;
    static Sprite _white1x1;

    void OnEnable()   => Rebuild();
    void OnValidate() => Rebuild();

    Vector2 Origin()
    {
        if (autoCenterToCamera && Camera.main)
        {
            var cam = Camera.main;
            float halfH = cam.orthographicSize;
            var c = (Vector2)cam.transform.position;
            float w = width * cell;
            float h = height * cell;
            return new Vector2(c.x - w * 0.5f, (c.y + halfH - topMarginWorld) - h);
        }
        return Vector2.zero;
    }

    void Ensure()
    {
        if (_root == null)
        {
            var go = GameObject.Find("PlayfieldRoot");
            if (!go){ go = new GameObject("PlayfieldRoot"); go.transform.SetParent(transform, false); }
            _root = go.transform;
        }
        if (_white1x1 == null)
        {
            var tex = new Texture2D(1,1,TextureFormat.RGBA32,false);
            tex.SetPixel(0,0,Color.white); tex.Apply();
            _white1x1 = Sprite.Create(tex, new Rect(0,0,1,1), new Vector2(0.5f,0.5f), 1f);
        }
        if (_fill == null)
        {
            var g = new GameObject("Fill");
            g.transform.SetParent(_root, false);
            _fill = g.AddComponent<SpriteRenderer>();
            _fill.sharedMaterial = null; // Sprites/Default
            _fill.sprite = _white1x1;
            _fill.sortingOrder = -200;   // ниже всего
        }
        if (_border == null)
        {
            var g = new GameObject("Border");
            g.transform.SetParent(_root, false);
            _border = g.AddComponent<LineRenderer>();
            _border.useWorldSpace = true;
            _border.loop = true;
            _border.material = new Material(Shader.Find("Unlit/Color"));
            _border.numCornerVertices = 2;
            _border.sortingOrder = -150; // выше заливки, ниже оптики/лучей
        }
    }

    public void Rebuild()
    {
        if (!isActiveAndEnabled) return;
        Ensure();

        Vector2 o = Origin();
        float w = width * cell;
        float h = height * cell;

        // fill
        _fill.color = fillColor;
        _fill.transform.position   = new Vector3(o.x + w*0.5f, o.y + h*0.5f, 0f);
        _fill.transform.localScale = new Vector3(w, h, 1f);

        // border
        _border.startWidth = _border.endWidth = borderWidth;
        _border.material.color = borderColor;
        _border.positionCount = 4;
        _border.SetPositions(new Vector3[]{
            new Vector3(o.x,   o.y,   0),
            new Vector3(o.x+w, o.y,   0),
            new Vector3(o.x+w, o.y+h, 0),
            new Vector3(o.x,   o.y+h, 0),
        });
    }
}
