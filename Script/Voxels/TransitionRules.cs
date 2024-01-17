using System.Collections.Generic;
using static StarLoop.Script.Cell;
namespace StarLoop.Script;

public enum Cell : byte
{
    Air,
    Wire,
    RedHead,
    RedTail,
    GreenHead,
    GreenTail,
    BlueHead,
    BlueTail,
}
public static class TransitionRules
{
    private static Rule[] Always => System.Array.Empty<Rule>();
    public static readonly List<WireworldTransition> Transitions = new List<WireworldTransition>()
    {
        new(new Rule[] { new(RedHead, 1, 2) }, Wire, RedHead),
        new(Always, RedHead, RedTail), 
        new(Always, RedTail, Wire),
        
        new(new Rule[] { new(GreenHead, 1, 2) }, Wire, GreenHead),
        new(Always, GreenHead, GreenTail),
        new(Always, GreenTail, Wire), 
        
        new(new Rule[] { new(BlueHead, 1, 2) }, Wire, BlueHead),
        new(Always, BlueHead, BlueTail),
        new(Always, BlueTail, Wire),
    };
}