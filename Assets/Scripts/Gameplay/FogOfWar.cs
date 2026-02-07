using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace fireMCG.PathOfLayouts.Gameplay
{
    public sealed class FogOfWar : MonoBehaviour
    {
        [SerializeField] private RawImage _fogImage;
        [SerializeField] private RectTransform _fogTransform;

        [SerializeField] private Shader _fogShader;
        [SerializeField] private Shader _fogStampShader;

        [SerializeField] private int _hardBrushRadius = 60;
        [SerializeField] private int _softBrushRadius = 100;

        private void Awake()
        {
            Assert.IsNotNull(_fogImage);
            Assert.IsNotNull(_fogTransform);
            Assert.IsNotNull(_fogShader);
            Assert.IsNotNull(_fogStampShader);
        }

        public void Build(int width, int height)
        {
            _fogTransform.sizeDelta = new Vector2(width, height);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_fogTransform);
        }

        public void RevealAt(Vector2Int revealPosition)
        {

        }
    }
}