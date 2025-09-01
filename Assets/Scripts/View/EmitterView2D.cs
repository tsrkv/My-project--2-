using UnityEngine;

[RequireComponent(typeof(Emitter))]
public class EmitterView2D : MonoBehaviour
{
    static Sprite _white1x1;          // кэш одного спрайта на все объекты
    SpriteRenderer sr;
    LineRenderer arrow;

    void Awake()
    {
        // --- гарантируем спрайт 1x1
        if (_white1x1 == null)
        {
            var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _white1x1 = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        // --- квадрат-эмиттер
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sharedMaterial = null;             // Sprites/Default
        sr.sortingOrder = 15;
        sr.sprite = _white1x1;
        transform.localScale = Vector3.one * 0.8f;
        sr.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        // --- стрелка-направление: получаем существующий или создаём
        arrow = GetComponent<LineRenderer>();
        if (arrow == null) arrow = gameObject.AddComponent<LineRenderer>();
        arrow.useWorldSpace = false;
        if (arrow.material == null)
            arrow.material = new Material(Shader.Find("Unlit/Color"));
        arrow.material.color = Color.red;
        arrow.positionCount = 2;
        arrow.startWidth = arrow.endWidth = 0.06f;
        arrow.numCapVertices = 2;
        arrow.sortingOrder = 20;

        UpdateArrow();
    }

    void Update() => UpdateArrow();

    void UpdateArrow()
    {
        var e = GetComponent<Emitter>();
        Vector3 dir = e.direction switch
        {
            Direction.Left  => Vector3.left,
            Direction.Up    => Vector3.up,
            Direction.Down  => Vector3.down,
            _               => Vector3.right
        };
        arrow.SetPosition(0, Vector3.zero);
        arrow.SetPosition(1, dir * 0.5f);
    }
}
