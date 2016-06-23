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
	public sealed class PauseAdorner : CanvasAdorner
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

		protected override void Draw()
		{
			if (_layoutRoot == null) return;
			Task upd = RunInUiThreadAsync(() =>
			{
				if (_layoutRoot == null) return;
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

				double ballY = GetLineY(new Tone(3, NoteBianche.si, Accidenti.Nil)) - LINE_GAP_HALF;

				Canvas.SetTop(newBall, ballY);
				// 		<!--<Path Stroke="Black" StrokeThickness="2"
				// Data = "M24,12.5 V-80 H50 V-78 H24 V-70 H50 V-68 H24 V-60" /> -->
			});
		}

		public override double GetHeight()
		{
			var estimator = new PauseAdornerEstimator(_chiave, _pause);
			return estimator.GetHeight();
		}

		public override double GetWidth()
		{
			var estimator = new PauseAdornerEstimator(_chiave, _pause);
			return estimator.GetWidth();
		}
	}

	public sealed class PauseAdornerEstimator : CanvasAdornerBase
	{
		private Chiavi _chiave;
		private Pause _pause = null;

		#region ctor and dispose
		public PauseAdornerEstimator(Chiavi chiave, Pause pause)
		{
			_chiave = chiave;
			_pause = pause;
		}
		#endregion ctor and dispose

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
