using System;
using LS.Platform;
using LS.UI;
using LS.UI.Scenes;
using LS.Engine;
using LS.Model;

namespace LS.UI
{
	public class GameController
	{
		public IGameWindow GameWindow { get; }
		IScene CurrentScene;

		ILogger Log;

		GameEngine GameEngine;

		public GameController (IGameWindow gameWindow)
		{
			GameWindow = gameWindow;
			GameWindow.OnPaint += OnPaint;
			GameWindow.OnKeyDown += OnKeyDown;
			GameWindow.OnQuit += OnQuit;

			GameWindow.OnPress += OnPress;
			GameWindow.OnDetailPress += OnDetailPress;
			GameWindow.OnRelease += OnRelease;
			GameWindow.OnDetailRelease += OnDetailRelease;
		}

		GameState CurrentState => GameEngine.CurrentState;

		public void Startup (IFileStorage storage)
		{
			GameEngine = new GameEngine (storage);

			Log = Dependencies.Get<ILogger> ();

			var combatScene = new CombatScene (this);
			combatScene.Load (CurrentState.CurrentMap);
			CurrentScene = combatScene;

			GameWindow.StartAnimationTimer ();
		}

		public void Invalidate ()
		{
			GameWindow.Invalidate ();
		}

		void OnQuit (object sender, EventArgs e)
		{
		}

		void OnKeyDown (object sender, KeyEventArgs e)
		{
			CurrentScene.HandleKeyDown (e.Character);
		}

		void OnPress (object sender, ClickEventArgs e)
		{
			CurrentScene.OnPress (e.Position);
		}

		void OnDetailPress (object sender, ClickEventArgs e)
		{
			CurrentScene.OnDetailPress (e.Position);
		}

		void OnRelease (object sender, ClickEventArgs e)
		{
			CurrentScene.OnRelease (e.Position);
		}

		void OnDetailRelease (object sender, ClickEventArgs e)
		{
			CurrentScene.OnDetailRelease (e.Position);
		}

		void OnPaint (object sender, PaintEventArgs e)
		{
			CurrentState.ProcessTick (GameWindow.Frame);

			e.Surface.Canvas.Scale (GameWindow.Scale);
			CurrentScene.HandlePaint (e.Surface, CurrentState, GameWindow.Frame);
		}
	}
}