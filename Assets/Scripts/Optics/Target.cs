// TODO: Target implementation
using UnityEngine;

public class Target : Optic {
    public BeamColor required = BeamColor.White;
    [HideInInspector] public bool isSatisfied;

    public override (bool stop, System.Collections.Generic.List<Beam> outs) Interact(Beam incoming) {
        if ((incoming.Color & required) == required) isSatisfied = true;
        // цель не блокирует — луч проходит дальше
        return (false, null);
    }

    void OnDrawGizmos(){
        Gizmos.color = isSatisfied ? Color.green : Color.gray;
        Gizmos.DrawWireSphere(transform.position, 0.35f);
    }
}