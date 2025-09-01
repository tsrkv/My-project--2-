using UnityEngine;

public abstract class Optic : MonoBehaviour {
    // Возвращает: остановить ли луч и/или список выходящих лучей
    public abstract (bool stop, System.Collections.Generic.List<Beam> outs) Interact(Beam incoming);
    public virtual bool Blocks => false;
}

public struct Beam {
    public Vector2Int Cell;
    public Direction Dir;
    public BeamColor Color;
}
