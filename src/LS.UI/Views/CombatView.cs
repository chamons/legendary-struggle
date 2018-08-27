using System;
using SkiaSharp;
using LS.UI.Views.Combat.Utilities;
using LS.UI.Views.Combat.Views;
using LS.UI.Views.Combat.Renderers;
using LS.UI.Utilities;
using LS.UI.Scenes;
using LS.Core;

namespace LS.UI.Views
{
	interface ITargettingSelection
	{
		void EnableTargetting (TargettingType type);
		void DisableTargetting ();

		bool TargettingEnabled { get; }
		int CurrentTarget { get; }
	}

	class CombatView : View, ITargettingSelection
	{
		CharacterRenderCache RenderCache = new CharacterRenderCache ();

		SKSurface Background;
		string MapName;

		readonly Point LogOffset = new Point (40, 0);
		Point SkillSelectionOffset;
		const int SkillSelectionBottomMargin = 20;
		readonly Size SkillSelectionSize = new Size (350, 200);

		LogView LogView;
		TargettingView TargetView;
		SkillSelectionView SkillSelectionView;

		public CombatView (Point position, Size size, IProcessUserAction processAction) : base (position, size)
		{
			LogView = new LogView (LogOffset, new Size (size.Width - (LogOffset.X * 2), 45));
			TargetView = new TargettingView (position, size);
			SkillSelectionOffset = new Point ((size.Width / 2) - (SkillSelectionSize.Width / 2), size.Height - SkillSelectionSize.Height - SkillSelectionBottomMargin);
			SkillSelectionView = new SkillSelectionView (SkillSelectionOffset, SkillSelectionSize, this, processAction);
		}

		public void Load (string mapName)
		{
			MapName = mapName;

			var mapLoader = new MapLoader ($"data/maps/{MapName}.tmx");

			Background = BackgroundRenderer.Render (mapLoader, 2f);
		}

		public override SKSurface Draw (GameState currentState, long frame)
		{
			Clear ();

			DrawBackground ();

			foreach (Character c in currentState.AllCharacters)
			{
				Point renderPoint = CharacterRenderLocation.GetRenderPoint (currentState, c);
				RenderCache [c].Render (Canvas, currentState, c, renderPoint.X, renderPoint.Y, frame);
			}

			Canvas.DrawSurface (TargetView.Draw (currentState, frame), 0, 0);
			Canvas.DrawSurface (LogView.Draw (currentState, frame), LogOffset.X, LogOffset.Y);

			// We must do this from outside since DrawSurface doesn't do transparency the way we need
			if (SkillSelectionView.IsEnabled (currentState))
				Canvas.DrawRect (SkillSelectionView.ScreenRect, Styles.SkillSelectionBackground);
			Canvas.DrawSurface (SkillSelectionView.Draw (currentState, frame), SkillSelectionOffset.X, SkillSelectionOffset.Y);

			return Surface;
		}

		Point GetBackgroundOffset ()
		{
			switch (MapName)
			{
				case "BeachMap":
					return new Point (-205, -325);
				default:
					throw new NotImplementedException ();
			}
		}

		void DrawBackground ()
		{
			var backgroundOffset = GetBackgroundOffset ();

			Background.Draw (Canvas, backgroundOffset.X, backgroundOffset.Y, null);
		}

		public void EnableTargetting (TargettingType type) => TargetView.EnableTargetting (type);
		public void DisableTargetting () => TargetView.DisableTargetting ();
		public bool TargettingEnabled => TargetView.Enabled;
		public int CurrentTarget => TargetView.CurrentTarget;

		public override HitTestResults HitTest (SKPointI point)
		{
			return null;
		}

		public void HandleKeyDown (string character)
		{
			bool handled = false;
			if (TargettingEnabled)
				handled = TargetView.HandleKeyDown (character);

			if (!handled)
				SkillSelectionView.HandleKeyDown (character);
		}

		public void ShowSkill (string character, string name)
		{
			LogView.Show ($"{character} - {name}", 45);
		}
	}
}
