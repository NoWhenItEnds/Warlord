using System;
using Godot;
using Warlord.Entities.Nodes.Locations;
using Warlord.Entities.Resources;
using Warlord.Managers;
using Warlord.Organisations;
using Warlord.Organisations.Objectives;

namespace Warlord.UI.Windows
{
    /// <summary> A popup with information about a selected location. </summary>
    public partial class LocationSelection : Control
    {
        /// <summary> The label to show the name of the location. </summary>
        [ExportGroup("Nodes")]
        [ExportSubgroup("Labels")]
        [Export] private RichTextLabel _nameLabel;

        /// <summary> The button to tag a location to be garrisoned. </summary>
        [ExportSubgroup("Buttons")]
        [Export] private Button _garrisonButton;

        /// <summary> The button to tag a location to be assaulted. </summary>
        [Export] private Button _assaultButton;


        /// <summary> The current location data the selection is tracking. </summary>
        private LocationData? _trackedLocation = null;

        /// <summary> The current location node the selection is tracking. </summary>
        private LocationNode? _trackedNode = null;

        /// <summary> A reference to the game world's UI manager. </summary>
        private UIManager _uiManager;

        /// <summary> A reference to the game world's camera. </summary>
        private CameraManager _cameraManager;

        /// <summary> A reference to the game's location manager. </summary>
        private LocationManager _locationManager;

        /// <summary> A reference to the game's organisation manager. </summary>
        private OrganisationManager _organisationManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            Toggle(null);

            _uiManager = UIManager.Instance;
            _cameraManager = CameraManager.Instance;
            _locationManager = LocationManager.Instance;
            _organisationManager = OrganisationManager.Instance;

            _garrisonButton.ButtonDown += OnGarrisonButton;
        }


        /// <summary> Toggle the selector's target. </summary>
        /// <param name="location"> The location to follow. A null indicates to turn the selector off. </param>
        public void Toggle(LocationData? location)
        {
            _trackedLocation = location;

            if(location != null)
            {
                _locationManager.TryGetNode(location, out _trackedNode);
            }
            else
            {
                _trackedNode = null;
            }

            Visible = _trackedNode != null;
        }


        /// <inheritdoc/>
        public override void _Process(Double delta)
        {
            UpdateUI();
            if (_trackedNode != null)
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


        /// <summary> Update all the UI elements. </summary>
        private void UpdateUI()
        {
            if(_trackedNode != null)
            {
                _nameLabel.Text = _trackedNode.Data.Name;
            }
        }


        /// <summary> Send a organisation objective to garrison the selected location. </summary>
        private void OnGarrisonButton()
        {
            if (_trackedLocation != null)
            {
                _organisationManager.PlayerController.AddObjective(new GarrisonObjective(_trackedLocation));
            }
        }


        /// <inheritdoc/>
        public override void _ExitTree()
        {
            _garrisonButton.ButtonDown -= OnGarrisonButton;
        }
    }
}
