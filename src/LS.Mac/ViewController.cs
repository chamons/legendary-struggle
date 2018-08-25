using System;

using AppKit;
using CoreGraphics;
using Foundation;

using SkiaSharp;
using SkiaSharp.Views.Mac;

using LS.Core;
using LS.UI;
using LS.Utilities;

namespace ArenaLS.Mac
{
	public class CanvasView : SKCanvasView
	{
		public CanvasView (IntPtr p) : base (p)
		{
		}

		public CanvasView (CGRect r) : base (r)
		{
		}

		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
	}

	public partial class ViewController : NSViewController, INSWindowDelegate, IGameWindow
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		GameClient Client;
		PaintEventArgs PaintArgs = new PaintEventArgs ();
		ClickEventArgs ClickArgs = new ClickEventArgs ();
		KeyEventArgs KeyArgs = new KeyEventArgs ();

		public event EventHandler<PaintEventArgs> OnPaint;
		public event EventHandler<ClickEventArgs> OnDetailPress;
		public event EventHandler<ClickEventArgs> OnPress;
		public event EventHandler<KeyEventArgs> OnKeyDown;
		public event EventHandler<EventArgs> OnQuit;
		public event EventHandler<ClickEventArgs> OnRelease;
		public event EventHandler<ClickEventArgs> OnDetailRelease;

		SKCanvasView Canvas;
		NSTimer Timer;

		public long Frame { get; private set; } = 0;
		public float Scale => 1;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Client = new GameController (this);
			Controller.Startup (new FileStorage ());

			Canvas = new CanvasView (View.Frame) { IgnorePixelScaling = true };
			Canvas.PaintSurface += OnPlatformPaint;

			View.AddSubview (Canvas);

			Canvas.AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MinYMargin |
				NSViewResizingMask.MaxXMargin | NSViewResizingMask.MaxYMargin | NSViewResizingMask.HeightSizable |
				NSViewResizingMask.WidthSizable;
		}

		public override void ViewDidAppear ()
		{
			base.ViewDidAppear ();
			View.Window.Delegate = this;
		}

		[Export ("windowShouldClose:")]
		public bool WindowShouldClose (NSObject sender)
		{
			OnQuit?.Invoke (this, EventArgs.Empty);
			return true;
		}

		public override void KeyDown (NSEvent theEvent)
		{
			KeyArgs.Character = ConvertNSEventToKeyString (theEvent);
			OnKeyDown?.Invoke (this, KeyArgs);
		}

		string ConvertNSEventToKeyString (NSEvent theEvent)
		{
			switch (theEvent.KeyCode)
			{
				case (ushort)NSKey.UpArrow:
					return "Up";
				case (ushort)NSKey.DownArrow:
					return "Down";
				case (ushort)NSKey.LeftArrow:
					return "Left";
				case (ushort)NSKey.RightArrow:
					return "Right";
				case (ushort)NSKey.Keypad1:
					return "NumPad1";
				case (ushort)NSKey.Keypad2:
					return "NumPad2";
				case (ushort)NSKey.Keypad3:
					return "NumPad3";
				case (ushort)NSKey.Keypad4:
					return "NumPad4";
				case (ushort)NSKey.Keypad5:
					return "NumPad5";
				case (ushort)NSKey.Keypad6:
					return "NumPad6";
				case (ushort)NSKey.Keypad7:
					return "NumPad7";
				case (ushort)NSKey.Keypad8:
					return "NumPad8";
				case (ushort)NSKey.Keypad9:
					return "NumPad9";
				default:
					return theEvent.Characters;
			}
		}

		public override void MouseDown (NSEvent theEvent)
		{
			ClickArgs.Position = GetPositionFromEvent (theEvent);
			OnPress?.Invoke (this, ClickArgs);
		}

		public override void RightMouseDown (NSEvent theEvent)
		{
			ClickArgs.Position = GetPositionFromEvent (theEvent);
			OnDetailPress?.Invoke (this, ClickArgs);
		}

		public override void MouseUp (NSEvent theEvent)
		{
			ClickArgs.Position = GetPositionFromEvent (theEvent);
			OnRelease?.Invoke (this, ClickArgs);
		}

		public override void RightMouseUp (NSEvent theEvent)
		{
			ClickArgs.Position = GetPositionFromEvent (theEvent);
			OnDetailRelease?.Invoke (this, ClickArgs);
		}

		SKPointI GetPositionFromEvent (NSEvent theEvent)
		{
			CGPoint p = theEvent.LocationInWindow;
			return new SKPointI ((int)p.X, (int)View.Frame.Height - (int)p.Y);
		}

		public void Invalidate ()
		{
			if (Canvas != null)
				Canvas.NeedsDisplay = true;
		}

		public void Close ()
		{
			NSApplication.SharedApplication.Terminate (this);
		}

		void OnPlatformPaint (object sender, SKPaintSurfaceEventArgs e)
		{
			PaintArgs.Surface = e.Surface;
			OnPaint?.Invoke (this, PaintArgs);
		}

		public void StartAnimationTimer ()
		{
			Timer = NSTimer.CreateRepeatingScheduledTimer (new TimeSpan (0, 0, 0, 0, 33), t => {
				Frame++;
				Invalidate (); // This is a bit lazy				
			});
		}
	}
}
