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
	public sealed class ChordAdorner : Adorner
	{
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
					if (_chord != null)
					{
						_chord.PropertyChanged -= OnChord_PropertyChanged;
						_chord.ChromaFlagsPositionChanged -= OnChord_ChromaFlagsPositionChanged;
					}
					_chord = value;
					if (_chord != null)
					{
						_chord.PropertyChanged += OnChord_PropertyChanged;
						_chord.ChromaFlagsPositionChanged += OnChord_ChromaFlagsPositionChanged;
					}
				}
			}
		}
		private void OnChord_ChromaFlagsPositionChanged(object sender, EventArgs e)
		{
			Draw();
		}
		private void OnChord_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Chord.IsChromaFlagsBelow)) return;
			Draw();
		}

		#region ctor and dispose
		static ChordAdorner()
		{
			FLAG_GAP = (double)App.Current.Resources["FlagGap"];
			FLAG_WIDTH = (double)App.Current.Resources["NoteFlagWidth"];
			MIN_MAST_HEIGHT = (double)App.Current.Resources["MinMastHeight"];
		}
		public ChordAdorner(Canvas parentLayoutRoot, Chiavi chiave, Chord chord) : base(parentLayoutRoot)
		{
			_chiave = chiave;
			Chord = chord;
			Draw();
		}
		protected override void Dispose(bool isDisposing)
		{
			if (_chord != null) _chord.PropertyChanged -= OnChord_PropertyChanged;
		}
		#endregion ctor and dispose

		private void Draw()
		{
			Task upd = RunInUiThreadAsync(() =>
			{
				_layoutRoot.Children.Clear();
				if (_chord == null) return;

				LinkedChromasHelper helper = _chord.IsChromaFlagsBelow
					? new LinkedChromaHelper_Below(_chord, _layoutRoot) as LinkedChromasHelper
					: new LinkedChromaHelper_Above(_chord, _layoutRoot) as LinkedChromasHelper;
				//LinkedChromasHelper helper = null;
				//if (_chord.IsChromaFlagsBelow) helper = new LinkedChromaHelper_Below(); else helper = new LinkedChromaHelper_Above();
				// draw balls
				var ballImageSource = _chord.Duration.DurataCanonica == DurateCanoniche.Breve || _chord.Duration.DurataCanonica == DurateCanoniche.Semibreve || _chord.Duration.DurataCanonica == DurateCanoniche.Minima ? _emptyBallImage : _blackBallImage;
				var ballYs = LinkedChromasHelper.GetBallYs(_chiave, _chord);
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
					// 		<!--<Path Stroke="Black" StrokeThickness="2"
					// Data = "M24,12.5 V-80 H50 V-78 H24 V-70 H50 V-68 H24 V-60" /> -->

					idx++;
				}

				// draw mast and (flags or curls)
				if (LinkedChromasHelper.GetIsNeedsMast(_chord))
				{
					var mastTop = helper.GetTallestMastTop(_chiave, _chord);
					helper.DrawMast(ballYs, mastTop);

					int howManyFlagsOrCurls = LinkedChromasHelper.GetHowManyFlagsOrCurls(_chord);
					if (LinkedChromasHelper.GetIsNeedsFlags(_chord))
					{
						for (int i = 1; i <= howManyFlagsOrCurls; i++)
						{
							helper.DrawFlag(i, mastTop);
						}
					}
					else if (LinkedChromasHelper.GetIsNeedsCurl(_chord))
					{
						for (int i = 1; i <= howManyFlagsOrCurls; i++)
						{
							helper.DrawCurl(i, mastTop);
						}
					}
				}
			});
		}

		public override double GetHeight()
		{
			return PENTAGRAM_HEIGHT;
		}

		public override double GetWidth()
		{
			if (_chord == null) throw new ArgumentNullException("ChordAdorner.GetWidth() needs a chord");
			if (_chord.Duration.PuntiDiValore == PuntiDiValore.Nil) return NOTE_BALL_WIDTH + FLAG_WIDTH;
			else return NOTE_BALL_WIDTH + NOTE_BALL_WIDTH + FLAG_WIDTH;
		}

		#region utils
		internal abstract class LinkedChromasHelper
		{
			protected readonly Chord _chord = null;
			protected readonly Canvas _layoutRoot = null;
			internal LinkedChromasHelper(Chord chord, Canvas layoutRoot)
			{
				if (chord == null) throw new ArgumentNullException("LinkedChromasHelper needs a chord");
				if (layoutRoot == null) throw new ArgumentNullException("LinkedChromasHelper needs a canvas");
				_chord = chord;
				_layoutRoot = layoutRoot;
			}
			internal static List<double> GetBallYs(Chiavi chiave, Chord chord)
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
			internal static bool GetIsNeedsMast(Chord chord)
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
			internal static bool GetIsNeedsFlags(Chord chord)
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
						return (chord.NextJoinedChord != null && chord.NextJoinedChord.IsChromaFlagsBelow == chord.IsChromaFlagsBelow)
							|| (chord.PrevJoinedChord != null && chord.PrevJoinedChord.IsChromaFlagsBelow == chord.IsChromaFlagsBelow);
				}
			}
			internal static bool GetIsNeedsCurl(Chord chord)
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
						return !GetIsNeedsFlags(chord);
				}
			}
			internal static int GetHowManyFlagsOrCurls(Chord chord)
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
			internal abstract double GetTallestMastTop(Chiavi chiave, Chord chord);
			internal abstract void DrawMast(List<double> ballYs, double minY);
			internal abstract void DrawFlag(double flagNo, double mastTop/*, double flagWidth*/);
			internal abstract void DrawCurl(double flagNo, double mastTop/*, double flagWidth*/);
		}

		private class LinkedChromaHelper_Above : LinkedChromasHelper
		{
			internal LinkedChromaHelper_Above(Chord chord, Canvas layoutRoot) : base(chord, layoutRoot) { }
			private static double GetMastTop(List<double> ballYs)
			{
				return ballYs.Min() + LINE_GAP / 2.0 - MIN_MAST_HEIGHT;
			}
			internal override double GetTallestMastTop(Chiavi chiave, Chord chord)
			{
				if (chord == null) throw new ArgumentException("LinkedChromaHelper_Above.GetTallestMastTop() needs a chord");

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
			internal override void DrawMast(List<double> ballYs, double flagY)
			{
				double ballY = ballYs.Max() + LINE_GAP / 2.0;

				var mast = new PathFigure()
				{
					StartPoint = new Point(NOTE_BALL_WIDTH - 1.0, ballY)
				};
				mast.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH - 1.0, flagY) });
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
			internal override void DrawFlag(double flagNo, double mastTop)
			{
				PathFigure flagLeft = new PathFigure() { StartPoint = new Point(NOTE_BALL_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) };
				PathFigure flagRight = new PathFigure() { StartPoint = new Point(NOTE_BALL_WIDTH - 2.0, mastTop + (flagNo - 1) * FLAG_GAP) };

				if (_chord.NextJoinedChord != null && _chord.PrevJoinedChord == null)
				{
					/*
					 * Se la seguente ha la barretta che sto facendo, aggiungi una barra a destra
					 * altrimenti aggiungi una mezza barretta a destra
					 */
					if (GetHowManyFlagsOrCurls(_chord.NextJoinedChord) >= flagNo)
						flagRight.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH - 2.0 + 2.0 * FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
					else
						flagRight.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH - 2.0 + FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
				}
				else if (_chord.PrevJoinedChord != null && _chord.NextJoinedChord == null)
				{
					/*
					 * Se la precedente ha la barretta che sto facendo, aggiungi una barra a sinistra
					 * altrimenti aggiungi una mezza barretta a sinistra
					 */
					if (GetHowManyFlagsOrCurls(_chord.PrevJoinedChord) >= flagNo)
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH - 2.0 * FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
					else
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH - FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
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
					if (GetHowManyFlagsOrCurls(_chord.PrevJoinedChord) >= flagNo)
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH - 2.0 * FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
					else
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH - FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
					if (GetHowManyFlagsOrCurls(_chord.NextJoinedChord) >= flagNo)
						flagRight.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH - 2.0 + 2.0 * FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });
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
			internal override void DrawCurl(double flagNo, double mastTop)
			{
				var flag = new PathFigure()
				{
					StartPoint = new Point(NOTE_BALL_WIDTH - 2.0, mastTop + (flagNo - 1) * FLAG_GAP)
				};
				flag.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH - 2.0 + FLAG_WIDTH, mastTop + (flagNo - 1) * FLAG_GAP) });

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
		}

		private class LinkedChromaHelper_Below : LinkedChromasHelper
		{
			internal LinkedChromaHelper_Below(Chord chord, Canvas layoutRoot) : base(chord, layoutRoot) { }
			private static double GetMastTop(List<double> ballYs)
			{
				return ballYs.Max() + LINE_GAP / 2.0 + MIN_MAST_HEIGHT;
			}
			internal override double GetTallestMastTop(Chiavi chiave, Chord chord)
			{
				if (chord == null) throw new ArgumentException("LinkedChromaHelper_Below.GetTallestMastTop() needs a chord");

				var ballsYs = GetBallYs(chiave, chord);
				double mastTop = GetMastTop(ballsYs);
				double tallestMastTop = mastTop;

				Chord otherChord = chord.PrevJoinedChord;
				while (otherChord != null)
				{
					var otherBallsYs = GetBallYs(chiave, otherChord);
					double otherMastTop = GetMastTop(otherBallsYs);
					tallestMastTop = Math.Max(otherMastTop, tallestMastTop);
					otherChord = otherChord.PrevJoinedChord;
				}

				otherChord = chord.NextJoinedChord;
				while (otherChord != null)
				{
					var otherBallsYs = GetBallYs(chiave, otherChord);
					double otherMastTop = GetMastTop(otherBallsYs);
					tallestMastTop = Math.Max(otherMastTop, tallestMastTop);
					otherChord = otherChord.NextJoinedChord;
				}

				return tallestMastTop;
			}
			internal override void DrawMast(List<double> ballYs, double flagY)
			{
				double ballY = ballYs.Min() + LINE_GAP / 2.0;

				var mast = new PathFigure()
				{
					StartPoint = new Point(1.0, ballY)
				};
				mast.Segments.Add(new LineSegment() { Point = new Point(1.0, flagY) });
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
			internal override void DrawFlag(double flagNo, double mastTop)
			{
				PathFigure flagLeft = new PathFigure() { StartPoint = new Point(2.0, mastTop - (flagNo - 1) * FLAG_GAP) };
				PathFigure flagRight = new PathFigure() { StartPoint = new Point(0.0, mastTop - (flagNo - 1) * FLAG_GAP) };

				if (_chord.NextJoinedChord != null && _chord.PrevJoinedChord == null)
				{
					/*
					 * Se la seguente ha la barretta che sto facendo, aggiungi 1 barra a destra
					 * altrimenti aggiungi una mezza barretta a destra
					 */
					if (GetHowManyFlagsOrCurls(_chord.NextJoinedChord) >= flagNo)
						flagRight.Segments.Add(new LineSegment() { Point = new Point(2.0 * FLAG_WIDTH, mastTop - (flagNo - 1) * FLAG_GAP) });
					else
						flagRight.Segments.Add(new LineSegment() { Point = new Point(FLAG_WIDTH, mastTop - (flagNo - 1) * FLAG_GAP) });
				}
				else if (_chord.PrevJoinedChord != null && _chord.NextJoinedChord == null)
				{
					/*
					 * Se la precedente ha la barretta che sto facendo, aggiungi una barra a sinistra
					 * altrimenti aggiungi una mezza barretta a sinistra
					 */
					if (GetHowManyFlagsOrCurls(_chord.PrevJoinedChord) >= flagNo)
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(2.0 - 2.0 * FLAG_WIDTH, mastTop - (flagNo - 1) * FLAG_GAP) });
					else
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(2.0 - FLAG_WIDTH, mastTop - (flagNo - 1) * FLAG_GAP) });
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
					if (GetHowManyFlagsOrCurls(_chord.PrevJoinedChord) >= flagNo)
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(2.0 - 2.0 * FLAG_WIDTH, mastTop - (flagNo - 1) * FLAG_GAP) });
					else
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(2.0 - FLAG_WIDTH, mastTop - (flagNo - 1) * FLAG_GAP) });
					if (GetHowManyFlagsOrCurls(_chord.NextJoinedChord) >= flagNo)
						flagRight.Segments.Add(new LineSegment() { Point = new Point(2.0 * FLAG_WIDTH, mastTop - (flagNo - 1) * FLAG_GAP) });
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
			internal override void DrawCurl(double flagNo, double mastTop)
			{
				var flag = new PathFigure()
				{
					StartPoint = new Point(0.0, mastTop - (flagNo - 1) * FLAG_GAP)
				};
				flag.Segments.Add(new LineSegment() { Point = new Point(FLAG_WIDTH, mastTop - (flagNo - 1) * FLAG_GAP) });

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
		}
		#endregion utils
	}
}
