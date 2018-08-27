using SkiaSharp;
using System;
using LS.UI.Views.Combat.Utilities;

namespace LS.UI.Views.Combat.Renderers
{
	class BackgroundRenderer
	{
		public static SKSurface Render (MapLoader mapLoader, float scale)
		{
			var tilesetLoader = new TilesetLoader ($"data/maps/{mapLoader.TilesetName}.png", mapLoader.TileSize);

			int backgroundHeight = (int)Math.Round (mapLoader.MapPixelHeight * scale);
			int backgroundWidth = (int)Math.Round (mapLoader.MapPixelWidth * scale);

			var background = SKSurface.Create (new SKImageInfo (backgroundHeight, backgroundWidth));

			DrawBackgroundLayer (background, mapLoader, tilesetLoader, 0, scale);
			DrawBackgroundLayer (background, mapLoader, tilesetLoader, 1, scale);

			return background;
		}

		static void DrawBackgroundLayer (SKSurface surface, MapLoader mapLoader, TilesetLoader tilesetLoader, int index, float scale)
		{
			var terrainTiles = mapLoader.GetTiles (index);

			for (int x = 0; x < terrainTiles.GetLength (0); ++x)
			{
				for (int y = 0; y < terrainTiles.GetLength (1); ++y)
				{
					int id = terrainTiles [x, y] - 1;
					var tilesetRect = tilesetLoader.GetRect (id);
					var renderRect = GetRenderRect (x, y, mapLoader.TileSize, scale);

					surface.Canvas.DrawBitmap (tilesetLoader.Tileset, tilesetRect, renderRect);
				}
			}
		}

		static SKRect GetRenderRect (int x, int y, int tileSize, float scale)
		{
			float left = x * tileSize * scale;
			float top = y * tileSize * scale;
			float size = tileSize * scale;

			return SKRect.Create (left, top, size, size);
		}
	}
}
