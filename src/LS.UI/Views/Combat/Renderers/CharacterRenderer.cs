using LS.Core;
using SkiaSharp;

namespace LS.UI.Views.Combat.Renderers
{
	enum CharacterStyle { Normal, Double, ExtraLarge }

	struct CharacterStyleInfo
	{
		public static CharacterStyleInfo [] Styles = new CharacterStyleInfo [] {
			new CharacterStyleInfo (26, 36, 52, 72, 60, 30, -55, 16),
			new CharacterStyleInfo (52, 72, 52, 72, 60, 30, -55, 16),
			new CharacterStyleInfo (122, 114, 122, 114, 21, 152, 29, 127),
		};

		public readonly int Width;
		public readonly int Height;
		public readonly int RenderWidth;
		public readonly int RenderHeight;
		public readonly int TextXOffset;
		public readonly int TextYOffset;
		public readonly int CastXOffset;
		public readonly int CastYOffset;

		public CharacterStyleInfo (int width, int height, int renderWidth, int renderHeight, int textXOffset, int textYOffset, int castXOffset, int castYOffset)
		{
			Width = width;
			Height = height;
			RenderWidth = renderWidth;
			RenderHeight = renderHeight;
			TextXOffset = textXOffset;
			TextYOffset = textYOffset;
			CastXOffset = castXOffset;
			CastYOffset = castYOffset;
		}
	}

	class CharacterRenderer
	{
		TilesetLoader CharacterLoader;
		HUDRenderer HUDRenderer;
		int StartingID;
		CharacterStyleInfo StyleInfo;

		CharacterRenderer (TilesetLoader characterLoader, int startingID, CharacterStyleInfo styleInfo)
		{
			CharacterLoader = characterLoader;
			StartingID = startingID;
			StyleInfo = styleInfo;

			HUDRenderer = new HUDRenderer (StyleInfo);
		}

		static CharacterRenderer Create (string path, int startingID, CharacterStyleInfo style)
		{
			TilesetLoader loader = new TilesetLoader (path, style.Width, style.Height);
			return new CharacterRenderer (loader, startingID, style);
		}

		public static CharacterRenderer CreateNormalSized (string path, int startingID, bool doubleSize)
		{
			var styleInfo = doubleSize ? CharacterStyleInfo.Styles [(int)CharacterStyle.Double] : CharacterStyleInfo.Styles [(int)CharacterStyle.Normal];
			return Create (path, startingID, styleInfo);
		}

		public static CharacterRenderer CreateExtraLarge (string path, int startingID)
		{
			return Create (path, startingID, CharacterStyleInfo.Styles [(int)CharacterStyle.ExtraLarge]);
		}

		readonly int [] FrameOffset = new int [] { 1, 0, 1, 2 };
		int CalculateAnimationOffset (long frame)
		{
			const int FramesBetweenAnimation = 6;
			return FrameOffset [(int)((frame / FramesBetweenAnimation) % 4)];
		}

		public void Render (SKCanvas canvas, Character c, int x, int y, long frame)
		{
			DrawCharacter (canvas, x, y, frame);
			HUDRenderer.Render (canvas, c, x, y, frame);
		}

		void DrawCharacter (SKCanvas canvas, int x, int y, long frame)
		{
			var tilesetRect = CharacterLoader.GetRect (StartingID + CalculateAnimationOffset (frame));
			canvas.DrawBitmap (CharacterLoader.Tileset, tilesetRect, SKRect.Create (x, y, StyleInfo.RenderWidth, StyleInfo.RenderHeight));
		}
	}
}
