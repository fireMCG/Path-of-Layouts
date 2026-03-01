namespace fireMCG.PathOfLayouts.Content
{
    public static class AddressablesKeys
    {
        public static class Groups
        {
            public const string GRAPH_RENDERS = "Graph_Renders";
            public const string LAYOUT_THUMBNAILS = "Layout_Thumbnails";
            public const string LAYOUT_IMAGES = "Layout_Images";
        }

        public static class Labels
        {
            public const string AREA_GRAPH_RENDER_PREFIX = "area_graphRender__";
            public const string GRAPH_LAYOUT_THUMBNAIL_PREFIX = "graph_layoutThumbnail__";
            public const string LAYOUT_IMAGE_PREFIX = "layout_image__";

            public static string GetAreaGraphRenderLabel(string areaId)
            {
                if (string.IsNullOrWhiteSpace(areaId))
                {
                    return string.Empty;
                }

                return AREA_GRAPH_RENDER_PREFIX + areaId;
            }

            public static string GetGraphLayoutThumbnailLabel(string graphId)
            {
                if (string.IsNullOrWhiteSpace(graphId))
                {
                    return string.Empty;
                }

                return GRAPH_LAYOUT_THUMBNAIL_PREFIX + graphId;
            }

            public static string GetLayoutImageLabel(string layoutId)
            {

                if (string.IsNullOrWhiteSpace(layoutId))
                {
                    return string.Empty;
                }

                return LAYOUT_IMAGE_PREFIX + layoutId;
            }
        }

        public static class Profiles
        {
            public const string PC_LOCAL = "PC_Local";
            public const string ANDROID_CCD = "Android_CCD";
        }
    }
}