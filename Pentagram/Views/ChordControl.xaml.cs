using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Utilz.Controlz;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Pentagram.Views
{
	public sealed partial class ChordControl : ObservableControl
	{
		public readonly static int HOW_MANY_WHITE_NOTES;
		public readonly static double LINE_GAP;
		public readonly static double BAR_GAP;
		public readonly static double MIN_MAST_HEIGHT;
		private static PersistentData.Duration _defaultDuration = new PersistentData.Duration(DurateCanoniche.Semibiscroma);
		private static Tone _defaultTone0 = new Tone(3, NoteBianche.@do, Accidenti.Bequadro);
		private static Tone _defaultTone1 = new Tone(3, NoteBianche.mi, Accidenti.Diesis);
		private static Tone _defaultTone2 = new Tone(3, NoteBianche.sol, Accidenti.Bemolle);
		private static Tone _defaultTone3 = new Tone(4, NoteBianche.@do, Accidenti.Bequadro);
		private static Chord _defaultChord = new Chord(_defaultDuration, SegniSuNote.Nil, _defaultTone0, _defaultTone1, _defaultTone2, _defaultTone3);
		private static readonly BitmapImage _blackBallImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/palla_nera_100.png", UriKind.Absolute) };
		private static readonly BitmapImage _emptyBallImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/palla_vuota_100.png", UriKind.Absolute) };

		static ChordControl()
		{
			LINE_GAP = (double)App.Current.Resources["LineGap"];
			BAR_GAP = (double)App.Current.Resources["FlagGap"];
			MIN_MAST_HEIGHT = (double)App.Current.Resources["MinMastHeight"];
			HOW_MANY_WHITE_NOTES = Enum.GetValues(typeof(NoteBianche)).GetLength(0);
		}

		public Chiavi Chiave
		{
			get { return (Chiavi)GetValue(ChiaveProperty); }
			set { SetValue(ChiaveProperty, value); }
		}
		public static readonly DependencyProperty ChiaveProperty =
			DependencyProperty.Register("Chiave", typeof(Chiavi), typeof(ChordControl), new PropertyMetadata(All.DEFAULT_CHIAVE));

		public Chord Chord
		{
			get { return (Chord)GetValue(ChordProperty); }
			set { SetValue(ChordProperty, value); }
		}
		public static readonly DependencyProperty ChordProperty =
			DependencyProperty.Register("Chord", typeof(Chord), typeof(ChordControl), new PropertyMetadata(null, OnChordChanged));
		private static void OnChordChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var instance = obj as ChordControl;
			//var oldValue = args.OldValue as Chord;
			//var newValue = args.NewValue as Chord;
			instance?.Update();
		}


		//private double _verticalMastScale = 5.0;
		//public double VerticalMastScale { get { return _verticalMastScale; } private set { _verticalMastScale = value; RaisePropertyChanged_UI(); } }

		//private string _pathData = "M24,12.5 V-80 H50 V-78 H24 V-70 H50 V-68 H24 V-60";
		//public string PathData { get { return _pathData; } private set { _pathData = value; RaisePropertyChanged_UI(); } }

		public ChordControl()
		{
			this.InitializeComponent();
			Update();
		}

		private void Update()
		{
			Task upd = RunInUiThreadAsync(() =>
			{
				LayoutRoot.Children.Clear();
				if (Chord == null) return;

				// set control width
				if (Chord.NextJoinedChord != null /*|| Chord.PrevJoinedChord != null*/) LayoutRoot.Width = LINE_GAP * 2.0;
				else LayoutRoot.Width = LINE_GAP * 3.0;

				// draw balls
				var ballImageSource = Chord.Duration.DurataCanonica == DurateCanoniche.Breve || Chord.Duration.DurataCanonica == DurateCanoniche.Semibreve || Chord.Duration.DurataCanonica == DurateCanoniche.Minima ? _emptyBallImage : _blackBallImage;
				var ballYs = new List<double>();

				foreach (var tone in Chord.Tones)
				{
					var newBall = new Image()
					{
						Source = ballImageSource, //Chord.Duration.DurataCanonica == DurateCanoniche.Breve || Chord.Duration.DurataCanonica == DurateCanoniche.Semibreve || Chord.Duration.DurataCanonica == DurateCanoniche.Minima ? _emptyBallImage : _blackBallImage,
						Height = LINE_GAP
					};
					LayoutRoot.Children.Add(newBall);

					double ballY = ((4.0 - Convert.ToDouble(tone.Ottava)) * HOW_MANY_WHITE_NOTES * LINE_GAP / 2.0 + LINE_GAP / 2.0 * (HOW_MANY_WHITE_NOTES - (int)tone.NotaBianca));
					if (Chiave == Chiavi.Violino) ballY -= LINE_GAP / 2.0;
					else if (Chiave == Chiavi.Basso) ballY -= LINE_GAP * 7.5;

					Canvas.SetTop(newBall, ballY);
					// 		<!--<Path Stroke="Black" StrokeThickness="2"
					// Data = "M24,12.5 V-80 H50 V-78 H24 V-70 H50 V-68 H24 V-60" /> -->

					ballYs.Add(ballY);
				}

				// draw mast and (bars or curls)
				if (GetIsNeedsMast())
				{
					var canvasTopAndMastTop = AddMast(ballYs);
					if (GetIsNeedsBars())
					{
						for (int i = 0; i < GetHowManyBarsOrCurls(); i++)
						{
							AddBar(i, canvasTopAndMastTop, LINE_GAP * 2.0);
						}
					}
					else if (GetIsNeedsCurl())
					{
						for (int i = 0; i < GetHowManyBarsOrCurls(); i++)
						{
							AddBar(i, canvasTopAndMastTop, LINE_GAP);
						}
					}
				}
			});
		}
		private bool GetIsNeedsMast()
		{
			switch (Chord.Duration.DurataCanonica)
			{
				case DurateCanoniche.Breve:
					return false;
				case DurateCanoniche.Semibreve:
					return false;
				default:
					return true;
			}

		}
		private bool GetIsNeedsBars()
		{
			switch (Chord.Duration.DurataCanonica)
			{
				case DurateCanoniche.Breve:
					return false;
				case DurateCanoniche.Semibreve:
					return false;
				case DurateCanoniche.Minima:
					return false;
				case DurateCanoniche.Semiminima:
					return false;
				default:
					return Chord.NextJoinedChord != null;
			}
		}
		private bool GetIsNeedsCurl()
		{
			switch (Chord.Duration.DurataCanonica)
			{
				case DurateCanoniche.Breve:
					return false;
				case DurateCanoniche.Semibreve:
					return false;
				case DurateCanoniche.Minima:
					return false;
				case DurateCanoniche.Semiminima:
					return false;
				default:
					return Chord.PrevJoinedChord == null && Chord.NextJoinedChord == null;
			}
		}
		private int GetHowManyBarsOrCurls()
		{
			switch (Chord.Duration.DurataCanonica)
			{
				case DurateCanoniche.Croma:
					return 1;
				case DurateCanoniche.Semicroma:
					return 2;
				case DurateCanoniche.Biscroma:
					return 3;
				case DurateCanoniche.Semibiscroma:
					return 4;
				default:
					return 0;
			}

		}

		private Tuple<double, double> AddMast(List<double> ballYs)
		{
			double maxY = ballYs.Max() + LINE_GAP / 2.0;
			double minY = ballYs.Min() + LINE_GAP / 2.0 - MIN_MAST_HEIGHT;

			var mast = new PathFigure()
			{
				StartPoint = new Point(LINE_GAP - 1.0, maxY)
			};
			mast.Segments.Add(new LineSegment() { Point = new Point(LINE_GAP - 1.0, minY) });
			var geom = new PathGeometry();
			geom.Figures.Add(mast);

			var newPath = new Windows.UI.Xaml.Shapes.Path()
			{
				Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
				StrokeThickness = 2.0,
				Data = geom
			};
			LayoutRoot.Children.Add(newPath);

			return Tuple.Create(ballYs.Min(), minY);
		}

		private void AddBar(double barNo, Tuple<double, double> canvasTopAndMastTop, double barWidth)
		{
			var bar = new PathFigure()
			{
				StartPoint = new Point(LINE_GAP - 2.0, canvasTopAndMastTop.Item2 + barNo * BAR_GAP)
			};
			bar.Segments.Add(new LineSegment() { Point = new Point(LINE_GAP - 2.0 + barWidth, canvasTopAndMastTop.Item2 + barNo * BAR_GAP) });
			var geom = new PathGeometry();
			geom.Figures.Add(bar);

			var newPath = new Windows.UI.Xaml.Shapes.Path()
			{
				Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
				StrokeThickness = 6.0,
				Data = geom
			};
			LayoutRoot.Children.Add(newPath);
		}
	}
}
