using fireMCG.PathOfLayouts.Campaign.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace fireMCG.PathOfLayouts.Campaign
{
    [CreateAssetMenu(menuName = "Path of Layouts/Campaign/Layout", fileName = "Layout_")]
    public sealed class LayoutDef : DefBase
    {
        public TagSet tags = new TagSet();

        public AssetReferenceT<Texture2D> layoutImage;

        public NavigationDataAsset navigationData;
    }
}