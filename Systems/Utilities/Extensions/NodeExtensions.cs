using Godot;
using System;
using System.Collections.Generic;

namespace Warlord.Utilities.Extensions
{
    /// <summary> Helper methods for working with nodes. </summary>
    public static class NodeExtensions
    {
        /// <summary> Searches children and returns all child nodes of given type (Recursive). </summary>
        /// <param name="parentNode"> Node to search from. </param>
        /// <typeparam name="T"> Type to find. </typeparam>
        /// <returns> Returns all child nodes of type T. </returns>
        public static List<T> GetAllChildComponentsByType<T>(this Node parentNode) where T : Node
        {
            List<T> childNodes = new List<T>();

            // Check the parent
            if (parentNode is T node) // If the given node is the type, add it to the list
            {
                childNodes.Add(node);
            }

            // Check the children
            Int32 allChildrenCount = parentNode.GetChildCount();
            for (Int32 i = 0; i < allChildrenCount; i++)
            {
                Node childNode = parentNode.GetChild(i);

                Int32 subChildCount = childNode.GetChildCount();
                if (subChildCount > 0) // Recursive check sub-children
                {
                    childNodes.AddRange(childNode.GetAllChildComponentsByType<T>());
                }
                else // Or just check this single node
                {
                    if (childNode is T child) // If the given node is the type, add it to the list
                    {
                        childNodes.Add(child);
                    }
                }
            }

            return childNodes;
        }


        /// <summary> Look at with constraints to not throw errors when angles are too close </summary>
        /// <param name="nodeToRotate"></param>
        /// <param name="targetGlobalPosition"></param>
        public static void SafeLookAt(this Node3D nodeToRotate, Vector3 targetGlobalPosition)
        {
            // ERR_FAIL_COND_MSG(p_pos.is_equal_approx(p_target), "Node origin and target are in the same position, look_at() failed.");
            Boolean isAligned = Vector3.Up.Cross(targetGlobalPosition - nodeToRotate.GlobalPosition).IsZeroApprox();
            Boolean isEqual = nodeToRotate.GlobalTransform.Origin == targetGlobalPosition;
            Single angleBetween = MathExtensions.AngleBetween(nodeToRotate.GlobalPosition, targetGlobalPosition);

            if (isAligned == false && isEqual == false && angleBetween > MathExtensions.SAFE_FLOAT_EPSILON)
            {
                nodeToRotate.LookAt(targetGlobalPosition, Vector3.Up);
            }
        }
    }
}
