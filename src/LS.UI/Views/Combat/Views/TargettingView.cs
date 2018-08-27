using SkiaSharp;

using LS.Core;
using LS.UI.Scenes;
using LS.UI.Utilities;
using LS.UI.Views.Combat.Renderers;
using LS.UI.Views.Combat.Utilities;

namespace LS.UI.Views.Combat.Views
{
	class TargettingView : View
	{
		TargettingRenderer TargetRender;
		public bool Enabled { get; private set; }
		public int CurrentTarget { get; private set; }
		GameState GameState;

		TargettingType Type;

		public TargettingView (Point position, Size size) : base (position, size)
		{
			TargetRender = new TargettingRenderer ();
		}		

		public void EnableTargetting (TargettingType type)
		{
			Enabled = true;
			Type = type;
			SetDefaultTarget ();
		}

		public void DisableTargetting ()
		{
			Enabled = false;
			TargetRender.Reset ();
		}

		void SetDefaultTarget ()
		{
			switch (Type)
			{
				case TargettingType.Player:
					CurrentTarget = 0;
					break;
				case TargettingType.Enemy:
				case TargettingType.Both:
					CurrentTarget = 5;
					return;
			}
		}

		public override SKSurface Draw (GameState currentState, long frame)
		{
			Clear ();
			GameState = currentState;

			if (Enabled)
			{
				Character character = null;
				if (CurrentTarget > 4)
					character = GameState.Enemies.ElementOrDefault (CurrentTarget - 5);
				else
					character = GameState.Party.ElementOrDefault (CurrentTarget);

				if (character != null)
				{
					Point renderLocation = CharacterRenderLocation.GetRenderPoint (GameState, character);
					TargetRender.Render (Canvas, GameState, character, renderLocation.X, renderLocation.Y, frame);
				}
			}
			return Surface;
		}

		void HandleDirection (Direction direction)
		{
			TargetRender.Reset ();

			switch (Type)
			{
				case TargettingType.Player:
					HandleDirectionVertical (direction, 0, GameState.Party.Length - 1);
					break;
				case TargettingType.Enemy:
					// MultipleEnemies
					HandleDirectionVertical (direction, 5, 5 + GameState.Enemies.Length - 1);
					break;
				case TargettingType.Both:
					switch (direction)
					{
						case Direction.North:
						case Direction.South:
							if (CurrentTarget <= 4)
								HandleDirectionVertical (direction, 0, GameState.Party.Length - 1);
							else
								HandleDirectionVertical (direction, 5, 5 + GameState.Enemies.Length - 1);
							break;
						case Direction.East:
							CurrentTarget = 0;
								break;
						case Direction.West:
							// MultipleEnemies
							CurrentTarget = 5;
							break;
					}
					return;
			}
		}

		void HandleDirectionVertical (Direction direction, int min, int max)
		{
			switch (direction)
			{
				case Direction.North:
					CurrentTarget--;
					if (CurrentTarget < min)
						CurrentTarget = min;
					return;
				case Direction.South:
					CurrentTarget++;
					if (CurrentTarget > max)
						CurrentTarget = max;
					return;
			}
		}

		public override HitTestResults HitTest (SKPointI point)
		{
			return null;
		}

		public bool HandleKeyDown (string character)
		{
			switch (character)
			{
				case "Up":
					HandleDirection (Direction.North);
					return true;
				case "Down":
					HandleDirection (Direction.South);
					return true;
				case "Left":
					HandleDirection (Direction.West);
					return true;
				case "Right":
					HandleDirection (Direction.East);
					return true;
			}
			return false;
		}
	}
}
