using Godot;
using System;

namespace Warlord.Entities.Components.Powers
{
    public record Power
    {
        public String Name { get; init; } = String.Empty;

        public String Description { get; init; } = String.Empty;

        public Effect Effect { get; init; } = Effect.Empty;
    }

    public record Effect
    {
        public String Id { get; init; } = String.Empty;

        public EffectKind Kind { get; init; } = EffectKind.NONE;

        public EffectRange Range { get; init; } = EffectRange.NONE;

        public EffectDuration Duration { get; init; } = EffectDuration.NONE;


        public static Effect Empty => new Effect();
    }

    public enum EffectKind
    {
        NONE,
        OFFENCE,
        DEFENCE,
        MOVEMENT,
        SENSORY,
        CONTROL
    }


    public enum EffectRange
    {
        NONE,
        PERSONAL,
        CLOSE,
        PERCEPTION,     // What the user can DIRECTLY 'perceive'.
        VARIABLE        // Based upon rank.
    }


    public enum EffectDuration
    {
        NONE,
        INSTANT,
        SUSTAINED,
        PERMANENT,
        VARIABLE        // Based upon rank.
    }
}
