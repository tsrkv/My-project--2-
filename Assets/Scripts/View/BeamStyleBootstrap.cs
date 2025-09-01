using UnityEngine;

public class BeamStyleBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Apply()
    {
        var mat = new Material(Shader.Find("Unlit/Color")); // без освещения
        foreach (var lr in Object.FindObjectsByType<LineRenderer>(FindObjectsSortMode.None))
        {
            if (!lr.name.Contains("Beam")) continue;
            lr.material = mat;
            lr.widthMultiplier = 0.08f;
            lr.numCapVertices = 6;
            lr.sortingOrder = 30;

            // мягкий "ядро+свечение"
            var g = new Gradient();
            g.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(lr.material.color == default ? Color.white : lr.material.color, 0f),
                    new GradientColorKey(lr.material.color == default ? Color.white : lr.material.color, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.0f, 0f),
                    new GradientAlphaKey(1.0f, 0.15f),
                    new GradientAlphaKey(1.0f, 0.85f),
                    new GradientAlphaKey(0.0f, 1f)
                }
            );
            lr.colorGradient = g;
        }
    }
}
