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

namespace Pentagram.Adorners
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

		#region ctor and dispose
		public PauseAdorner(Canvas parentLayoutRoot, Chiavi chiave, Pause pause) : base(parentLayoutRoot)
		{
			_chiave = chiave;
			Pause = pause;
			Draw();
		}
		protected override void Dispose(bool isDisposing)
		{
			if (_pause != null) _pause.PropertyChanged -= OnPause_PropertyChanged;
		}
		#endregion ctor and dispose

		private void Draw()
		{
			Task upd = RunInUiThreadAsync(() =>
			{
				_layoutRoot.Children.Clear();
				if (_pause == null) return;

				// draw symbol
				BitmapImage imageSource = _minimaImage; // Pause.Duration.DurataCanonica == DurateCanoniche.Breve || Chord.Duration.DurataCanonica == DurateCanoniche.Semibreve || Chord.Duration.DurataCanonica == DurateCanoniche.Minima ? _emptyBallImage : _blackBallImage;
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

		public override double GetHeight()
		{
			return PENTAGRAM_HEIGHT;
		}

		public override double GetWidth()
		{
			if (_pause == null) throw new ArgumentNullException("PauseAdorner.GetWidth() needs a pause");
			if (_pause.Duration.PuntiDiValore == PuntiDiValore.Nil) return NOTE_BALL_WIDTH;
			else return NOTE_BALL_WIDTH + NOTE_BALL_WIDTH;
		}
	}
}
