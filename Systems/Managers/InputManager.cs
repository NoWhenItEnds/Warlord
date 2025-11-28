using System;
using Godot;
using Godot.Collections;
using Warlord.Entities.Nodes;
using Warlord.Entities.Nodes.Building;
using Warlord.Utilities.Singletons;

namespace Warlord.Managers
{
    /// <summary> Translates the player's input into the game world. </summary>
    public partial class InputManager : SingletonNode<InputManager>
    {
        /// <summary> A reference to the game world's main camera. </summary>
        private CameraManager _camera;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _camera = CameraManager.Instance;
        }


        /// <inheritdoc/>
        public override void _Process(Double delta)
        {
            // --- Move the camera. ---
            Vector3 cameraMovement = new Vector3(
                Input.GetAxis("camera_move_left", "camera_move_right"),
                (Input.IsActionJustReleased("camera_zoom_out") ? 1 : 0) - (Input.IsActionJustReleased("camera_zoom_in") ? 1 : 0),
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


        /// <inheritdoc/>
        public override void _UnhandledInput(InputEvent @event)
        {
            if(@event.IsActionPressed("action_select"))
            {
                Viewport viewPort = GetViewport();
                Vector2 position = @event is InputEventMouseButton mouse ? mouse.Position : viewPort.GetVisibleRect().Size * 0.5f;
                _camera.QueueRaycast(SelectCallback, position);
            }
        }


        /// <summary> Handle the callback from a screen raycast query. </summary>
        /// <param name="result"> The raw raycast result. </param>
        private void SelectCallback(Dictionary result)
        {
            if(result.TryGetValue("collider", out Variant collider))
            {
                if (collider.Obj is ActorNode actor)
                {
                    GD.Print(actor.Name);
                }
                else if (collider.Obj is BuildingNode building)
                {
                    GD.Print(building.BuildingName);
                }
            }
        }
    }
}
