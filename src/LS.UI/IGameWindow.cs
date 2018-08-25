using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace LS.UI
{
	public class PaintEventArgs : EventArgs
	{
		public SKSurface Surface { get; set; }
	}

	public class ClickEventArgs : EventArgs
	{
		public SKPointI Position { get; set; }
	}

	public class KeyEventArgs : EventArgs
	{
		public string Character { get; set; }
	}

	public interface IGameWindow
	{
		void Invalidate ();
		void Close ();
		void StartAnimationTimer ();

		long Frame { get; }
		float Scale { get; }

		event EventHandler<PaintEventArgs> OnPaint;

		event EventHandler<ClickEventArgs> OnPress;
		event EventHandler<ClickEventArgs> OnDetailPress;
		event EventHandler<ClickEventArgs> OnRelease;
		event EventHandler<ClickEventArgs> OnDetailRelease;

		event EventHandler<KeyEventArgs> OnKeyDown;
		event EventHandler<EventArgs> OnQuit;
	}
}
