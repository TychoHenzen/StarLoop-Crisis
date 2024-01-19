namespace StarLoop.Script;

public struct Rule
{
    public readonly byte Type;
    public readonly byte Min;
    public readonly byte Max;

    public Rule(byte type, byte min, byte max, float odds = 1)
    {
        Type = type;
        Min = min;
        Max = max;
    }
    public Rule(Cell type, byte min, byte max, float odds = 1)
    {
        Type = (byte)type;
        Min = min;
        Max = max;
    }
}