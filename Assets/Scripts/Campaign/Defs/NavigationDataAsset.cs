using fireMCG.PathOfLayouts.Campaign.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace fireMCG.PathOfLayouts.Campaign
{
    [CreateAssetMenu(menuName = "Path of Layouts/Campaign/Navigation Data Asset", fileName = "NavData_")]
    public sealed class NavigationDataAsset : ScriptableObject
    {
        [Min(1)] public int width = 1;
        [Min(1)] public int height = 1;

        public float scaleModifier;

        public List<NavigationNode> nodes = new();

        [Tooltip("Indices into nodes[]. Can repeat indices.")]
        public List<int> visitOrder = new();

        [Tooltip("1 bit per cell where 1 = walkable and 0 = blocked.")]
        [SerializeField] private byte[] _walkableBits = Array.Empty<byte>();

        public bool IsWalkable(int x, int y)
        {
            if((uint)x >= (uint)width || (uint)y >= (uint)height)
            {
                return false;
            }

            int cellIndex = y * width + x;
            int byteIndex = cellIndex >> 3; // /8
            int bitIndex = cellIndex & 7;   // %8

            if(_walkableBits == null || byteIndex < 0 || byteIndex >= _walkableBits.Length)
            {
                return false;
            }

            return ((_walkableBits[byteIndex] >> bitIndex) & 1) == 1;
        }

        public void SetPackedWalkableBits(int newWidth, int newHeight, byte[] packedBits)
        {
            if(newWidth < 1 || newHeight < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            width = newWidth;
            height = newHeight;
            _walkableBits = packedBits ?? Array.Empty<byte>();
        }

        public ReadOnlySpan<byte> GetPackedBits() => _walkableBits;
    }
}