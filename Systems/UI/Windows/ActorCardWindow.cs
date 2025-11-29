using Godot;
using Warlord.UI.Components;
using Warlord.Utilities;

namespace Warlord.UI.Windows
{
    public partial class ActorCardWindow : Control
    {
        /// <summary> The parent container for actor cards. </summary>
        [ExportGroup("Nodes")]
        [Export] private HBoxContainer _cardContainer;


        /// <summary> The prefab used for spawning actor cards. </summary>
        [ExportGroup("Resources")]
        [Export] private PackedScene _actorCardPrefab;


        /// <summary> The object pool for actor cards. </summary>
        private ObjectPool<ActorCard> _objectPool;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _objectPool = new ObjectPool<ActorCard>(_cardContainer, _actorCardPrefab, 10);
        }
    }
}
