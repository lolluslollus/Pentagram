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
	public sealed class ChordAdorner : Adorner
	{
		public readonly static int HOW_MANY_WHITE_NOTES;
		public readonly static double LINE_GAP;
		public readonly static double FLAG_GAP;
		public readonly static double FLAG_WIDTH;
		public readonly static double MIN_MAST_HEIGHT;
		private static PersistentData.Duration _defaultDuration = new PersistentData.Duration(DurateCanoniche.Semibiscroma);
		private static Tone _defaultTone0 = new Tone(3, NoteBianche.@do, Accidenti.Bequadro);
		private static Tone _defaultTone1 = new Tone(3, NoteBianche.mi, Accidenti.Diesis);
		private static Tone _defaultTone2 = new Tone(3, NoteBianche.sol, Accidenti.Bemolle);
		private static Tone _defaultTone3 = new Tone(4, NoteBianche.@do, Accidenti.Bequadro);
		private static Chord _defaultChord = new Chord(_defaultDuration, SegniSuNote.Nil, _defaultTone0, _defaultTone1, _defaultTone2, _defaultTone3);
		private static readonly BitmapImage _blackBallImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/palla_nera_100.png", UriKind.Absolute) };
		private static readonly BitmapImage _emptyBallImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/palla_vuota_100.png", UriKind.Absolute) };

		static ChordAdorner()
		{
			LINE_GAP = (double)App.Current.Resources["LineGap"];
			FLAG_GAP = (double)App.Current.Resources["FlagGap"];
			FLAG_WIDTH = (double)App.Current.Resources["FlagWidth"];
			MIN_MAST_HEIGHT = (double)App.Current.Resources["MinMastHeight"];
			HOW_MANY_WHITE_NOTES = Enum.GetValues(typeof(NoteBianche)).GetLength(0);
		}

		private Chiavi _chiave;

		private Chord _chord = null;
		public Chord Chord
		{
			get { return _chord; }
			private set
			{
				bool isChanged = value != _chord;
				if (isChanged)
				{
					if (_chord != null) _chord.PropertyChanged -= OnChord_PropertyChanged;
					_chord = value;
					if (_chord != null) _chord.PropertyChanged += OnChord_PropertyChanged;
				}
			}
		}
		private void OnChord_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Draw();
		}

		private readonly Canvas _layoutRoot = null;

		#region ctor and dispose
		public ChordAdorner(Canvas layoutRoot, Chiavi chiave, Chord chord)
		{
			_layoutRoot = new Canvas() { Name = "LayoutRoot" };
			layoutRoot.Children.Add(_layoutRoot);
			_chiave = chiave;
			Chord = chord;
			Draw();
		}
		public override void Dispose()
		{
			if (_chord != null) _chord.PropertyChanged -= OnChord_PropertyChanged;
		}
		#endregion ctor and dispose

		#region draw
		internal void Draw()
		{
			Task upd = RunInUiThreadAsync(() =>
			{
				_layoutRoot.Children.Clear();
				if (_chord == null) return;

				// draw balls
				var ballImageSource = _chord.Duration.DurataCanonica == DurateCanoniche.Breve || _chord.Duration.DurataCanonica == DurateCanoniche.Semibreve || _chord.Duration.DurataCanonica == DurateCanoniche.Minima ? _emptyBallImage : _blackBallImage;
				var ballYs = GetBallYs(_chiave, _chord);
				int idx = 0;

				foreach (var tone in _chord.Tones)
				{
					var newBall = new Image()
					{
						Source = ballImageSource,
						Height = LINE_GAP
					};
					_layoutRoot.Children.Add(newBall);

					Canvas.SetTop(newBall, ballYs[idx]);

					//double ballY = ((4.0 - Convert.ToDouble(tone.Ottava)) * HOW_MANY_WHITE_NOTES * LINE_GAP / 2.0 + LINE_GAP / 2.0 * (HOW_MANY_WHITE_NOTES - (int)tone.NotaBianca));
					//if (_chiave == Chiavi.Violino) ballY -= LINE_GAP / 2.0;
					//else if (_chiave == Chiavi.Basso) ballY -= LINE_GAP * 7.5;

					//Canvas.SetTop(newBall, ballY);
					//// 		<!--<Path Stroke="Black" StrokeThickness="2"
					//// Data = "M24,12.5 V-80 H50 V-78 H24 V-70 H50 V-68 H24 V-60" /> -->

					//ballYs.Add(ballY);
					idx++;
				}

				// draw mast and (flags or curls)
				if (GetIsNeedsMast(_chord))
				{
					var mastTop = GetTallestMastTop(_chiave, _chord); // GetMastTop(ballYs);
					DrawMast(ballYs, mastTop);

					int howManyFlagsOrCurls = GetHowManyFlagsOrCurls(_chord);
					if (GetIsNeedsFlags(_chord))
					{
						for (int i = 1; i <= howManyFlagsOrCurls; i++)
						{
							DrawFlag(i, mastTop);//, FLAG_WIDTH * 2.0);
						}
					}
					else if (GetIsNeedsCurl(_chord))
					{
						for (int i = 1; i <= howManyFlagsOrCurls; i++)
						{
							DrawCurl(i, mastTop);//, FLAG_WIDTH);
						}
					}
				}
			});
		}

		private void DrawMast(List<double> ballYs, double minY)
		{
			double maxY = ballYs.Max() + LINE_GAP / 2.0;

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
			_layoutRoot.Children.Add(newPath);
		}

		private void DrawFlag(double flagNo, double mastTop/*, double flagWidth*/)
		{
			PathFigure flagLeft = new PathFigure() { StartPoint = new Point(LINE_GAP, mastTop + (flagNo - 1) * FLAG_GAP) };
			PathFigure flagRight = new PathFigure() { StartPoint = new Point(LINE_GAP - 2.0, mastTop + (flagNo - 1) * FLAG_GAP) };

			if (Chord.NextJoinedChord != null && Chord.PrevJoinedChord == null)
			{
				/*
				 * Se la seguente ha la barretta che sto facendo, aggiungi una barra a destra
				 * altrimenti aggiungi una mezza barretta a destra
				 */
				if (GetHowManyFlagsOrCurls(Chord.NextJoinedChord) >= flagNo)
					flagRight.Segments.Add(new LineSegment() { Point = new Point(LINE_GAP - 2.0 + 2.0 * FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
				else
					flagRight.Segments.Add(new LineSegment() { Point = new Point(LINE_GAP - 2.0 + FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
			}
			else if (Chord.PrevJoinedChord != null && Chord.NextJoinedChord == null)
			{
				/*
				 * Se la precedente ha la barretta che sto facendo, aggiungi una barra a sinistra
				 * altrimenti aggiungi una mezza barretta a sinistra
				 */
				if (GetHowManyFlagsOrCurls(Chord.PrevJoinedChord) >= flagNo)
					flagLeft.Segments.Add(new LineSegment() { Point = new Point(LINE_GAP - 2.0 * FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
				else
					flagLeft.Segments.Add(new LineSegment() { Point = new Point(LINE_GAP - FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
			}
			else
			{
				// LOLLO TODO this looks good but check it a little more
				/*
				 * Se la precedente ha la barretta che sto facendo, aggiungi una barra a sinistra
				 * altrimenti aggiungi una mezza barretta a sinistra
				 * Poi
				 * Se la seguente ha la barretta che sto facendo, aggiungi una barra a destra
				 * altrimenti no
				 * */
				if (GetHowManyFlagsOrCurls(Chord.PrevJoinedChord) >= flagNo)
					flagLeft.Segments.Add(new LineSegment() { Point = new Point(LINE_GAP - 2.0 * FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
				else
					flagLeft.Segments.Add(new LineSegment() { Point = new Point(LINE_GAP - FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
				if (GetHowManyFlagsOrCurls(Chord.NextJoinedChord) >= flagNo)
					flagRight.Segments.Add(new LineSegment() { Point = new Point(LINE_GAP - 2.0 + 2.0 * FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
			}

			var geom = new PathGeometry();
			if (flagLeft?.Segments != null && flagLeft.Segments.Count > 0) geom.Figures.Add(flagLeft);
			if (flagRight?.Segments != null && flagRight.Segments.Count > 0) geom.Figures.Add(flagRight);
			if (geom.Figures.Count < 1) return;

			var newPath = new Windows.UI.Xaml.Shapes.Path()
			{
				Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
				StrokeThickness = 6.0,
				Data = geom
			};
			_layoutRoot.Children.Add(newPath);
		}
		private void DrawCurl(double flagNo, double mastTop/*, double flagWidth*/)
		{
			var flag = new PathFigure()
			{
				StartPoint = new Point(LINE_GAP - 2.0, mastTop + (flagNo - 1) * FLAG_GAP)
			};
			flag.Segments.Add(new LineSegment() { Point = new Point(LINE_GAP - 2.0 + FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });

			var geom = new PathGeometry();
			geom.Figures.Add(flag);

			var newPath = new Windows.UI.Xaml.Shapes.Path()
			{
				Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
				StrokeThickness = 6.0,
				Data = geom
			};
			_layoutRoot.Children.Add(newPath);
		}
		#endregion draw

		#region utils
		private static List<double> GetBallYs(Chiavi chiave, Chord chord)
		{
			var ballYs = new List<double>();

			foreach (var tone in chord.Tones)
			{
				double ballY = ((4.0 - Convert.ToDouble(tone.Ottava)) * HOW_MANY_WHITE_NOTES * LINE_GAP / 2.0 + LINE_GAP / 2.0 * (HOW_MANY_WHITE_NOTES - (int)tone.NotaBianca));

				if (chiave == Chiavi.Violino) ballY -= LINE_GAP / 2.0;
				else if (chiave == Chiavi.Basso) ballY -= LINE_GAP * 7.5;

				ballYs.Add(ballY);
			}

			return ballYs;
		}
		private static bool GetIsNeedsMast(Chord chord)
		{
			switch (chord.Duration.DurataCanonica)
			{
				case DurateCanoniche.Breve:
					return false;
				case DurateCanoniche.Semibreve:
					return false;
				default:
					return true;
			}

		}
		private static bool GetIsNeedsFlags(Chord chord)
		{
			switch (chord.Duration.DurataCanonica)
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
					return chord.NextJoinedChord != null || chord.PrevJoinedChord != null;
			}
		}
		private static bool GetIsNeedsCurl(Chord chord)
		{
			switch (chord.Duration.DurataCanonica)
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
					return chord.PrevJoinedChord == null && chord.NextJoinedChord == null;
			}
		}
		private static int GetHowManyFlagsOrCurls(Chord chord)
		{
			switch (chord.Duration.DurataCanonica)
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

		private static double GetMastTop(List<double> ballYs)
		{
			return ballYs.Min() + LINE_GAP / 2.0 - MIN_MAST_HEIGHT;
		}

		private static double GetTallestMastTop(Chiavi chiave, Chord chord)
		{
			if (chord == null) throw new ArgumentException("ChordAdorner.GetTallestMastTop() needs a chord");

			var ballsYs = GetBallYs(chiave, chord);
			double mastTop = GetMastTop(ballsYs);
			double tallestMastTop = mastTop;

			Chord otherChord = chord.PrevJoinedChord;
			while (otherChord != null)
			{
				var otherBallsYs = GetBallYs(chiave, otherChord);
				double otherMastTop = GetMastTop(otherBallsYs);
				tallestMastTop = Math.Min(otherMastTop, tallestMastTop);
				otherChord = otherChord.PrevJoinedChord;
			}

			otherChord = chord.NextJoinedChord;
			while (otherChord != null)
			{
				var otherBallsYs = GetBallYs(chiave, otherChord);
				double otherMastTop = GetMastTop(otherBallsYs);
				tallestMastTop = Math.Min(otherMastTop, tallestMastTop);
				otherChord = otherChord.NextJoinedChord;
			}

			return tallestMastTop;
		}
		#endregion utils
	}
}
