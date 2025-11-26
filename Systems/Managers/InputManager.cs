using System;
using Godot;
using Warlord.Nodes;
using Warlord.Utilities.Singletons;

namespace Warlord.Managers
{
    /// <summary> Translates the player's input into the game world. </summary>
    public partial class InputManager : SingletonNode<InputManager>
    {
        /// <summary> A reference to the game world's main camera. </summary>
        [ExportGroup("Nodes")]
        [Export] private MainCamera3D _camera;


        /// <inheritdoc/>
        public override void _Process(Double delta)
        {
            // Move the camera.
            Vector3 cameraMovement = new Vector3(
                Input.GetAxis("camera_move_left", "camera_move_right"),
                Input.GetAxis("camera_zoom_out", "camera_zoom_in"),
                Input.GetAxis("camera_move_up", "camera_move_down"));
            if(cameraMovement != Vector3.Zero)
            {
                _camera.Move(cameraMovement.Normalized() * (Single)delta);
            }

            Single cameraRotate = Input.GetAxis("camera_rotate_right", "camera_rotate_left");
            if(cameraRotate != 0f)
            {
                _camera.Rotate(cameraRotate * (Single)delta);
            }
        }
    }
}
