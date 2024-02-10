#region

using Godot;

#endregion

namespace StarLoop.Script.Datatypes;

public struct TransformDefinition
{
    public Vector3 Position { get; init; }
    public Vector3 Rotation { get; init; }
    public Vector3 Scale { get; init; }
}