namespace fireMCG.PathOfLayouts.Content
{
    public static class AddressablesKeys
    {
        public static class Groups
        {
            public const string GRAPH_RENDERS = "Graph_Renders";
            public const string LAYOUT_IMAGES = "Layout_Images";
            public const string LAYOUT_NAVIGATION_DATA = "Layout_Navigation_Data";
        }

        public static class Labels
        {
            public const string AREA_GRAPH_RENDERS_PREFIX = "area_graphRenders__";
            public const string GRAPH_LAYOUT_IMAGES_PREFIX = "graph_layoutImages__";

            public static string GetAreaGraphRendersLabel(string areaId)
            {
                if (string.IsNullOrWhiteSpace(areaId))
                {
                    return string.Empty;
                }

                return AREA_GRAPH_RENDERS_PREFIX + areaId;
            }

            public static string GetGraphLayoutImagesLabel(string graphId)
            {
                if (string.IsNullOrWhiteSpace(graphId))
                {
                    return string.Empty;
                }

                return GRAPH_LAYOUT_IMAGES_PREFIX + graphId;
            }
        }

        public static class Profiles
        {
            public const string PC_LOCAL = "PC_Local";
            public const string ANDROID_CCD = "Android_CCD";
        }
    }
}