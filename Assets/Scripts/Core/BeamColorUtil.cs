using UnityEngine;

public static class BeamColorUtil
{
    public static Color BeamColorToUnity(BeamColor c)
    {
        float r = (c & BeamColor.R) != 0 ? 1f : 0f;
        float g = (c & BeamColor.G) != 0 ? 1f : 0f;
        float b = (c & BeamColor.B) != 0 ? 1f : 0f;
        if (r==0 && g==0 && b==0) return Color.white;
        return new Color(r,g,b,1f);
    }
}
