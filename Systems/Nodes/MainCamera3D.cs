using System;
using Godot;

namespace Warlord.Nodes
{
    /// <summary> The scene's camera that is used by the player. </summary>
    [GlobalClass]
    public partial class MainCamera3D : Camera3D
    {
        [ExportGroup("Settings")]
        [Export] private Single _moveSpeed = 10f;

        [Export] private Single _rotateSpeed = 10f;

        public void Move(Vector3 position)
        {
            GlobalPosition += position * _moveSpeed;
        }


        public void Rotate(Single amount)
        {
            GlobalRotationDegrees += new Vector3(0f, amount, 0f) * _rotateSpeed;
        }
    }
}
