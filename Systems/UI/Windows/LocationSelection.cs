using System;
using Godot;
using Warlord.Entities.Nodes.Locations;
using Warlord.Managers;

namespace Warlord.UI.Windows
{
    /// <summary> A popup with information about a selected location. </summary>
    public partial class LocationSelection : Control
    {
        /// <summary> The current location the selection is tracking. </summary>
        private LocationNode? _trackedNode = null;

        /// <summary> A reference to the game world's UI manager. </summary>
        private UIManager _uiManager;

        /// <summary> A reference to the game world's camera. </summary>
        private CameraManager _cameraManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            Toggle(null);
            _uiManager = UIManager.Instance;
            _cameraManager = CameraManager.Instance;
        }


        /// <summary> Toggle the selector's target. </summary>
        /// <param name="location"> The location to follow. A null indicates to turn the selector off. </param>
        public void Toggle(LocationNode? location)
        {
            _trackedNode = location;
            Visible = location != null;
        }


        /// <inheritdoc/>
        public override void _Process(Double delta)
        {
            if(_trackedNode != null)
            {
                Vector2 nodePosition = _cameraManager.MainCamera.UnprojectPosition(_trackedNode.GlobalTransform.Origin);
                Vector2 offset = CalculateOffset(nodePosition);
                GlobalPosition = nodePosition + offset;
            }
        }


        /// <summary> Calculate the offset required to keep the element fully in view. </summary>
        /// <param name="position"> The top-left position of the element. </param>
        /// <returns> The required offset. </returns>
        private Vector2 CalculateOffset(Vector2 position)
        {
            Vector2 offset = Vector2.Zero;
            Rect2 usableRect = _uiManager.GetUsableScreen();
            Single x0 = usableRect.Position.X;
            Single y0 = usableRect.Position.Y;
            Single x1 = x0 + usableRect.Size.X;
            Single y1 = y0 + usableRect.Size.Y;

            if (position.X + Size.X > x1) { offset.X = x1 - (position.X + Size.X); }
            if (position.Y + Size.Y > y1) { offset.Y = y1 - (position.Y + Size.Y); }
            if (position.X < x0) { offset.X = Math.Abs(position.X); }
            if (position.Y < y0) { offset.Y = Math.Abs(position.Y); }

            return offset;
        }
    }
}
