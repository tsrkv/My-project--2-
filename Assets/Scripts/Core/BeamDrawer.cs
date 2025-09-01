// TODO: BeamDrawer implementation
using UnityEngine;
using System.Collections.Generic;

public class BeamDrawer : MonoBehaviour
{
    readonly List<LineRenderer> pool = new();
    Material mat;

    void Awake()
    {
        // Один базовый материал (renderer.material создаст копию на рендерере — цвета не будут конфликтовать)
        mat = new Material(Shader.Find("Unlit/Color"));
    }

    LineRenderer GetLine()
    {
        foreach (var lr in pool)
            if (!lr.enabled)
            {
                lr.enabled = true;
                return lr;
            }

        var go = new GameObject("BeamLine");
        go.transform.SetParent(transform, false);

        var l = go.AddComponent<LineRenderer>();
        l.useWorldSpace = true;
        l.positionCount = 2;
        l.startWidth = l.endWidth = 0.08f;   // чуть толще, чтобы лучше читался
        l.numCapVertices = 6;                // округлённые концы
        l.material = mat;                    // renderer.material -> уникальный инстанс на каждом LineRenderer
        l.sortingOrder = 30;                 // поверх сетки/фона
        pool.Add(l);
        return l;
    }

    /// <summary>
    /// segs: список отрезков (A,B,цвет луча)
    /// </summary>
    public void Draw(List<(Vector3 a, Vector3 b, BeamColor col)> segs)
    {
        // выключаем все существующие линии
        foreach (var lr in pool) lr.enabled = false;

        // рисуем текущие сегменты
        foreach (var s in segs)
        {
            var lr = GetLine();

            // цвет луча по RGB-флагам
            lr.material.color = ToColor(s.col);

            // необязательный мягкий градиент (ядро->края)
            var g = new Gradient();
            g.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(lr.material.color, 0f),
                    new GradientColorKey(lr.material.color, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.0f, 0f),
                    new GradientAlphaKey(1.0f, 0.15f),
                    new GradientAlphaKey(1.0f, 0.85f),
                    new GradientAlphaKey(0.0f, 1f)
                }
            );
            lr.colorGradient = g;

            // лёгкий сдвиг по Z, чтобы не слипалось с фоном
            lr.SetPosition(0, s.a + Vector3.forward * 0.0f);
            lr.SetPosition(1, s.b + Vector3.forward * 0.0f);
        }
    }

    Color ToColor(BeamColor c)
    {
        float r = (c & BeamColor.R) != 0 ? 1f : 0f;
        float g = (c & BeamColor.G) != 0 ? 1f : 0f;
        float b = (c & BeamColor.B) != 0 ? 1f : 0f;
        // если ни один компонент не выставлен — считаем белым
        if (r == 0f && g == 0f && b == 0f) return Color.white;
        return new Color(r, g, b, 1f);
    }
}
