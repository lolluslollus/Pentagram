using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Pentagram.Views
{
	public sealed class PauseAdorner : Adorner
	{
		private static readonly BitmapImage _minimaImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/minima_p_100.png", UriKind.Absolute) };

		private Chiavi _chiave;

		private Pause _pause = null;
		public Pause Pause
		{
			get { return _pause; }
			private set
			{
				bool isChanged = value != _pause;
				if (isChanged)
				{
					if (_pause != null) _pause.PropertyChanged -= OnPause_PropertyChanged;
					_pause = value;
					if (_pause != null) _pause.PropertyChanged += OnPause_PropertyChanged;
				}
			}
		}

		private void OnPause_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Draw();
		}

		private readonly Canvas _layoutRoot = null;
		public PauseAdorner(Canvas layoutRoot, Chiavi chiave, Pause pause)
		{
			_layoutRoot = new Canvas() { Name = "LayoutRoot" };
			layoutRoot.Children.Add(_layoutRoot);
			_chiave = chiave;
			Pause = pause;
			Draw();
		}
		public override void Dispose()
		{
			if (_pause != null) _pause.PropertyChanged -= OnPause_PropertyChanged;
		}

		internal void Draw()
		{
			Task upd = RunInUiThreadAsync(() =>
			{
				_layoutRoot.Children.Clear();
				if (Pause == null) return;

				// draw symbol
				var imageSource = _minimaImage; // Pause.Duration.DurataCanonica == DurateCanoniche.Breve || Chord.Duration.DurataCanonica == DurateCanoniche.Semibreve || Chord.Duration.DurataCanonica == DurateCanoniche.Minima ? _emptyBallImage : _blackBallImage;
												//				var ballYs = new List<double>();

				var newBall = new Image()
				{
					Source = imageSource, //Chord.Duration.DurataCanonica == DurateCanoniche.Breve || Chord.Duration.DurataCanonica == DurateCanoniche.Semibreve || Chord.Duration.DurataCanonica == DurateCanoniche.Minima ? _emptyBallImage : _blackBallImage,
					Height = LINE_GAP
				};
				_layoutRoot.Children.Add(newBall);

				double ballY = (LINE_GAP * 3.0);
				if (_chiave == Chiavi.Violino) ballY -= LINE_GAP / 2.0;
				else if (_chiave == Chiavi.Basso) ballY -= LINE_GAP * 7.5;

				Canvas.SetTop(newBall, ballY);
				// 		<!--<Path Stroke="Black" StrokeThickness="2"
				// Data = "M24,12.5 V-80 H50 V-78 H24 V-70 H50 V-68 H24 V-60" /> -->
			});
		}
	}
}
