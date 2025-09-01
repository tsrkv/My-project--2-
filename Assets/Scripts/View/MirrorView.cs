using UnityEngine;

[RequireComponent(typeof(Mirror))]
public class MirrorView : MonoBehaviour
{
    LineRenderer lr;
    void Awake()
    {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.startWidth = lr.endWidth = 0.08f;
        lr.numCapVertices = 2;
        lr.sortingOrder = 20;
        UpdateVisual();
    }
    void OnValidate(){ if (lr) UpdateVisual(); }
    void Update(){ UpdateVisual(); }

    void UpdateVisual()
    {
        var m = GetComponent<Mirror>();
        lr.material.color = Color.white;
        // рисуем небольшой сегмент “/” или “\” в пределах клетки
        if (m.type == MirrorType.Slash)
        {
            lr.SetPosition(0, new Vector3(-0.35f, -0.35f, 0));
            lr.SetPosition(1, new Vector3( 0.35f,  0.35f, 0));
        }
        else
        {
            lr.SetPosition(0, new Vector3(-0.35f,  0.35f, 0));
            lr.SetPosition(1, new Vector3( 0.35f, -0.35f, 0));
        }
    }
}
