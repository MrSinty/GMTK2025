using UnityEngine;

public static class SpriteCombiner
{
    public static Sprite CombineSprites(Texture2D[] layers)
    {
        if (layers == null || layers.Length == 0)
            return null;

        int width = layers[0].width;
        int height = layers[0].height;

        Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, false);
        result.filterMode = FilterMode.Point;
        Color[] finalPixels = new Color[width * height];

        for (int i = 0; i < finalPixels.Length; i++)
            finalPixels[i] = new Color(0, 0, 0, 0);

        foreach (var layer in layers)
        {
            Color[] layerPixels = layer.GetPixels();
            for (int i = 0; i < finalPixels.Length; i++)
            {
                finalPixels[i] = AlphaBlend(finalPixels[i], layerPixels[i]);
            }
        }

        result.SetPixels(finalPixels);
        result.Apply();

        return Sprite.Create(result, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
    }

    private static Color AlphaBlend(Color bg, Color fg)
    {
        float alpha = fg.a + bg.a * (1 - fg.a);
        if (alpha == 0)
            return new Color(0, 0, 0, 0);

        float r = (fg.r * fg.a + bg.r * bg.a * (1 - fg.a)) / alpha;
        float g = (fg.g * fg.a + bg.g * bg.a * (1 - fg.a)) / alpha;
        float b = (fg.b * fg.a + bg.b * bg.a * (1 - fg.a)) / alpha;
        return new Color(r, g, b, alpha);
    }
}