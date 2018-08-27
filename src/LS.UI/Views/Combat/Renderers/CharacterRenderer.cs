using LS.Core;
using SkiaSharp;

namespace LS.UI.Views.Combat.Renderers
{
	enum CharacterStyle { Normal, Double, Square, FourByThree, ExtraLarge, ExtraLargeAndTall }

	struct CharacterStyleInfo
	{
		public static CharacterStyleInfo [] Styles = new CharacterStyleInfo [] {
			new CharacterStyleInfo (26, 36, 0, 0, 52, 72, 60, 30, -55, 26, -34, 22),
			new CharacterStyleInfo (52, 72, 0, 0, 52, 72, 60, 30, -55, 26, -34, 22),

			new CharacterStyleInfo (50, 46, 20, 15, 100, 92, 21, 152, 39, 124, 53, 120),
			new CharacterStyleInfo (94, 100, 20, 5, 94, 100, 21, 152, 39, 124, 53, 120),

			new CharacterStyleInfo (122, 114, 0, 0, 122, 114, 21, 152, 29, 127, 43, 123),
			new CharacterStyleInfo (120, 160, 10, -20, 120, 160, 21, 152, 39, 124, 53, 120)
		};

		public readonly int Width;
		public readonly int Height;
		public readonly int RenderXOffset;
		public readonly int RenderYOffset;
		public readonly int RenderWidth;
		public readonly int RenderHeight;
		public readonly int TextXOffset;
		public readonly int TextYOffset;
		public readonly int CastXOffset;
		public readonly int CastYOffset;
		public readonly int CastTextXOffset;
		public readonly int CastTextYOffset;

		public CharacterStyleInfo (int width, int height, int renderXOffset, int renderYOffset, int renderWidth, int renderHeight, int textXOffset, int textYOffset, int castXOffset, int castYOffset, int castTextXOffset, int castTextYOffset)
		{
			Width = width;
			Height = height;
			RenderXOffset = renderXOffset;
			RenderYOffset = renderYOffset;
			RenderWidth = renderWidth;
			RenderHeight = renderHeight;
			TextXOffset = textXOffset;
			TextYOffset = textYOffset;
			CastXOffset = castXOffset;
			CastYOffset = castYOffset;
			CastTextXOffset = castTextXOffset;
			CastTextYOffset = castTextYOffset;
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

		public static CharacterRenderer CreateSquare (string path, int startingID)
		{
			return Create (path, startingID, CharacterStyleInfo.Styles[(int)CharacterStyle.Square]);
		}

		public static CharacterRenderer CreateFourByThree (string path, int startingID)
		{
			return Create (path, startingID, CharacterStyleInfo.Styles[(int)CharacterStyle.FourByThree]);
		}

		public static CharacterRenderer CreateExtraLarge (string path, int startingID)
		{
			return Create (path, startingID, CharacterStyleInfo.Styles [(int)CharacterStyle.ExtraLarge]);
		}

		public static CharacterRenderer CreateExtraLargeAndTall (string path, int startingID)
		{
			return Create (path, startingID, CharacterStyleInfo.Styles[(int)CharacterStyle.ExtraLargeAndTall]);
		}

		readonly int [] FrameOffset = new int [] { 1, 0, 1, 2 };
		int CalculateAnimationOffset (long frame)
		{
			const int FramesBetweenAnimation = 6;
			return FrameOffset [(int)((frame / FramesBetweenAnimation) % 4)];
		}

		public void Render (SKCanvas canvas, GameState state, Character c, int x, int y, long frame)
		{
			DrawCharacter (canvas, x, y, frame);
			HUDRenderer.Render (canvas, state, c, x, y, frame);
		}

		void DrawCharacter (SKCanvas canvas, int x, int y, long frame)
		{
			var tilesetRect = CharacterLoader.GetRect (StartingID + CalculateAnimationOffset (frame));
			canvas.DrawBitmap (CharacterLoader.Tileset, tilesetRect, SKRect.Create (x + StyleInfo.RenderXOffset, y + StyleInfo.RenderYOffset, StyleInfo.RenderWidth, StyleInfo.RenderHeight));
		}
	}
}
