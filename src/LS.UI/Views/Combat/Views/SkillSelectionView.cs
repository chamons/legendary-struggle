using System;
using SkiaSharp;

using LS.UI.Utilities;
using LS.Core;

namespace LS.UI.Views.Combat.Views
{
	class SkillSelectionView : View
	{
		TilesetLoader Loader;
		int Selected = 0;
		ITargettingSelection Targetting;
		IProcessUserAction ProcessAction;
		GameState State;

		public SkillSelectionView (Point position, Size size, ITargettingSelection targetting, IProcessUserAction processAction) : base (position, size)
		{
			Targetting = targetting;
			ProcessAction = processAction;
			Loader = new TilesetLoader ("data/tf_icon_32.png", 32);
		}

		public bool IsEnabled (GameState state)
		{
			Character activeCharacter = state.ActiveCharacter;
			return activeCharacter != null && activeCharacter.CT == Time.ActionAmount;
		}

		public override SKSurface Draw (GameState currentState, long frame)
		{
			State = currentState;
			Canvas.Clear (SKColors.Transparent);

			if (IsEnabled (currentState))
			{
				Character activeCharacter = currentState.ActiveCharacter;
				for (int i = 0; i < activeCharacter.Skills.Length; ++i)
				{
					Skill s = activeCharacter.Skills[i];
					int y = 50 + (25 * i);
					if (i == Selected)
					{
						SKRect bitmapRect = SKRect.Create (15, y - 22, 32, 32);
						Canvas.DrawBitmap (Loader.Tileset, Loader.GetRect (296), bitmapRect, Styles.AntialiasPaint);
					}
					Canvas.DrawText (s.CosmeticName, new SKPoint (50, y), s.Available ? Styles.TextPaint : Styles.DarkTextPaint);
				}
			}

			return Surface;
		}

		public override HitTestResults HitTest (SKPointI point)
		{
			return null;
		}

		public void HandleKeyDown (string character)
		{
			if (!IsEnabled (State))
				return;

			switch (character)
			{
				case "Up":
					if (!Targetting.TargettingEnabled)
					{
						Selected -= 1;
						Selected = Selected.Clamp (0, State.ActiveCharacter.Skills.Length - 1);
					}
					break;
				case "Down":
					if (!Targetting.TargettingEnabled)
					{
						Selected += 1;
						Selected = Selected.Clamp (0, State.ActiveCharacter.Skills.Length - 1);
					}
					break;
				case "\r":
				case "Return":
					if (Targetting.TargettingEnabled)
					{
						long targetID = State.GetCharacterFromSlot (Targetting.CurrentTarget).ID;

						Targetting.DisableTargetting ();
						ProcessAction.ProcessAction (Selected, targetID);
						Selected = 0;
					}
					else
					{
						Targetting.EnableTargetting (Scenes.TargettingType.Both);
					}
					break;
			}
		}
	}
}