// TODO: Wall implementation
public class Wall : Optic {
    public override bool Blocks => true;
    public override (bool stop, System.Collections.Generic.List<Beam> outs) Interact(Beam incoming)
        => (true, null);
}
