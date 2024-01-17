namespace StarLoop.Script;

public struct WireworldTransition
{
    public readonly Rule[] Neighbors;
    public readonly byte Self;
    public readonly byte Result;

    public WireworldTransition(Rule[] neighbors, byte self, byte result)
    {
        Neighbors = neighbors;
        Self = self;
        Result = result;
    }
    
    public WireworldTransition(Rule[] neighbors, Cell self, Cell result)
    {
        Neighbors = neighbors;
        Self = (byte)self;
        Result = (byte)result;
    }
}