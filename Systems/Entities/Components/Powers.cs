using System;
using System.Collections.Generic;
using Warlord.Entities.Powers.Effects;

namespace Warlord.Entities.Components.Powers
{
    public class Power
    {
        public String Name { get; init; } = String.Empty;

        public String Description { get; init; } = String.Empty;

        public Effect Effect { get; init; } = Effect.Empty;

        public Dictionary<Descriptor, Int32> Descriptors = new Dictionary<Descriptor, Int32>();
    }


    public record Descriptor
    {
        public String Name { get; init; } = String.Empty;

        public PowerKind Kind { get; init; } = PowerKind.NONE;

        public Int32 Cost { get; init; } = 0;
    }
}
