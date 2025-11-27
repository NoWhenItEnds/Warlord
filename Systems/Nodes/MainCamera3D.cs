using System;
using Godot;
using Godot.Collections;

namespace Warlord.Nodes
{
    /// <summary> The scene's camera that is used by the player. </summary>
    [GlobalClass]
    public partial class MainCamera3D : Camera3D
    {
        [ExportGroup("Settings")]
        [Export] private Single _moveSpeed = 10f;

        [Export] private Single _rotateSpeed = 10f;

        private Vector2? _queuedRaycast = null; // TODO - Make a list of tuple for callback [Func<Dictionary>, Vector2], so that multiple can queue on the same frame.

        public void Move(Vector3 position)
        {
            GlobalPosition += position * new Vector3(_moveSpeed, _moveSpeed * 3f, _moveSpeed);
        }


        public void Rotate(Single amount)
        {
            GlobalRotationDegrees += new Vector3(0f, amount, 0f) * _rotateSpeed;
        }


        public void QueueRaycast(Vector2 position) => _queuedRaycast = position;


        public override void _PhysicsProcess(Double delta)
        {
            if(_queuedRaycast != null)
            {
                Vector3 origin = ProjectRayOrigin(_queuedRaycast.Value);
                Vector3 end = origin + ProjectRayNormal(_queuedRaycast.Value) * 5000f;
                PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(origin, end);
                query.CollideWithAreas = true;
                Dictionary result = GetWorld3D().DirectSpaceState.IntersectRay(query);
                GD.Print(result);

                _queuedRaycast = null;
            }
        }
    }
}
