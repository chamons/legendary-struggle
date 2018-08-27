using SkiaSharp;
using System;

namespace LS.UI.Views
{
	class TilesetLoader
	{
		public SKBitmap Tileset { get; }
		public readonly int TileWidth;
		public readonly int TileHeight;
		readonly int WidthInTiles;
		readonly int HeightInTiles;

		public TilesetLoader (string path, int tileSize) : this (path, tileSize, tileSize)
		{
		}

		public TilesetLoader (string path, int tileWidth, int tileHeight)
		{
			Tileset = SKBitmap.Decode (path);
			TileWidth = tileWidth;
			TileHeight = tileHeight;
			WidthInTiles = Tileset.Width / TileWidth;
			HeightInTiles = Tileset.Height / TileHeight;
		}

		public SKRect GetRect (int id)
		{
			int column = id % WidthInTiles;
			int row = (int)Math.Floor ((double)id / (double)WidthInTiles);

			return SKRect.Create (TileWidth * column, TileHeight * row, TileWidth, TileHeight);
		}
	}
}