using SkiaSharp;

using LS.UI.Utilities;
using LS.Core;
using System.Collections.Generic;

namespace LS.UI.Views.Combat.Views
{
	class LogView : View
	{
		class Message
		{
			public string MessageText;
			public long Duration;

			public Message (string messageText, long duration)
			{
				MessageText = messageText;
				Duration = duration;
			}
		}

		Queue<Message> Queue = new Queue<Message> ();
		Message CurrentMessage;

		TilesetLoader Loader;

		public LogView (Point position, Size size) : base (position, size)
		{
			Loader = new TilesetLoader ("data/tf_icon_32.png", 32);
		}

		public void Show (string message, int duration)
		{
			if (CurrentMessage == null)
				CurrentMessage = new Message (message, duration);
			else
				Queue.Enqueue (new Message (message, duration));
		}

		bool IsEnabled () => CurrentMessage != null;

		public override SKSurface Draw (GameState currentState, long frame)
		{
			Clear ();

			if (IsEnabled ())
			{
				var logFrameRect = SKRect.Create (SKPoint.Empty, Size.AsSKSize ());
				Canvas.DrawRoundRect (logFrameRect, 15, 20, Styles.LogBorder);
				logFrameRect.Inflate (-2, -2);
				Canvas.DrawRoundRect (logFrameRect, 15, 20, Styles.LogFill);
				Canvas.DrawText (CurrentMessage.MessageText, new SKPoint (Size.Width / 2, 6 + Size.Height / 2), Styles.LargeTextPaintCentered);

				CurrentMessage.Duration -= 1;
				if (CurrentMessage.Duration <= 0)
				{
					CurrentMessage = null;
					if (Queue.Count > 0)
						CurrentMessage = Queue.Dequeue ();
				}
			}
			else
			{
				Surface.Canvas.Clear ();
			}
			return Surface;
		}

		public override HitTestResults HitTest (SKPointI point) => null;
	}
}
