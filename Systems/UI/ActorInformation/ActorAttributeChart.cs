using Godot;
using Godot.Collections;
using System;
using Warlord.Nodes;

namespace Warlord.UI.ActorInformation
{
    /// <summary> The radar chart used to represent an actor's attribute values. </summary>
    public partial class ActorAttributeChart : Control
    {
        /// <summary> The radar chart component for the fill. </summary>
        [ExportGroup("Nodes")]
        [ExportSubgroup("Radars")]
        [Export] private RadarChart _fillChart;

        /// <summary> The radar chart component for the outline. </summary>
        [Export] private PolylineRadarChart _outlineChart;


        /// <summary> Set the chart's attribute values. </summary>
        /// <param name="strength"> The strength value as a percent (0.0 - 1.0). </param>
        /// <param name="dexterity"> The dexterity value as a percent (0.0 - 1.0). </param>
        /// <param name="vigor"> The vigor value as a percent (0.0 - 1.0). </param>
        /// <param name="intellect"> The intellect value as a percent (0.0 - 1.0). </param>
        /// <param name="presence"> The presence value as a percent (0.0 - 1.0). </param>
        public void SetAttributes(Single strength, Single dexterity, Single vigor, Single intellect, Single presence)
        {
            strength = Math.Clamp(strength,  0f, 1f);
            dexterity = Math.Clamp(dexterity, 0f, 1f);
            vigor = Math.Clamp(vigor, 0f, 1f);
            intellect = Math.Clamp(intellect, 0f, 1f);
            presence = Math.Clamp(presence, 0f, 1f);

            Array<Single> data = [strength, dexterity, vigor, intellect, presence];
            _fillChart.Data = data;
            _outlineChart.Data = data;
        }
    }
}
