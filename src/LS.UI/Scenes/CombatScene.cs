using System;
using SkiaSharp;

using LS.UI.Utilities;
using LS.UI.Views;
using LS.Core;

namespace LS.UI.Scenes
{
	enum TargettingType { Player, Enemy, Both }

	class CombatScene : IScene
	{
		GameController Controller;
		CombatView CombatView;

		readonly Point CombatOffset = new Point (0, 0);
		readonly Size CombatSize = new Size (1000, 720);

		public CombatScene (GameController controller)
		{
			Controller = controller;
			CombatView = new CombatView (CombatOffset, CombatSize, controller);
		}

		public void Load (string mapName)
		{
			CombatView.Load (mapName);
		}

		public void HandlePaint (SKSurface surface, GameState currentState, long frame)
		{
			surface.Canvas.Clear (SKColors.Black);

			surface.Canvas.DrawSurface (CombatView.Draw (currentState, frame), 0, 0);
		}

		public void Invalidate ()
		{
			Controller.Invalidate ();
		}

		public void OnPress (SKPointI point)
		{
		}

		public void OnDetailPress (SKPointI point)
		{
		}

		public void OnRelease (SKPointI point)
		{
		}

		public void OnDetailRelease (SKPointI point)
		{
		}

		public void HandleKeyDown (string character)
		{
			CombatView.HandleKeyDown (character);
		}
	}
}
