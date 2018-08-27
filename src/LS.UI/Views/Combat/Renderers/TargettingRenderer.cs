using SkiaSharp;

using LS.Core;
using LS.UI.Utilities;

namespace LS.UI.Views.Combat.Renderers
{
	class TargettingRenderer
	{
		TilesetLoader Loader;
		long StartingFrame = long.MinValue;

		public TargettingRenderer ()
		{
			Loader = new TilesetLoader ("data/tf_icon_32.png", 32);
		}

		public void Reset ()
		{
			StartingFrame = long.MinValue;
		}

		bool ShouldShow (long frame)
		{
			long delta = frame - StartingFrame;
			if (delta <= 9)
				return true;
			if (delta >= 18)
				Reset ();
			return false;
		}

		Point OffsetForSlot (int slot)
		{
			if (slot <= 4)
				return new Point (10, -14);
			else
				return new Point (43, 0);
		}

		public void Render (SKCanvas canvas, GameState state, Character c, int x, int y, long frame)
		{
			if (StartingFrame == long.MinValue)
				StartingFrame = frame;

			if (ShouldShow (frame))
			{
				Point offset = OffsetForSlot (c.GetSlot (state));
				SKRect bitmapRect = SKRect.Create (x + offset.X, y + offset.Y, 32, 32);
				canvas.DrawBitmap (Loader.Tileset, Loader.GetRect (26), bitmapRect, Styles.AntialiasPaint);
			}
		}
	}
}
