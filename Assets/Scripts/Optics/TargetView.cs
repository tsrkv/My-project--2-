using UnityEngine;

[RequireComponent(typeof(Target))]
public class TargetView : MonoBehaviour
{
    SpriteRenderer sr;
    void Awake()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sharedMaterial = null;  // Sprites/Default
        sr.sortingOrder = 16;

        var tex = new Texture2D(1,1,TextureFormat.RGBA32,false);
        tex.SetPixel(0,0,Color.white); tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0,0,1,1), new Vector2(0.5f,0.5f), 1f);
        transform.localScale = Vector3.one * 0.8f;
    }

    void Update()
    {
        var t = GetComponent<Target>();
        // серый — пустой, зелёный — выполнен
        sr.color = t.isSatisfied ? new Color(0.2f, 0.9f, 0.2f, 1f)
                                 : new Color(0.25f,0.25f,0.25f,1f);
    }
}
