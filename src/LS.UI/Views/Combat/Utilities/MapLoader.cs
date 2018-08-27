using TiledSharp;

namespace LS.UI.Views.Combat.Utilities
{
	class MapLoader
	{
		TmxMap Map;

		public readonly int TileSize;
		public int MapPixelWidth => Map.Width * Map.TileWidth;
		public int MapPixelHeight => Map.Height * Map.TileHeight;
		public string TilesetName => Map.Tilesets [0].Name;

		public MapLoader (string name)
		{
			Map = new TmxMap (name);

			if (Map.TileWidth != Map.TileHeight)
				throw new System.NotImplementedException ("MapLoader not designed for non-square tiles");
			TileSize = Map.TileWidth;
		}

		public int [,] GetTiles (int layer)
		{
			int [,] tiles = new int [Map.Width, Map.Height];
			foreach (var tile in Map.Layers [layer].Tiles)
				tiles [tile.X, tile.Y] = tile.Gid;
			return tiles;
		}
	}
}
