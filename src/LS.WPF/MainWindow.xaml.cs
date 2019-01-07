using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LS.UI;
using LS.Utilities;
using SkiaSharp;

namespace LS.WPF
{
	public partial class MainWindow : Window, IGameWindow
	{
		LS.UI.GameController Controller;
		PaintEventArgs PaintArgs = new PaintEventArgs ();
		ClickEventArgs ClickArgs = new ClickEventArgs ();
		UI.KeyEventArgs KeyArgs = new UI.KeyEventArgs ();

		public long Frame { get; private set; } = 0;

		float IGameWindow.Scale => 1;

		public event EventHandler<PaintEventArgs> OnPaint;
		public event EventHandler<ClickEventArgs> OnDetailPress;
		public event EventHandler<ClickEventArgs> OnPress;
		public new event EventHandler<UI.KeyEventArgs> OnKeyDown;
		public event EventHandler<ClickEventArgs> OnRelease;
		public event EventHandler<ClickEventArgs> OnDetailRelease;

		public event EventHandler<EventArgs> OnQuit;
		System.Windows.Media.Matrix Transform;
		System.Windows.Threading.DispatcherTimer Timer;

		public MainWindow ()
		{
			InitializeComponent ();

			Loaded += OnLoaded;
			TextInput += OnPlatformTextEnter;
			KeyDown += OnPlatformKeyDown;
			Closed += OnPlatformClose;

			SkiaView.PaintSurface += OnPlatformPaint;
		}

		public void StartAnimationTimer ()
		{
			Timer = new System.Windows.Threading.DispatcherTimer ();
			Timer.Tick += (o, e) =>
			{
				Frame++;
				Invalidate (); // This is a bit lazy				
			};
			Timer.Interval = new TimeSpan (0, 0, 0, 0, 33);
			Timer.Start ();
		}

		public void Invalidate ()
		{
			SkiaView.InvalidateVisual ();
		}

		void IGameWindow.Close ()
		{
			Application.Current.Shutdown ();
		}

		void OnPlatformPaint (object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{
			PaintArgs.Surface = e.Surface;
			OnPaint?.Invoke (this, PaintArgs);
		}

		void OnLoaded (object sender, RoutedEventArgs e)
		{
			Transform = PresentationSource.FromVisual (this).CompositionTarget.TransformToDevice;
			Controller = new GameController (this);
			Controller.Startup (new FileStorage ());
			SkiaView.InvalidateVisual ();
		}

		SKPointI GetMousePosition (System.Windows.Input.MouseButtonEventArgs e)
		{
			Point p = e.GetPosition (null);
			p = Transform.Transform (p);
			return new SKPointI ((int)p.X, (int)p.Y);
		}

		void OnPlatformMouseDown (object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			EventHandler<ClickEventArgs> currentEvent = e.ChangedButton == System.Windows.Input.MouseButton.Left ? OnPress : OnDetailPress;
			ClickArgs.Position = GetMousePosition (e);
			currentEvent?.Invoke (this, ClickArgs);
		}

		void OnPlatformMouseUp (object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			EventHandler<ClickEventArgs> currentEvent = e.ChangedButton == System.Windows.Input.MouseButton.Left ? OnRelease : OnDetailRelease;
			ClickArgs.Position = GetMousePosition (e);
			currentEvent?.Invoke (this, ClickArgs);
		}

		void OnPlatformKeyDown (object sender, System.Windows.Input.KeyEventArgs e)
		{
			string entry = e.Key.ToString ();
			if (entry.Length > 1)
			{
				KeyArgs.Character = e.Key.ToString ();
				OnKeyDown?.Invoke (this, KeyArgs);
			}
		}

		void OnPlatformTextEnter (object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			string entry = e.TextComposition.Text;
			if (entry.Length == 1 && char.IsLetter (entry [0]))
			{
				KeyArgs.Character = entry;
				OnKeyDown?.Invoke (this, KeyArgs);
			}
		}

		void OnPlatformClose (object sender, EventArgs e)
		{
			OnQuit?.Invoke (this, e);
		}
	}
}
