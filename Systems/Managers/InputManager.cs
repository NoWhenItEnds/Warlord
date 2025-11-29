using System;
using Godot;
using Godot.Collections;
using Warlord.Entities.Data;
using Warlord.Entities.Nodes;
using Warlord.Entities.Nodes.Locations;
using Warlord.Utilities.Singletons;

namespace Warlord.Managers
{
    /// <summary> Translates the player's input into the game world. </summary>
    public partial class InputManager : SingletonNode<InputManager>
    {
        /// <summary> A reference to the game world's main camera. </summary>
        private CameraManager _cameraManager;

        /// <summary> A reference to the game world's UI manager. </summary>
        private UIManager _uiManager;

        /// <summary> A reference to the game world's actor manager. </summary>
        private ActorManager _actorManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _cameraManager = CameraManager.Instance;
            _uiManager = UIManager.Instance;
            _actorManager = ActorManager.Instance;
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
                _cameraManager.Move(cameraMovement.Normalized() * (Single)delta);
            }

            Single cameraRotate = Input.GetAxis("camera_rotate_right", "camera_rotate_left");
            if(cameraRotate != 0f)
            {
                _cameraManager.Rotate(cameraRotate * (Single)delta);
            }
        }


        /// <inheritdoc/>
        public override void _UnhandledInput(InputEvent @event)
        {
            if(@event.IsActionPressed("action_select"))
            {
                Viewport viewPort = GetViewport();
                Vector2 position = @event is InputEventMouseButton mouse ? mouse.Position : viewPort.GetVisibleRect().Size * 0.5f;
                _cameraManager.QueueRaycast(SelectCallback, position);
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
                    ActorData? data = _actorManager.GetDataFromNode(actor);
                    _uiManager.ToggleActorSelection(data);
                }
                else if (collider.Obj is LocationNode building)
                {
                    _uiManager.ToggleLocationSelection(building);
                }
            }
            else
            {
                _uiManager.ToggleLocationSelection(null);
            }
        }
    }
}
