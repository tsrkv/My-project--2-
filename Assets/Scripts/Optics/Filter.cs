// TODO: Filter implementation
using UnityEngine;
using System.Collections.Generic;

public class Filter : Optic {
    public BeamColor pass = BeamColor.R; // пропускаемый цвет
    public override (bool stop, List<Beam> outs) Interact(Beam incoming) {
        var newColor = incoming.Color & pass;
        if (newColor == BeamColor.None) return (true, null); // всё отфильтровали — луч гаснет
        return (false, new List<Beam>{ new Beam{ Cell=incoming.Cell, Dir=incoming.Dir, Color=newColor }});
    }

    void OnDrawGizmos(){
        var p = transform.position;
        var c = (pass & BeamColor.R)!=0 ? Color.red :
                (pass & BeamColor.G)!=0 ? Color.green :
                (pass & BeamColor.B)!=0 ? Color.blue : Color.gray;
        Gizmos.color = c;
        Gizmos.DrawWireCube(p, new Vector3(0.8f,0.8f,0));
    }
}
