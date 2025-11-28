using System;
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


        private Vector2? _queuedRaycast = null; // TODO - Make a list of tuple for callback [Func<Dictionary>, Vector2], so that multiple can queue on the same frame.


        /// <summary> Move the camera within the game world. </summary>
        /// <param name="position"> The relative position to modify the camera's position. </param>
        public void Move(Vector3 position)
        {
            Vector3 translation = GlobalTransform.Basis * new Vector3(position.X, 0f, position.Z) * _moveSpeed;
            Vector3 zoom = _camera.GlobalTransform.Basis.Z * position.Y * (_moveSpeed * 5f);
            GlobalPosition += translation + zoom;
        }


        /// <summary> Rotate the camera around the Y-axis. </summary>
        /// <param name="amount"> The relative amount, in degrees, to rotate the camera. </param>
        public void Rotate(Single amount)
        {
            GlobalRotationDegrees += new Vector3(0f, amount, 0f) * _rotateSpeed;
        }


        public void QueueRaycast(Vector2 position) => _queuedRaycast = position;


        public override void _PhysicsProcess(Double delta)
        {
            if(_queuedRaycast != null)
            {
                Vector3 origin = _camera.ProjectRayOrigin(_queuedRaycast.Value);
                Vector3 end = origin + _camera.ProjectRayNormal(_queuedRaycast.Value) * 5000f;
                PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(origin, end);
                query.CollideWithAreas = true;
                Dictionary result = GetWorld3D().DirectSpaceState.IntersectRay(query);
                GD.Print(result);

                if(result.TryGetValue("collider", out Variant collider) && collider.Obj is GridMap map)
                {
                    if(result.TryGetValue("position", out Variant position) && position.Obj is Vector3 hitPosition)
                    {
                        GD.Print($"{hitPosition} || {map.ToLocal(hitPosition)} || {map.LocalToMap(map.ToLocal(hitPosition))}");
                        Int32 cellId = map.GetCellItem(map.LocalToMap(hitPosition));
                        GD.Print(cellId);
                    }
                }

                _queuedRaycast = null;
            }
        }
    }
}
