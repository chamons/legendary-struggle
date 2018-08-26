using SkiaSharp;

using LS.Core;
using LS.UI.Utilities;

namespace LS.UI.Views
{
	class HitTestResults
	{
		internal View View { get; }
		internal object Data { get; }

		internal HitTestResults (View view, object data)
		{
			View = view;
			Data = data;
		}
	}

	abstract class View
	{
		public Point Position { get; protected set; }
		public Size Size { get; protected set; }
		public SKRect VisualRect => new SKRect (0, 0, Size.Width, Size.Height);
		public SKRect ScreenRect => new SKRect (Position.X, Position.Y, Position.X + Size.Width, Position.Y + Size.Height);

		protected View (Point position, Size size)
		{
			Position = position;
			Size = size;
			Surface = SKSurface.Create (Size.Width, Size.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
		}

		protected SKSurface Surface { get; private set; }
		protected SKCanvas Canvas => Surface.Canvas;

		protected void Clear () => Canvas.Clear ();

		public abstract SKSurface Draw (GameState currentState, long frame);
		public abstract HitTestResults HitTest (SKPointI point);		
	}
}
