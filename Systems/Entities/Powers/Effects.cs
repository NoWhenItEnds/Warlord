using System;

namespace Warlord.Entities.Powers.Effects
{
    public enum PowerKind
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
        TOUCH,
        PERCEPTION,     // What the user can DIRECTLY 'perceive'.
        VARIABLE        // Based upon the power's rank.
    }


    public enum EffectDuration
    {
        NONE,
        INSTANT,
        CONCENTRATION,
        CONTINUOUS,
        PERMANENT
    }


    public record Effect
    {
        public String Name { get; init; } = String.Empty;

        public PowerKind Kind { get; init; } = PowerKind.NONE;

        public EffectRange Range { get; init; } = EffectRange.NONE;

        public EffectDuration Duration { get; init; } = EffectDuration.NONE;

        public static Effect Empty => new Effect();
    }
}
