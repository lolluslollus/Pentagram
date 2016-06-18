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
		//public readonly static double LINE_GAP_HALF;
		//public readonly static double LINE_GAP_TWICE;
		public readonly static double BAR_GAP;
		public readonly static double MIN_MAST_HEIGHT;
		private static Note _defaultNote0 = new Note(DurateCanoniche.Semibiscroma, 3, NoteBianche.@do, Accidenti.Bequadro);
		private static Note _defaultNote1 = new Note(DurateCanoniche.Semibiscroma, 3, NoteBianche.mi, Accidenti.Bequadro);
		private static Note _defaultNote2 = new Note(DurateCanoniche.Semibiscroma, 3, NoteBianche.sol, Accidenti.Bequadro);
		private static Note _defaultNote3 = new Note(DurateCanoniche.Semibiscroma, 4, NoteBianche.@do, Accidenti.Bequadro);
		private static Chord _defaultChord = new Chord(_defaultNote0, _defaultNote1, _defaultNote2, _defaultNote3);
		private static readonly BitmapImage _blackBallImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/palla_nera_100.png", UriKind.Absolute) };
		private static readonly BitmapImage _emptyBallImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/palla_vuota_100.png", UriKind.Absolute) };

		static ChordControl()
		{
			LINE_GAP = (double)App.Current.Resources["LineGap"];
			//LINE_GAP_HALF = LINE_GAP / 2.0;
			//LINE_GAP_TWICE = LINE_GAP * 2.0;
			BAR_GAP = (double)App.Current.Resources["BarGapHeight"];
			MIN_MAST_HEIGHT = (double)App.Current.Resources["MinMastHeight"];
			HOW_MANY_WHITE_NOTES = Enum.GetValues(typeof(NoteBianche)).GetLength(0);
		}

		public Chord Chord
		{
			get { return (Chord)GetValue(ChordProperty); }
			set { SetValue(ChordProperty, value); }
		}
		public static readonly DependencyProperty ChordProperty =
			DependencyProperty.Register("Chord", typeof(Chord), typeof(ChordControl), new PropertyMetadata(_defaultChord, OnChordChanged));
		private static void OnChordChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var instance = obj as ChordControl;
			//var oldValue = args.OldValue as Chord;
			//var newValue = args.NewValue as Chord;

			instance?.Update();
		}


		private double _verticalMastScale = 5.0;
		public double VerticalMastScale { get { return _verticalMastScale; } private set { _verticalMastScale = value; RaisePropertyChanged_UI(); } }

		private string _pathData = "M24,12.5 V-80 H50 V-78 H24 V-70 H50 V-68 H24 V-60";
		public string PathData { get { return _pathData; } private set { _pathData = value; RaisePropertyChanged_UI(); } }

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

				if (Chord.NextJoinedChords.Count > 0 || Chord.PrevJoinedChords.Count > 0) LayoutRoot.Width = LINE_GAP * 2.0;
				else LayoutRoot.Width = LINE_GAP * 3.0;


				// var highestNote = Chord.GetHighestNote();
				// int[] ballYs = new int[Chord.Touches.Count(tou => tou is Note)];
				var ballYsForMast = new List<double>();
				// double mastTopY;
				int howManyBars = 0;
				bool isNeedsMast = false;
				bool isNeedsBar = false;
				bool isNeedsCurl = false;

				foreach (Note note in Chord.Touches.Where(tou => tou is Note))
				{
					var newBall = new Image()
					{
						Source = note.DurataCanonica == DurateCanoniche.Breve || note.DurataCanonica == DurateCanoniche.Semibreve ? _emptyBallImage : _blackBallImage,
						Height = LINE_GAP
					};
					LayoutRoot.Children.Add(newBall);

					double ballY = ((3 - Convert.ToInt32(note.Ottava)) * HOW_MANY_WHITE_NOTES * LINE_GAP / 2.0 + LINE_GAP / 2.0 * (HOW_MANY_WHITE_NOTES - (int)note.NotaBianca));
					Canvas.SetTop(newBall, ballY);

					if (note.DurataCanonica == DurateCanoniche.Breve || note.DurataCanonica == DurateCanoniche.Semibreve) continue;
					// 		<!--<Path Stroke="Black" StrokeThickness="2"
					// Data = "M24,12.5 V-80 H50 V-78 H24 V-70 H50 V-68 H24 V-60" /> -->
					isNeedsMast = true;
					ballYsForMast.Add(ballY);
					if (note.DurataCanonica == DurateCanoniche.Minima || note.DurataCanonica == DurateCanoniche.Semiminima) continue;
					if (Chord.NextJoinedChords.Count > 0) isNeedsBar = true;
					if (Chord.PrevJoinedChords.Count == 0 && Chord.NextJoinedChords.Count == 0) isNeedsCurl = true;

					switch (note.DurataCanonica)
					{
						case DurateCanoniche.Croma:
							if (howManyBars < 1) howManyBars = 1;
							break;
						case DurateCanoniche.Semicroma:
							if (howManyBars < 2) howManyBars = 2;
							break;
						case DurateCanoniche.Biscroma:
							if (howManyBars < 3) howManyBars = 3;
							break;
						case DurateCanoniche.Semibiscroma:
							if (howManyBars < 4) howManyBars = 4;
							break;
						default:
							break;
					}
				}
				if (isNeedsMast)
				{
					var canvasTopAndMastTop = AddMast(ballYsForMast);
					if (isNeedsBar)
					{
						for (int i = 0; i < howManyBars; i++)
						{
							AddBar(i, canvasTopAndMastTop, LINE_GAP * 2.0);
						}
					}
					else if (isNeedsCurl)
					{
						for (int i = 0; i < howManyBars; i++)
						{
							AddBar(i, canvasTopAndMastTop, LINE_GAP);
						}
					}
				}
			});
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
			Canvas.SetTop(newPath, ballYs.Min());

			return Tuple.Create(ballYs.Min(), minY);
		}

		private void AddBar(double barNo, Tuple<double, double> canvasTopAndMastTop, double barWidth)
		{
			double barX0 = LINE_GAP - 2.0;
			double barX1 = barX0 + barWidth;
			var bar = new PathFigure()
			{
				StartPoint = new Point(barX0, canvasTopAndMastTop.Item2 + barNo * BAR_GAP)
			};
			bar.Segments.Add(new LineSegment() { Point = new Point(barX1, canvasTopAndMastTop.Item2 + barNo * BAR_GAP) });
			var geom = new PathGeometry();
			geom.Figures.Add(bar);

			var newPath = new Windows.UI.Xaml.Shapes.Path()
			{
				Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
				StrokeThickness = 4.0,
				Data = geom
			};
			LayoutRoot.Children.Add(newPath);
			Canvas.SetTop(newPath, canvasTopAndMastTop.Item1);
		}
	}
}
