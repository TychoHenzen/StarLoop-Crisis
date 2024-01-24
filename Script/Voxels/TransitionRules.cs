using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;
using static StarLoop.Script.Voxels.Cell;

namespace StarLoop.Script.Voxels;

public static class TransitionRules
{
    public static readonly ImmutableDictionary<byte, WireWorldTransition[]> Transitions = GenerateTransitions();
    private static IEnumerable<Rule> Always => Array.Empty<Rule>();

    private static Func<double, bool>[] Odds => new Func<double, bool>[]
    {
        v => v < 1,
        v => v < 0.99f,
        v => v < 0.98,
        v => v < 0.9,
        v => v < 0.8
    };

    private static Cell[] Wires => new[] { Wire, WireDecay1, WireDecay2, WireDecay3, WireDecay4 };

    private static Cell[] RedHeads => new[] { RedHead, RedHeadDecay1, RedHeadDecay2, RedHeadDecay3, RedHeadDecay4 };
    private static Cell[] RedTails => new[] { RedTail, RedTailDecay1, RedTailDecay2, RedTailDecay3, RedTailDecay4 };

    private static Cell[] GreenHeads => new[]
        { GreenHead, GreenHeadDecay1, GreenHeadDecay2, GreenHeadDecay3, GreenHeadDecay4 };

    private static Cell[] GreenTails => new[]
        { GreenTail, GreenTailDecay1, GreenTailDecay2, GreenTailDecay3, GreenTailDecay4 };

    private static Cell[] BlueHeads => new[]
        { BlueHead, BlueHeadDecay1, BlueHeadDecay2, BlueHeadDecay3, BlueHeadDecay4 };

    private static Cell[] BlueTails => new[]
        { BlueTail, BlueTailDecay1, BlueTailDecay2, BlueTailDecay3, BlueTailDecay4 };

    private static Cell[] CyanHeads => new[]
        { CyanHead, CyanHeadDecay1, CyanHeadDecay2, CyanHeadDecay3, CyanHeadDecay4 };

    private static Cell[] CyanTails => new[]
        { CyanTail, CyanTailDecay1, CyanTailDecay2, CyanTailDecay3, CyanTailDecay4 };

    private static Cell[] MagentaHeads => new[]
        { MagentaHead, MagentaHeadDecay1, MagentaHeadDecay2, MagentaHeadDecay3, MagentaHeadDecay4 };

    private static Cell[] MagentaTails => new[]
        { MagentaTail, MagentaTailDecay1, MagentaTailDecay2, MagentaTailDecay3, MagentaTailDecay4 };

    private static Cell[] YellowHeads => new[]
        { YellowHead, YellowHeadDecay1, YellowHeadDecay2, YellowHeadDecay3, YellowHeadDecay4 };

    private static Cell[] YellowTails => new[]
        { YellowTail, YellowTailDecay1, YellowTailDecay2, YellowTailDecay3, YellowTailDecay4 };

    private static Cell[] WhiteHeads => new[]
        { WhiteHead, WhiteHeadDecay1, WhiteHeadDecay2, WhiteHeadDecay3, WhiteHeadDecay4 };

    private static Cell[] WhiteTails => new[]
        { WhiteTail, WhiteTailDecay1, WhiteTailDecay2, WhiteTailDecay3, WhiteTailDecay4 };

    private static Cell[] RedFilters => new[]
        { RedFilter, RedFilterDecay1, RedFilterDecay2, RedFilterDecay3, RedFilterDecay4 };

    private static Cell[] GreenFilters => new[]
        { GreenFilter, GreenFilterDecay1, GreenFilterDecay2, GreenFilterDecay3, GreenFilterDecay4 };

    private static Cell[] BlueFilters => new[]
        { BlueFilter, BlueFilterDecay1, BlueFilterDecay2, BlueFilterDecay3, BlueFilterDecay4 };

    private static Cell[] CyanFilters => new[]
        { CyanFilter, CyanFilterDecay1, CyanFilterDecay2, CyanFilterDecay3, CyanFilterDecay4 };

    private static Cell[] MagentaFilters => new[]
        { MagentaFilter, MagentaFilterDecay1, MagentaFilterDecay2, MagentaFilterDecay3, MagentaFilterDecay4 };

    private static Cell[] YellowFilters => new[]
        { YellowFilter, YellowFilterDecay1, YellowFilterDecay2, YellowFilterDecay3, YellowFilterDecay4 };

    private static Cell[] RedFilterRedHeads => new[]
    {
        RedFilterRedHead, RedFilterRedHeadDecay1, RedFilterRedHeadDecay2, RedFilterRedHeadDecay3, RedFilterRedHeadDecay4
    };

    private static Cell[] GreenFilterGreenHeads => new[]
    {
        GreenFilterGreenHead, GreenFilterGreenHeadDecay1, GreenFilterGreenHeadDecay2, GreenFilterGreenHeadDecay3,
        GreenFilterGreenHeadDecay4
    };

    private static Cell[] BlueFilterBlueHeads => new[]
    {
        BlueFilterBlueHead, BlueFilterBlueHeadDecay1, BlueFilterBlueHeadDecay2, BlueFilterBlueHeadDecay3,
        BlueFilterBlueHeadDecay4
    };

    private static Cell[] CyanFilterCyanHeads => new[]
    {
        CyanFilterCyanHead, CyanFilterCyanHeadDecay1, CyanFilterCyanHeadDecay2, CyanFilterCyanHeadDecay3,
        CyanFilterCyanHeadDecay4
    };

    private static Cell[] MagentaFilterMagentaHeads => new[]
    {
        MagentaFilterMagentaHead, MagentaFilterMagentaHeadDecay1, MagentaFilterMagentaHeadDecay2,
        MagentaFilterMagentaHeadDecay3, MagentaFilterMagentaHeadDecay4
    };

    private static Cell[] YellowFilterYellowHeads => new[]
    {
        YellowFilterYellowHead, YellowFilterYellowHeadDecay1, YellowFilterYellowHeadDecay2,
        YellowFilterYellowHeadDecay3, YellowFilterYellowHeadDecay4
    };

    private static Cell[] CyanFilterGreenHeads => new[]
    {
        CyanFilterGreenHead, CyanFilterGreenHeadDecay1, CyanFilterGreenHeadDecay2, CyanFilterGreenHeadDecay3,
        CyanFilterGreenHeadDecay4
    };

    private static Cell[] MagentaFilterBlueHeads => new[]
    {
        MagentaFilterBlueHead, MagentaFilterBlueHeadDecay1, MagentaFilterBlueHeadDecay2, MagentaFilterBlueHeadDecay3,
        MagentaFilterBlueHeadDecay4
    };

    private static Cell[] YellowFilterRedHeads => new[]
    {
        YellowFilterRedHead, YellowFilterRedHeadDecay1, YellowFilterRedHeadDecay2, YellowFilterRedHeadDecay3,
        YellowFilterRedHeadDecay4
    };

    private static Cell[] CyanFilterBlueHeads => new[]
    {
        CyanFilterBlueHead, CyanFilterBlueHeadDecay1, CyanFilterBlueHeadDecay2, CyanFilterBlueHeadDecay3,
        CyanFilterBlueHeadDecay4
    };

    private static Cell[] MagentaFilterRedHeads => new[]
    {
        MagentaFilterRedHead, MagentaFilterRedHeadDecay1, MagentaFilterRedHeadDecay2, MagentaFilterRedHeadDecay3,
        MagentaFilterRedHeadDecay4
    };

    private static Cell[] YellowFilterGreenHeads => new[]
    {
        YellowFilterGreenHead, YellowFilterGreenHeadDecay1, YellowFilterGreenHeadDecay2, YellowFilterGreenHeadDecay3,
        YellowFilterGreenHeadDecay4
    };


    private static Cell[] RedFilterRedTails => new[]
    {
        RedFilterRedTail, RedFilterRedTailDecay1, RedFilterRedTailDecay2, RedFilterRedTailDecay3, RedFilterRedTailDecay4
    };

    private static Cell[] GreenFilterGreenTails => new[]
    {
        GreenFilterGreenTail, GreenFilterGreenTailDecay1, GreenFilterGreenTailDecay2, GreenFilterGreenTailDecay3,
        GreenFilterGreenTailDecay4
    };

    private static Cell[] BlueFilterBlueTails => new[]
    {
        BlueFilterBlueTail, BlueFilterBlueTailDecay1, BlueFilterBlueTailDecay2, BlueFilterBlueTailDecay3,
        BlueFilterBlueTailDecay4
    };

    private static Cell[] CyanFilterCyanTails => new[]
    {
        CyanFilterCyanTail, CyanFilterCyanTailDecay1, CyanFilterCyanTailDecay2, CyanFilterCyanTailDecay3,
        CyanFilterCyanTailDecay4
    };

    private static Cell[] MagentaFilterMagentaTails => new[]
    {
        MagentaFilterMagentaTail, MagentaFilterMagentaTailDecay1, MagentaFilterMagentaTailDecay2,
        MagentaFilterMagentaTailDecay3, MagentaFilterMagentaTailDecay4
    };

    private static Cell[] YellowFilterYellowTails => new[]
    {
        YellowFilterYellowTail, YellowFilterYellowTailDecay1, YellowFilterYellowTailDecay2,
        YellowFilterYellowTailDecay3, YellowFilterYellowTailDecay4
    };

    private static Cell[] CyanFilterGreenTails => new[]
    {
        CyanFilterGreenTail, CyanFilterGreenTailDecay1, CyanFilterGreenTailDecay2, CyanFilterGreenTailDecay3,
        CyanFilterGreenTailDecay4
    };

    private static Cell[] MagentaFilterBlueTails => new[]
    {
        MagentaFilterBlueTail, MagentaFilterBlueTailDecay1, MagentaFilterBlueTailDecay2, MagentaFilterBlueTailDecay3,
        MagentaFilterBlueTailDecay4
    };

    private static Cell[] YellowFilterRedTails => new[]
    {
        YellowFilterRedTail, YellowFilterRedTailDecay1, YellowFilterRedTailDecay2, YellowFilterRedTailDecay3,
        YellowFilterRedTailDecay4
    };

    private static Cell[] CyanFilterBlueTails => new[]
    {
        CyanFilterBlueTail, CyanFilterBlueTailDecay1, CyanFilterBlueTailDecay2, CyanFilterBlueTailDecay3,
        CyanFilterBlueTailDecay4
    };

    private static Cell[] MagentaFilterRedTails => new[]
    {
        MagentaFilterRedTail, MagentaFilterRedTailDecay1, MagentaFilterRedTailDecay2, MagentaFilterRedTailDecay3,
        MagentaFilterRedTailDecay4
    };

    private static Cell[] YellowFilterGreenTails => new[]
    {
        YellowFilterGreenTail, YellowFilterGreenTailDecay1, YellowFilterGreenTailDecay2, YellowFilterGreenTailDecay3,
        YellowFilterGreenTailDecay4
    };

    private static ImmutableDictionary<byte, WireWorldTransition[]> GenerateTransitions()
    {
        var returned = new List<WireWorldTransition>();
        returned.AddRange(MakeWires(returned));
        returned.AddRange(Merge());
        returned.AddRange(Filter());
        returned.AddRange(Decay());
        returned.AddRange(Spread());
        returned.AddRange(Lights());

        returned.Add(new WireWorldTransition(Always, ClosedWindow, Glass, _ => WireWorldController.Step % 10 == 0));
        returned.Add(new WireWorldTransition(Always, Glass, ClosedWindow, _ => true));


        returned.AddRange(Spread());

        GD.Print($"Found {returned.Count} entries");
        var ret = returned.Distinct().ToArray();
        GD.Print($"trimmed to {ret.Length} entries");
        var results = new Dictionary<byte, WireWorldTransition[]>();
        for (byte b = 0; b < 255; b++)
        {
            results.Add(b, ret.Where(transition => transition.Self == b).Reverse().ToArray());
        }

        return results.ToImmutableDictionary();
    }

    private static IEnumerable<WireWorldTransition> MakeWires(List<WireWorldTransition> returned)
    {
        foreach (var wireWorldTransition in SimpleWire(Wires, RedHeads, RedTails, Odds))
            yield return wireWorldTransition;
        returned.AddRange(SimpleWire(Wires, GreenHeads, GreenTails, Odds));
        returned.AddRange(SimpleWire(Wires, BlueHeads, BlueTails, Odds));
        returned.AddRange(SimpleWire(Wires, CyanHeads, CyanTails, Odds));
        returned.AddRange(SimpleWire(Wires, MagentaHeads, MagentaTails, Odds));
        returned.AddRange(SimpleWire(Wires, YellowHeads, YellowTails, Odds));
        returned.AddRange(SimpleWire(Wires, WhiteHeads, WhiteTails, Odds));

        returned.AddRange(SimpleWire(RedFilters, RedFilterRedHeads, RedFilterRedTails, Odds));
        returned.AddRange(SimpleWire(GreenFilters, GreenFilterGreenHeads, GreenFilterGreenTails, Odds));
        returned.AddRange(SimpleWire(BlueFilters, BlueFilterBlueHeads, BlueFilterBlueTails, Odds));
        returned.AddRange(SimpleWire(CyanFilters, CyanFilterCyanHeads, CyanFilterCyanTails, Odds));
        returned.AddRange(SimpleWire(MagentaFilters, MagentaFilterMagentaHeads, MagentaFilterMagentaTails, Odds));
        returned.AddRange(SimpleWire(YellowFilters, YellowFilterYellowHeads, YellowFilterYellowTails, Odds));

        returned.AddRange(SimpleWire(CyanFilters, CyanFilterBlueHeads, CyanFilterBlueTails, Odds));
        returned.AddRange(SimpleWire(CyanFilters, CyanFilterGreenHeads, CyanFilterGreenTails, Odds));
        returned.AddRange(SimpleWire(MagentaFilters, MagentaFilterRedHeads, MagentaFilterRedTails, Odds));
        returned.AddRange(SimpleWire(MagentaFilters, MagentaFilterBlueHeads, MagentaFilterBlueTails, Odds));
        returned.AddRange(SimpleWire(YellowFilters, YellowFilterRedHeads, YellowFilterRedTails, Odds));
        returned.AddRange(SimpleWire(YellowFilters, YellowFilterGreenHeads, YellowFilterGreenTails, Odds));
    }

    private static IEnumerable<WireWorldTransition> Lights()
    {
        for (var headIndex = 0; headIndex < Wires.Length; headIndex++)
        {
            foreach (var wireWorldTransition in LampUp(headIndex)) yield return wireWorldTransition;
            foreach (var wireWorldTransition1 in LampDown()) yield return wireWorldTransition1;
        }
    }

    private static IEnumerable<WireWorldTransition> LampDown()
    {
        Func<double, bool> func = v => WireWorldController.Step % 10 == 0 && v < 0.5;
        yield return new WireWorldTransition(Always, Lamp1, Lamp2, func);
        yield return new WireWorldTransition(Always, Lamp1, Lamp2, func);
        yield return new WireWorldTransition(Always, Lamp1, Lamp2, func);
        yield return new WireWorldTransition(Always, Lamp1, Lamp2, func);
        yield return new WireWorldTransition(Always, Lamp1, Lamp2, func);
        yield return new WireWorldTransition(Always, Lamp1, Lamp2, func);
        yield return new WireWorldTransition(Always, Lamp1, Lamp2, func);

        yield return new WireWorldTransition(Always, Lamp2, Lamp3, func);
        yield return new WireWorldTransition(Always, Lamp2, Lamp3, func);
        yield return new WireWorldTransition(Always, Lamp2, Lamp3, func);
        yield return new WireWorldTransition(Always, Lamp2, Lamp3, func);
        yield return new WireWorldTransition(Always, Lamp2, Lamp3, func);
        yield return new WireWorldTransition(Always, Lamp2, Lamp3, func);
        yield return new WireWorldTransition(Always, Lamp2, Lamp3, func);

        yield return new WireWorldTransition(Always, Lamp3, Lamp4, func);
        yield return new WireWorldTransition(Always, Lamp3, Lamp4, func);
        yield return new WireWorldTransition(Always, Lamp3, Lamp4, func);
        yield return new WireWorldTransition(Always, Lamp3, Lamp4, func);
        yield return new WireWorldTransition(Always, Lamp3, Lamp4, func);
        yield return new WireWorldTransition(Always, Lamp3, Lamp4, func);
        yield return new WireWorldTransition(Always, Lamp3, Lamp4, func);
    }

    private static IEnumerable<WireWorldTransition> LampUp(int headIndex)
    {
        yield return new WireWorldTransition(new Rule[] { new(RedHeads[headIndex], 1, 2) }, Lamp1, Lamp2, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(GreenHeads[headIndex], 1, 2) }, Lamp1, Lamp2, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(BlueHeads[headIndex], 1, 2) }, Lamp1, Lamp2, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(CyanHeads[headIndex], 1, 2) }, Lamp1, Lamp2, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(MagentaHeads[headIndex], 1, 2) }, Lamp1, Lamp2,
            _ => true);
        yield return new WireWorldTransition(new Rule[] { new(YellowHeads[headIndex], 1, 2) }, Lamp1, Lamp2, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) }, Lamp1, Lamp2, _ => true);

        yield return new WireWorldTransition(new Rule[] { new(RedHeads[headIndex], 1, 2) }, Lamp2, Lamp3, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(GreenHeads[headIndex], 1, 2) }, Lamp2, Lamp3, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(BlueHeads[headIndex], 1, 2) }, Lamp2, Lamp3, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(CyanHeads[headIndex], 1, 2) }, Lamp2, Lamp3, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(MagentaHeads[headIndex], 1, 2) }, Lamp2, Lamp3,
            _ => true);
        yield return new WireWorldTransition(new Rule[] { new(YellowHeads[headIndex], 1, 2) }, Lamp2, Lamp3, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) }, Lamp2, Lamp3, _ => true);

        yield return new WireWorldTransition(new Rule[] { new(RedHeads[headIndex], 1, 2) }, Lamp3, Lamp4, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(GreenHeads[headIndex], 1, 2) }, Lamp3, Lamp4, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(BlueHeads[headIndex], 1, 2) }, Lamp3, Lamp4, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(CyanHeads[headIndex], 1, 2) }, Lamp3, Lamp4, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(MagentaHeads[headIndex], 1, 2) }, Lamp3, Lamp4,
            _ => true);
        yield return new WireWorldTransition(new Rule[] { new(YellowHeads[headIndex], 1, 2) }, Lamp3, Lamp4, _ => true);
        yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) }, Lamp3, Lamp4, _ => true);
    }

    private static IEnumerable<WireWorldTransition> Spread()
    {
        Func<double, bool> t1Spread = v => v < 0.001;
        Func<double, bool> t2Spread = v => v < 0.01;
        Func<double, bool> t3Spread = v => v < 0.1;
        yield return new WireWorldTransition(new Rule[] { new(WireDecay1, 1, 1) }, Wire, WireDecay1, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(WireDecay1, 2, 2) }, Wire, WireDecay1, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(WireDecay1, 3, 3) }, Wire, WireDecay1, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(WireDecay2, 1, 1) }, WireDecay1, WireDecay2, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(WireDecay2, 2, 2) }, WireDecay1, WireDecay2, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(WireDecay2, 3, 3) }, WireDecay1, WireDecay2, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(WireDecay3, 1, 1) }, WireDecay2, WireDecay3, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(WireDecay3, 2, 2) }, WireDecay2, WireDecay3, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(WireDecay3, 3, 3) }, WireDecay2, WireDecay3, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(WireDecay4, 1, 1) }, WireDecay3, WireDecay4, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(WireDecay4, 2, 2) }, WireDecay3, WireDecay4, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(WireDecay4, 3, 3) }, WireDecay3, WireDecay4, t3Spread);

        yield return new WireWorldTransition(new Rule[] { new(RedFilterDecay1, 1, 1) }, RedFilter, RedFilterDecay1,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(RedFilterDecay1, 2, 2) }, RedFilter, RedFilterDecay1,
            t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(RedFilterDecay1, 3, 3) }, RedFilter, RedFilterDecay1,
            t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(RedFilterDecay2, 1, 1) }, RedFilterDecay1,
            RedFilterDecay2, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(RedFilterDecay2, 2, 2) }, RedFilterDecay1,
            RedFilterDecay2, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(RedFilterDecay2, 3, 3) }, RedFilterDecay1,
            RedFilterDecay2, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(RedFilterDecay3, 1, 1) }, RedFilterDecay2,
            RedFilterDecay3, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(RedFilterDecay3, 2, 2) }, RedFilterDecay2,
            RedFilterDecay3, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(RedFilterDecay3, 3, 3) }, RedFilterDecay2,
            RedFilterDecay3, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(RedFilterDecay4, 1, 1) }, RedFilterDecay3,
            RedFilterDecay4, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(RedFilterDecay4, 2, 2) }, RedFilterDecay3,
            RedFilterDecay4, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(RedFilterDecay4, 3, 3) }, RedFilterDecay3,
            RedFilterDecay4, t3Spread);

        yield return new WireWorldTransition(new Rule[] { new(GreenFilterDecay1, 1, 1) }, GreenFilter,
            GreenFilterDecay1, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenFilterDecay1, 2, 2) }, GreenFilter,
            GreenFilterDecay1, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenFilterDecay1, 3, 3) }, GreenFilter,
            GreenFilterDecay1, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenFilterDecay2, 1, 1) }, GreenFilterDecay1,
            GreenFilterDecay2, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenFilterDecay2, 2, 2) }, GreenFilterDecay1,
            GreenFilterDecay2, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenFilterDecay2, 3, 3) }, GreenFilterDecay1,
            GreenFilterDecay2, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenFilterDecay3, 1, 1) }, GreenFilterDecay2,
            GreenFilterDecay3, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenFilterDecay3, 2, 2) }, GreenFilterDecay2,
            GreenFilterDecay3, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenFilterDecay3, 3, 3) }, GreenFilterDecay2,
            GreenFilterDecay3, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenFilterDecay4, 1, 1) }, GreenFilterDecay3,
            GreenFilterDecay4, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenFilterDecay4, 2, 2) }, GreenFilterDecay3,
            GreenFilterDecay4, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenFilterDecay4, 3, 3) }, GreenFilterDecay3,
            GreenFilterDecay4, t3Spread);

        yield return new WireWorldTransition(new Rule[] { new(BlueFilterDecay1, 1, 1) }, BlueFilter, BlueFilterDecay1,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueFilterDecay1, 2, 2) }, BlueFilter, BlueFilterDecay1,
            t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueFilterDecay1, 3, 3) }, BlueFilter, BlueFilterDecay1,
            t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueFilterDecay2, 1, 1) }, BlueFilterDecay1,
            BlueFilterDecay2, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueFilterDecay2, 2, 2) }, BlueFilterDecay1,
            BlueFilterDecay2, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueFilterDecay2, 3, 3) }, BlueFilterDecay1,
            BlueFilterDecay2, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueFilterDecay3, 1, 1) }, BlueFilterDecay2,
            BlueFilterDecay3, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueFilterDecay3, 2, 2) }, BlueFilterDecay2,
            BlueFilterDecay3, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueFilterDecay3, 3, 3) }, BlueFilterDecay2,
            BlueFilterDecay3, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueFilterDecay4, 1, 1) }, BlueFilterDecay3,
            BlueFilterDecay4, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueFilterDecay4, 2, 2) }, BlueFilterDecay3,
            BlueFilterDecay4, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueFilterDecay4, 3, 3) }, BlueFilterDecay3,
            BlueFilterDecay4, t3Spread);

        yield return new WireWorldTransition(new Rule[] { new(CyanFilterDecay1, 1, 1) }, CyanFilter, CyanFilterDecay1,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanFilterDecay1, 2, 2) }, CyanFilter, CyanFilterDecay1,
            t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanFilterDecay1, 3, 3) }, CyanFilter, CyanFilterDecay1,
            t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanFilterDecay2, 1, 1) }, CyanFilterDecay1,
            CyanFilterDecay2, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanFilterDecay2, 2, 2) }, CyanFilterDecay1,
            CyanFilterDecay2, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanFilterDecay2, 3, 3) }, CyanFilterDecay1,
            CyanFilterDecay2, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanFilterDecay3, 1, 1) }, CyanFilterDecay2,
            CyanFilterDecay3, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanFilterDecay3, 2, 2) }, CyanFilterDecay2,
            CyanFilterDecay3, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanFilterDecay3, 3, 3) }, CyanFilterDecay2,
            CyanFilterDecay3, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanFilterDecay4, 1, 1) }, CyanFilterDecay3,
            CyanFilterDecay4, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanFilterDecay4, 2, 2) }, CyanFilterDecay3,
            CyanFilterDecay4, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanFilterDecay4, 3, 3) }, CyanFilterDecay3,
            CyanFilterDecay4, t3Spread);

        yield return new WireWorldTransition(new Rule[] { new(MagentaFilterDecay1, 1, 1) }, MagentaFilter,
            MagentaFilterDecay1, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaFilterDecay1, 2, 2) }, MagentaFilter,
            MagentaFilterDecay1, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaFilterDecay1, 3, 3) }, MagentaFilter,
            MagentaFilterDecay1, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaFilterDecay2, 1, 1) }, MagentaFilterDecay1,
            MagentaFilterDecay2, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaFilterDecay2, 2, 2) }, MagentaFilterDecay1,
            MagentaFilterDecay2, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaFilterDecay2, 3, 3) }, MagentaFilterDecay1,
            MagentaFilterDecay2, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaFilterDecay3, 1, 1) }, MagentaFilterDecay2,
            MagentaFilterDecay3, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaFilterDecay3, 2, 2) }, MagentaFilterDecay2,
            MagentaFilterDecay3, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaFilterDecay3, 3, 3) }, MagentaFilterDecay2,
            MagentaFilterDecay3, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaFilterDecay4, 1, 1) }, MagentaFilterDecay3,
            MagentaFilterDecay4, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaFilterDecay4, 2, 2) }, MagentaFilterDecay3,
            MagentaFilterDecay4, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaFilterDecay4, 3, 3) }, MagentaFilterDecay3,
            MagentaFilterDecay4, t3Spread);

        yield return new WireWorldTransition(new Rule[] { new(YellowFilterDecay1, 1, 1) }, YellowFilter,
            YellowFilterDecay1, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowFilterDecay1, 2, 2) }, YellowFilter,
            YellowFilterDecay1, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowFilterDecay1, 3, 3) }, YellowFilter,
            YellowFilterDecay1, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowFilterDecay2, 1, 1) }, YellowFilterDecay1,
            YellowFilterDecay2, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowFilterDecay2, 2, 2) }, YellowFilterDecay1,
            YellowFilterDecay2, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowFilterDecay2, 3, 3) }, YellowFilterDecay1,
            YellowFilterDecay2, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowFilterDecay3, 1, 1) }, YellowFilterDecay2,
            YellowFilterDecay3, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowFilterDecay3, 2, 2) }, YellowFilterDecay2,
            YellowFilterDecay3, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowFilterDecay3, 3, 3) }, YellowFilterDecay2,
            YellowFilterDecay3, t3Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowFilterDecay4, 1, 1) }, YellowFilterDecay3,
            YellowFilterDecay4, t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowFilterDecay4, 2, 2) }, YellowFilterDecay3,
            YellowFilterDecay4, t2Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowFilterDecay4, 3, 3) }, YellowFilterDecay3,
            YellowFilterDecay4, t3Spread);
    }

    private static IEnumerable<WireWorldTransition> Decay()
    {
        Func<double, bool> t1Spread = v => v < 0.001;
        yield return new WireWorldTransition(new Rule[] { new(RedHeadDecay1, 1, 2) }, WireDecay1, RedHeadDecay2,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenHeadDecay1, 1, 2) }, WireDecay1, GreenHeadDecay2,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueHeadDecay1, 1, 2) }, WireDecay1, BlueHeadDecay2,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanHeadDecay1, 1, 2) }, WireDecay1, CyanHeadDecay2,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaHeadDecay1, 1, 2) }, WireDecay1, MagentaHeadDecay2,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowHeadDecay1, 1, 2) }, WireDecay1, YellowHeadDecay2,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(WhiteHeadDecay1, 1, 2) }, WireDecay1, WhiteHeadDecay2,
            t1Spread);

        yield return new WireWorldTransition(new Rule[] { new(RedHeadDecay2, 1, 2) }, WireDecay2, RedHeadDecay3,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenHeadDecay2, 1, 2) }, WireDecay2, GreenHeadDecay3,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueHeadDecay2, 1, 2) }, WireDecay2, BlueHeadDecay3,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanHeadDecay2, 1, 2) }, WireDecay2, CyanHeadDecay3,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaHeadDecay2, 1, 2) }, WireDecay2, MagentaHeadDecay3,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowHeadDecay2, 1, 2) }, WireDecay2, YellowHeadDecay3,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(WhiteHeadDecay2, 1, 2) }, WireDecay2, WhiteHeadDecay3,
            t1Spread);

        yield return new WireWorldTransition(new Rule[] { new(RedHeadDecay3, 1, 2) }, WireDecay3, RedHeadDecay4,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(GreenHeadDecay3, 1, 2) }, WireDecay3, GreenHeadDecay4,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(BlueHeadDecay3, 1, 2) }, WireDecay3, BlueHeadDecay4,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(CyanHeadDecay3, 1, 2) }, WireDecay3, CyanHeadDecay4,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(MagentaHeadDecay3, 1, 2) }, WireDecay3, MagentaHeadDecay4,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(YellowHeadDecay3, 1, 2) }, WireDecay3, YellowHeadDecay4,
            t1Spread);
        yield return new WireWorldTransition(new Rule[] { new(WhiteHeadDecay3, 1, 2) }, WireDecay3, WhiteHeadDecay4,
            t1Spread);
    }


    private static IEnumerable<WireWorldTransition> SimpleWire(IReadOnlyList<Cell> wireTypes,
        IReadOnlyList<Cell> headTypes, IReadOnlyList<Cell> tailTypes,
        IReadOnlyList<Func<double, bool>> odds)
    {
        for (var index = 0; index < wireTypes.Count; index++)
        {
            foreach (var t in headTypes)
            {
                yield return new WireWorldTransition(new Rule[] { new(t, 1, 2) }, wireTypes[index], headTypes[index],
                    odds[index]);
            }

            yield return new WireWorldTransition(Always, headTypes[index], tailTypes[index]);
            yield return new WireWorldTransition(Always, tailTypes[index], wireTypes[index]);
        }
    }

    private static IEnumerable<WireWorldTransition> Merge()
    {
        for (var wireIndex = 0; wireIndex < Wires.Length; wireIndex++)
        {
            for (var head1Index = 0; head1Index < Wires.Length; head1Index++)
            {
                for (var head2Index = 0; head2Index < Wires.Length; head2Index++)
                {
                    yield return new WireWorldTransition(
                        new Rule[] { new(GreenHeads[head1Index], 1, 1), new(BlueHeads[head2Index], 1, 1) },
                        Wires[wireIndex], CyanHeads[wireIndex], Odds[wireIndex]);
                    yield return new WireWorldTransition(
                        new Rule[] { new(RedHeads[head1Index], 1, 1), new(BlueHeads[head2Index], 1, 1) },
                        Wires[wireIndex], MagentaHeads[wireIndex], Odds[wireIndex]);
                    yield return new WireWorldTransition(
                        new Rule[] { new(RedHeads[head1Index], 1, 1), new(GreenHeads[head2Index], 1, 1) },
                        Wires[wireIndex], YellowHeads[wireIndex], Odds[wireIndex]);
                    yield return new WireWorldTransition(
                        new Rule[] { new(RedHeads[head1Index], 1, 1), new(CyanHeads[head2Index], 1, 1) },
                        Wires[wireIndex], WhiteHeads[wireIndex], Odds[wireIndex]);
                    yield return new WireWorldTransition(
                        new Rule[] { new(GreenHeads[head1Index], 1, 1), new(MagentaHeads[head2Index], 1, 1) },
                        Wires[wireIndex], WhiteHeads[wireIndex], Odds[wireIndex]);
                    yield return new WireWorldTransition(
                        new Rule[] { new(BlueHeads[head1Index], 1, 1), new(YellowHeads[head2Index], 1, 1) },
                        Wires[wireIndex], WhiteHeads[wireIndex], Odds[wireIndex]);
                }
            }
        }
    }

    private static IEnumerable<WireWorldTransition> Filter()
    {
        for (var wireIndex = 0; wireIndex < Wires.Length; wireIndex++)
        {
            for (var headIndex = 0; headIndex < Wires.Length; headIndex++)
            {
                yield return new WireWorldTransition(new Rule[] { new(RedHeads[headIndex], 1, 2) },
                    RedFilters[wireIndex], RedFilterRedHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(GreenHeads[headIndex], 1, 2) },
                    GreenFilters[wireIndex], GreenFilterGreenHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(BlueFilters[headIndex], 1, 2) },
                    BlueFilters[wireIndex], BlueFilterBlueHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(CyanHeads[headIndex], 1, 2) },
                    CyanFilters[wireIndex], CyanFilterCyanHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(MagentaHeads[headIndex], 1, 2) },
                    MagentaFilters[wireIndex], MagentaFilterMagentaHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(YellowHeads[headIndex], 1, 2) },
                    YellowFilters[wireIndex], YellowFilterYellowHeads[wireIndex], Odds[wireIndex]);

                yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) },
                    RedFilters[wireIndex], RedFilterRedHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) },
                    GreenFilters[wireIndex], GreenFilterGreenHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) },
                    BlueFilters[wireIndex], BlueFilterBlueHeads[wireIndex], Odds[wireIndex]);

                yield return new WireWorldTransition(new Rule[] { new(RedFilterRedHeads[headIndex], 1, 2) },
                    Wires[wireIndex], RedHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(GreenFilterGreenHeads[headIndex], 1, 2) },
                    Wires[wireIndex], GreenHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(BlueFilterBlueHeads[headIndex], 1, 2) },
                    Wires[wireIndex], BlueHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(CyanFilterCyanHeads[headIndex], 1, 2) },
                    Wires[wireIndex], CyanHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(MagentaFilterMagentaHeads[headIndex], 1, 2) },
                    Wires[wireIndex], MagentaHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(YellowFilterYellowHeads[headIndex], 1, 2) },
                    Wires[wireIndex], YellowHeads[wireIndex], Odds[wireIndex]);

                yield return new WireWorldTransition(new Rule[] { new(GreenHeads[headIndex], 1, 2) },
                    CyanFilters[wireIndex], CyanFilterGreenHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(BlueHeads[headIndex], 1, 2) },
                    CyanFilters[wireIndex], CyanFilterBlueHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(MagentaHeads[headIndex], 1, 2) },
                    CyanFilters[wireIndex], CyanFilterBlueHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(YellowHeads[headIndex], 1, 2) },
                    CyanFilters[wireIndex], CyanFilterGreenHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) },
                    CyanFilters[wireIndex], CyanFilterCyanHeads[wireIndex], Odds[wireIndex]);

                yield return new WireWorldTransition(new Rule[] { new(RedHeads[headIndex], 1, 2) },
                    MagentaFilters[wireIndex], MagentaFilterRedHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(BlueHeads[headIndex], 1, 2) },
                    MagentaFilters[wireIndex], MagentaFilterBlueHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(YellowHeads[headIndex], 1, 2) },
                    MagentaFilters[wireIndex], MagentaFilterRedHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(CyanFilters[headIndex], 1, 2) },
                    MagentaFilters[wireIndex], MagentaFilterBlueHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) },
                    MagentaFilters[wireIndex], MagentaFilterMagentaHeads[wireIndex], Odds[wireIndex]);

                yield return new WireWorldTransition(new Rule[] { new(RedHeads[headIndex], 1, 2) },
                    YellowFilters[wireIndex], YellowFilterRedHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(GreenHeads[headIndex], 1, 2) },
                    YellowFilters[wireIndex], YellowFilterGreenHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(CyanHeads[headIndex], 1, 2) },
                    YellowFilters[wireIndex], YellowFilterGreenHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(MagentaHeads[headIndex], 1, 2) },
                    YellowFilters[wireIndex], YellowFilterRedHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) },
                    YellowFilters[wireIndex], YellowFilterYellowHeads[wireIndex], Odds[wireIndex]);

                yield return new WireWorldTransition(new Rule[] { new(CyanHeads[headIndex], 1, 2) },
                    GreenTails[wireIndex], BlueHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(CyanHeads[headIndex], 1, 2) },
                    BlueTails[wireIndex], GreenHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(MagentaHeads[headIndex], 1, 2) },
                    RedTails[wireIndex], BlueHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(MagentaHeads[headIndex], 1, 2) },
                    BlueTails[wireIndex], RedHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(YellowHeads[headIndex], 1, 2) },
                    RedTails[wireIndex], GreenHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(YellowHeads[headIndex], 1, 2) },
                    GreenTails[wireIndex], RedHeads[wireIndex], Odds[wireIndex]);

                yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) },
                    RedTails[wireIndex], CyanHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) },
                    GreenTails[wireIndex], MagentaHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) },
                    BlueTails[wireIndex], YellowHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) },
                    CyanTails[wireIndex], RedHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) },
                    MagentaTails[wireIndex], GreenHeads[wireIndex], Odds[wireIndex]);
                yield return new WireWorldTransition(new Rule[] { new(WhiteHeads[headIndex], 1, 2) },
                    YellowTails[wireIndex], BlueHeads[wireIndex], Odds[wireIndex]);
            }
        }
    }
}