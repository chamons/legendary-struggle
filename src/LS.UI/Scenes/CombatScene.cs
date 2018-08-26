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
		class TargettingInfo
		{
			TargettingType Type;
			Action<int> OnTargetSelected;

			public TargettingInfo (TargettingType type, Action<int> onTargetSelected)
			{
				Type = type;
				OnTargetSelected = onTargetSelected;
			}

			public void Fire (int position) => OnTargetSelected (position);
		}

		GameController Controller;
		CombatView CombatView;

		readonly Point CombatOffset = new Point (0, 0);
		readonly Size CombatSize = new Size (1000, 720);

		TargettingInfo TargetInfo;
		bool TargettingEnabled => TargetInfo != null;

		public CombatScene (GameController controller)
		{
			Controller = controller;
			CombatView = new CombatView (CombatOffset, CombatSize);
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
			switch (character)
			{
				case "D1":
				case "1":
					if (!TargettingEnabled)
						EnableTargetting (TargettingType.Player, p => Console.WriteLine ($"One - {p}"));
					break;
				case "2":
				case "D2":
					if (!TargettingEnabled)
						EnableTargetting (TargettingType.Enemy, p => Console.WriteLine ($"Two - {p}"));
					break;
				case "3":
				case "D3":
					if (!TargettingEnabled)
						EnableTargetting (TargettingType.Both, p => Console.WriteLine ($"Three - {p}"));
					break;
				case "Up":
					HandleDirection (Direction.North);
					break;
				case "Down":
					HandleDirection (Direction.South);
					break;
				case "Left":
					HandleDirection (Direction.West);
					break;
				case "Right":
					HandleDirection (Direction.East);
					break;
				case "\r":
				case "Return":
					if (TargettingEnabled)
					{
						CombatView.DisableTargetting ();
						TargetInfo.Fire (CombatView.CurrentTarget);
						TargetInfo = null;
					}
					break;
			}
		}

		void EnableTargetting (TargettingType type, Action<int> action)
		{
			TargetInfo = new TargettingInfo (type, action);
			CombatView.EnableTargetting (type);
		}

		void HandleDirection (Direction direction)
		{
			if (TargettingEnabled)
				CombatView.HandleDirection (direction);
		}
	}
}
