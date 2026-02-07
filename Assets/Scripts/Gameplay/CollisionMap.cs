using UnityEngine;

namespace fireMCG.PathOfLayouts.Gameplay
{
    public sealed class CollisionMap : MonoBehaviour
    {
        public bool IsBuilt {  get; private set; }

        public void Build(Texture2D collisionMap)
        {
            IsBuilt = true;
        }

        public void Clear()
        {
            IsBuilt = false;
        }
    }
}