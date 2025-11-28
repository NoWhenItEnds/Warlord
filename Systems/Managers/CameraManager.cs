using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Warlord.Utilities.Singletons;

namespace Warlord.Managers
{
    /// <summary> The game world's camera that is used by the player. </summary>
    public partial class CameraManager : SingletonNode3D<CameraManager>
    {
        /// <summary> The main camera. </summary>
        [ExportGroup("Nodes")]
        [Export] private Camera3D _camera;


        /// <summary> A modifier to the camera's movement speed. </summary>
        [ExportGroup("Settings")]
        [Export] private Single _moveSpeed = 10f;

        /// <summary> A modifier to the camera's rotation speed. </summary>
        [Export] private Single _rotateSpeed = 30f;

        /// <summary> The height limits that the camera can move between. </summary>
        [Export] private Vector2 _heightLimits = new Vector2(3f, 100f);

        /// <summary> The map / translate bounds that the camera can move between. </summary>
        /// <remarks> [-X, +X] [-Z, +Z] </remarks>
        [Export] private Vector4 _mapLimits = new Vector4(-100, 100, -100, 100);


        /// <summary> A list of the raycasts requested from the camera's pov. </summary>
        private List<(Action<Dictionary> callback, Vector2 position)> _queuedRaycasts = new List<(Action<Dictionary> callback, Vector2 position)>();


        /// <summary> Move the camera within the game world. </summary>
        /// <param name="position"> The relative position to modify the camera's position. </param>
        public void Move(Vector3 position)  // TODO - Fix zoom out of bounds bug.
        {
            Vector3 translation = GlobalTransform.Basis * new Vector3(position.X, 0f, position.Z) * _moveSpeed;
            if (GlobalPosition.X + translation.X >= _mapLimits.X && GlobalPosition.X + translation.X <= _mapLimits.Y &&
                GlobalPosition.Z + translation.Z >= _mapLimits.Z && GlobalPosition.Z + translation.Z <= _mapLimits.W)
            {
                GlobalPosition += translation;
            }


            Vector3 zoom = _camera.GlobalTransform.Basis.Z * position.Y * (_moveSpeed * 5f);
            if(GlobalPosition.Y + zoom.Y >= _heightLimits.X && GlobalPosition.Y + zoom.Y <= _heightLimits.Y)
            {
                GlobalPosition += zoom;
            }
        }


        /// <summary> Rotate the camera around the Y-axis. </summary>
        /// <param name="amount"> The relative amount, in degrees, to rotate the camera. </param>
        public void Rotate(Single amount)
        {
            GlobalRotationDegrees += new Vector3(0f, amount, 0f) * _rotateSpeed;
        }


        /// <summary> Queue a raycast from a position on the screen. </summary>
        /// <param name="callback"> The function to return the discovered result to. </param>
        /// <param name="position"> The screen-relative position to cast the ray from. </param>
        public void QueueRaycast(Action<Dictionary> callback, Vector2 position) => _queuedRaycasts.Add(new(callback, position));


        /// <inheritdoc/>
        public override void _PhysicsProcess(Double delta)
        {
            if(_queuedRaycasts.Count > 0)
            {
                PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
                foreach ((Action<Dictionary> callback, Vector2 position) queue in _queuedRaycasts)
                {
                    Vector3 origin = _camera.ProjectRayOrigin(queue.position);
                    Vector3 end = origin + _camera.ProjectRayNormal(queue.position) * 1000f;
                    PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(origin, end);
                    Dictionary result = spaceState.IntersectRay(query);
                    queue.callback(result);
                }
                _queuedRaycasts.Clear();
            }
        }
    }
}
