using LS.Core;
using SkiaSharp;

namespace LS.UI.Scenes
{
	internal interface IScene
	{
		void HandlePaint (SKSurface surface, GameState currentState, long frame);

		void OnPress (SKPointI point);
		void OnDetailPress (SKPointI point);
		void OnRelease (SKPointI point);
		void OnDetailRelease (SKPointI point);

		void HandleKeyDown (string character);

		void Invalidate ();
	}
}
