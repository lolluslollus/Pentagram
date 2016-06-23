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

// LOLLO TODO draw the balls shifted if you have a chord such as do re mi
namespace Pentagram.Adorners
{
	public sealed class ChordAdorner : CanvasAdorner
	{
		public readonly static double FLAG_GAP;
		public readonly static double FLAG_WIDTH;
		public readonly static double FLAG_THICKNESS;
		public readonly static double MIN_MAST_HEIGHT;

		private static PersistentData.Duration _defaultDuration = new PersistentData.Duration(DurateCanoniche.Semibiscroma);
		private static Tone _defaultTone0 = new Tone(3, NoteBianche.@do, Accidenti.Bequadro);
		private static Tone _defaultTone1 = new Tone(3, NoteBianche.mi, Accidenti.Diesis);
		private static Tone _defaultTone2 = new Tone(3, NoteBianche.sol, Accidenti.Bemolle);
		private static Tone _defaultTone3 = new Tone(4, NoteBianche.@do, Accidenti.Bequadro);
		private static Chord _defaultChord = new Chord(_defaultDuration, SegniSuNote.Nil, _defaultTone0, _defaultTone1, _defaultTone2, _defaultTone3);
		private static readonly BitmapImage _blackBallImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/palla_nera_100.png", UriKind.Absolute) };
		private static readonly BitmapImage _emptyBallImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/palla_vuota_100.png", UriKind.Absolute) };
		private static readonly BitmapImage _bemolleImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/bemolle_100.png", UriKind.Absolute) };
		private static readonly BitmapImage _bequadroImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/bequadro_100.png", UriKind.Absolute) };
		private static readonly BitmapImage _diesisImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/diesis_100.png", UriKind.Absolute) };
		private static readonly BitmapImage _pdv1Image = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/punti1_100.png", UriKind.Absolute) };
		private static readonly BitmapImage _pdv2Image = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/punti2_100.png", UriKind.Absolute) };
		private static readonly BitmapImage _pdv3Image = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/punti3_100.png", UriKind.Absolute) };

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
			FLAG_GAP = NOTE_BALL_WIDTH * (double)App.Current.Resources["NoteFlagGapFactor"];
			FLAG_THICKNESS = NOTE_BALL_WIDTH * (double)App.Current.Resources["NoteFlagThicknessFactor"];
			FLAG_WIDTH = NOTE_BALL_WIDTH + 1; // (double)App.Current.Resources["NoteFlagWidth"];
			MIN_MAST_HEIGHT = NOTE_BALL_WIDTH * (double)App.Current.Resources["NoteMinMastHeightFactor"];
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

		#region draw
		protected override void Draw()
		{
			if (_layoutRoot == null) return;
			Task upd = RunInUiThreadAsync(() =>
			{
				if (_layoutRoot == null) return;
				_layoutRoot.Children.Clear();
				if (_chord == null) return;

				var ballImageSource = _chord.Duration.DurataCanonica == DurateCanoniche.Breve || _chord.Duration.DurataCanonica == DurateCanoniche.Semibreve || _chord.Duration.DurataCanonica == DurateCanoniche.Minima ? _emptyBallImage : _blackBallImage;
				var pdvImageSource = _chord.Duration.PuntiDiValore == PuntiDiValore.One ? _pdv1Image : _chord.Duration.PuntiDiValore == PuntiDiValore.Two ? _pdv2Image : _chord.Duration.PuntiDiValore == PuntiDiValore.Three ? _pdv3Image : null;
				double ballsX0 = _chord.Tones.Any(tone => tone.Accidente != Accidenti.Nil) ? NOTE_BALL_WIDTH : 0.0;
				var ballYs = LinkedChromasHelper.GetBallYs(_chiave, _chord);
				int idx = 0;

				foreach (var tone in _chord.Tones)
				{
					DrawBall(ballImageSource, ballsX0, ballYs, idx);
					DrawAccidente(ballYs, idx, tone.Accidente);
					DrawPuntiDiValore(ballsX0, ballYs, idx, pdvImageSource);
					// 		<!--<Path Stroke="Black" StrokeThickness="2"
					// Data = "M24,12.5 V-80 H50 V-78 H24 V-70 H50 V-68 H24 V-60" /> -->

					idx++;
				}

				// draw mast and (flags or curls)
				LinkedChromasHelper helper = _chord.IsChromaFlagsBelow
					? new LinkedChromaHelper_Below(_chord, _layoutRoot) as LinkedChromasHelper
					: new LinkedChromaHelper_Above(_chord, _layoutRoot) as LinkedChromasHelper;

				if (LinkedChromasHelper.GetIsNeedsMast(_chord))
				{
					var mastTopY = helper.GetTallestMastTop(_chiave, _chord);
					helper.DrawMast(ballsX0, ballYs, mastTopY);

					int howManyFlagsOrCurls = LinkedChromasHelper.GetHowManyFlagsOrCurls(_chord);
					if (LinkedChromasHelper.GetIsNeedsFlags(_chord))
					{
						for (int i = 1; i <= howManyFlagsOrCurls; i++)
						{
							helper.DrawFlag(i, ballsX0, mastTopY);
						}
					}
					else if (LinkedChromasHelper.GetIsNeedsCurl(_chord))
					{
						for (int i = 1; i <= howManyFlagsOrCurls; i++)
						{
							helper.DrawCurl(i, ballsX0, mastTopY);
						}
					}
				}
			});
		}

		private void DrawBall(BitmapImage ballImageSource, double ballsX0, List<double> ballYs, int idx)
		{
			var newBall = new Image()
			{
				Source = ballImageSource,
				Height = LINE_GAP
			};
			_layoutRoot.Children.Add(newBall);

			Canvas.SetLeft(newBall, ballsX0);
			Canvas.SetTop(newBall, ballYs[idx]);
		}

		private void DrawAccidente(List<double> ballYs, int idx, Accidenti accidente)
		{
			BitmapImage accidenteImageSource = null;// tone.Accidente == Accidenti.Bemolle ? _bemolleImage : tone.Accidente == Accidenti.Bequadro ? _bequadroImage : tone.Accidente == Accidenti.Diesis ? _diesisImage : null;
			switch (accidente)
			{
				case Accidenti.Nil:
					break;
				case Accidenti.Bequadro:
					accidenteImageSource = _bequadroImage;
					break;
				case Accidenti.Diesis:
					accidenteImageSource = _diesisImage;
					break;
				case Accidenti.Bemolle:
					accidenteImageSource = _bemolleImage;
					break;
				case Accidenti.DoppioDiesis:
					break;
				case Accidenti.DoppioBemolle:
					break;
				default:
					break;
			}
			if (accidenteImageSource != null)
			{
				var newAccidente = new Image()
				{
					Source = accidenteImageSource,
					Height = LINE_GAP
				};
				_layoutRoot.Children.Add(newAccidente);

				Canvas.SetLeft(newAccidente, 0.0);
				Canvas.SetTop(newAccidente, ballYs[idx]);
			}
		}

		private void DrawPuntiDiValore(double ballsX, List<double> ballYs, int idx, BitmapImage pdvImageSource)
		{
			if (pdvImageSource == null) return;

			var newPDV = new Image()
			{
				Source = pdvImageSource,
				Height = LINE_GAP
			};
			_layoutRoot.Children.Add(newPDV);

			Canvas.SetLeft(newPDV, ballsX + NOTE_BALL_WIDTH);
			Canvas.SetTop(newPDV, ballYs[idx]);
		}
		#endregion draw

		public override double GetHeight()
		{
			var estimator = new ChordAdornerEstimator(_chiave, _chord);
			return estimator.GetHeight();
		}

		public override double GetWidth()
		{
			var estimator = new ChordAdornerEstimator(_chiave, _chord);
			return estimator.GetWidth();
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
					ballYs.Add(GetLineY(chiave, tone));
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
			internal abstract void DrawMast(double ballsX0, List<double> ballYs, double minY);
			internal abstract void DrawFlag(int flagNo, double ballsX0, double mastTopY);
			internal abstract void DrawCurl(int flagNo, double ballsX0, double mastTopY);
		}

		private class LinkedChromaHelper_Above : LinkedChromasHelper
		{
			internal LinkedChromaHelper_Above(Chord chord, Canvas layoutRoot) : base(chord, layoutRoot) { }
			private static double GetMastTop(List<double> ballYs)
			{
				return ballYs.Min() + LINE_GAP_HALF - MIN_MAST_HEIGHT;
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
			internal override void DrawMast(double ballsX0, List<double> ballYs, double flagY)
			{
				double ballY = ballYs.Max() + LINE_GAP_HALF;
				double mastX0 = NOTE_BALL_WIDTH - 1.0 + ballsX0;

				var mast = new PathFigure()
				{
					StartPoint = new Point(mastX0, ballY)
				};
				mast.Segments.Add(new LineSegment() { Point = new Point(mastX0, flagY) });
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
			internal override void DrawFlag(int flagNo, double ballsX0, double mastTopY)
			{
				double flagY = mastTopY + (flagNo - 1) * FLAG_GAP;
				PathFigure flagLeft = new PathFigure() { StartPoint = new Point(NOTE_BALL_WIDTH + ballsX0, flagY) };
				PathFigure flagRight = new PathFigure() { StartPoint = new Point(NOTE_BALL_WIDTH - 2.0 + ballsX0, flagY) };

				if (_chord.NextJoinedChord != null && _chord.PrevJoinedChord == null)
				{
					/*
					 * Se la seguente ha la barretta che sto facendo, aggiungi una barra a destra
					 * altrimenti aggiungi una mezza barretta a destra
					 */
					if (GetHowManyFlagsOrCurls(_chord.NextJoinedChord) >= flagNo)
						flagRight.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH + ballsX0 - 2.0 + 2.0 * FLAG_WIDTH, flagY) });
					else
						flagRight.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH + ballsX0 - 2.0 + FLAG_WIDTH, flagY) });
				}
				else if (_chord.PrevJoinedChord != null && _chord.NextJoinedChord == null)
				{
					/*
					 * Se la precedente ha la barretta che sto facendo, aggiungi una barra a sinistra
					 * altrimenti aggiungi una mezza barretta a sinistra
					 */
					if (GetHowManyFlagsOrCurls(_chord.PrevJoinedChord) >= flagNo)
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH + ballsX0 - 2.0 * FLAG_WIDTH, flagY) });
					else
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH + ballsX0 - FLAG_WIDTH, flagY) });
				}
				else
				{
					/*
					 * Se la precedente ha la barretta che sto facendo, aggiungi una barra a sinistra
					 * altrimenti aggiungi una mezza barretta a sinistra
					 * Poi
					 * Se la seguente ha la barretta che sto facendo, aggiungi una barra a destra
					 * altrimenti no
					 * */
					if (GetHowManyFlagsOrCurls(_chord.PrevJoinedChord) >= flagNo)
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH + ballsX0 - 2.0 * FLAG_WIDTH, flagY) });
					else
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH + ballsX0 - FLAG_WIDTH, flagY) });
					if (GetHowManyFlagsOrCurls(_chord.NextJoinedChord) >= flagNo)
						flagRight.Segments.Add(new LineSegment() { Point = new Point(NOTE_BALL_WIDTH + ballsX0 - 2.0 + 2.0 * FLAG_WIDTH, flagY) });
				}

				var geom = new PathGeometry();
				if (flagLeft?.Segments != null && flagLeft.Segments.Count > 0) geom.Figures.Add(flagLeft);
				if (flagRight?.Segments != null && flagRight.Segments.Count > 0) geom.Figures.Add(flagRight);
				if (geom.Figures.Count < 1) return;

				var newPath = new Windows.UI.Xaml.Shapes.Path()
				{
					Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
					StrokeThickness = FLAG_THICKNESS,
					Data = geom
				};
				_layoutRoot.Children.Add(newPath);
			}
			internal override void DrawCurl(int flagNo, double ballsX0, double mastTopY)
			{
				var flag = new PathFigure()
				{
					StartPoint = new Point(ballsX0 + NOTE_BALL_WIDTH - 2.0, mastTopY + (flagNo - 1) * FLAG_GAP)
				};
				flag.Segments.Add(new LineSegment() { Point = new Point(ballsX0 + NOTE_BALL_WIDTH - 2.0 + FLAG_WIDTH, mastTopY + (flagNo - 1) * FLAG_GAP) });

				var geom = new PathGeometry();
				geom.Figures.Add(flag);

				var newPath = new Windows.UI.Xaml.Shapes.Path()
				{
					Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
					StrokeThickness = FLAG_THICKNESS,
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
				return ballYs.Max() + LINE_GAP_HALF + MIN_MAST_HEIGHT;
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
			internal override void DrawMast(double ballsX0, List<double> ballYs, double flagY)
			{
				double ballY = ballYs.Min() + LINE_GAP_HALF;
				double mastX0 = 1.0 + ballsX0;

				var mast = new PathFigure()
				{
					StartPoint = new Point(mastX0, ballY)
				};
				mast.Segments.Add(new LineSegment() { Point = new Point(mastX0, flagY) });
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
			internal override void DrawFlag(int flagNo, double ballsX0, double mastTopY)
			{
				double flagY = mastTopY - (flagNo - 1) * FLAG_GAP;
				PathFigure flagLeft = new PathFigure() { StartPoint = new Point(2.0 + ballsX0, flagY) };
				PathFigure flagRight = new PathFigure() { StartPoint = new Point(ballsX0, flagY) };

				if (_chord.NextJoinedChord != null && _chord.PrevJoinedChord == null)
				{
					/*
					 * Se la seguente ha la barretta che sto facendo, aggiungi 1 barra a destra
					 * altrimenti aggiungi una mezza barretta a destra
					 */
					if (GetHowManyFlagsOrCurls(_chord.NextJoinedChord) >= flagNo)
						flagRight.Segments.Add(new LineSegment() { Point = new Point(ballsX0 + 2.0 * FLAG_WIDTH, flagY) });
					else
						flagRight.Segments.Add(new LineSegment() { Point = new Point(ballsX0 + FLAG_WIDTH, flagY) });
				}
				else if (_chord.PrevJoinedChord != null && _chord.NextJoinedChord == null)
				{
					/*
					 * Se la precedente ha la barretta che sto facendo, aggiungi una barra a sinistra
					 * altrimenti aggiungi una mezza barretta a sinistra
					 */
					if (GetHowManyFlagsOrCurls(_chord.PrevJoinedChord) >= flagNo)
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(2.0 + ballsX0 - 2.0 * FLAG_WIDTH, flagY) });
					else
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(2.0 + ballsX0 - FLAG_WIDTH, flagY) });
				}
				else
				{
					/*
					 * Se la precedente ha la barretta che sto facendo, aggiungi una barra a sinistra
					 * altrimenti aggiungi una mezza barretta a sinistra
					 * Poi
					 * Se la seguente ha la barretta che sto facendo, aggiungi una barra a destra
					 * altrimenti no
					 * */
					if (GetHowManyFlagsOrCurls(_chord.PrevJoinedChord) >= flagNo)
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(2.0 + ballsX0 - 2.0 * FLAG_WIDTH, flagY) });
					else
						flagLeft.Segments.Add(new LineSegment() { Point = new Point(2.0 + ballsX0 - FLAG_WIDTH, flagY) });
					if (GetHowManyFlagsOrCurls(_chord.NextJoinedChord) >= flagNo)
						flagRight.Segments.Add(new LineSegment() { Point = new Point(ballsX0 + 2.0 * FLAG_WIDTH, flagY) });
				}

				var geom = new PathGeometry();
				if (flagLeft?.Segments != null && flagLeft.Segments.Count > 0) geom.Figures.Add(flagLeft);
				if (flagRight?.Segments != null && flagRight.Segments.Count > 0) geom.Figures.Add(flagRight);
				if (geom.Figures.Count < 1) return;

				var newPath = new Windows.UI.Xaml.Shapes.Path()
				{
					Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
					StrokeThickness = FLAG_THICKNESS,
					Data = geom
				};
				_layoutRoot.Children.Add(newPath);
			}
			internal override void DrawCurl(int flagNo, double ballsX0, double mastTopY)
			{
				var flag = new PathFigure()
				{
					StartPoint = new Point(ballsX0, mastTopY - (flagNo - 1) * FLAG_GAP)
				};
				flag.Segments.Add(new LineSegment() { Point = new Point(ballsX0 + FLAG_WIDTH, mastTopY - (flagNo - 1) * FLAG_GAP) });

				var geom = new PathGeometry();
				geom.Figures.Add(flag);

				var newPath = new Windows.UI.Xaml.Shapes.Path()
				{
					Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
					StrokeThickness = FLAG_THICKNESS,
					Data = geom
				};
				_layoutRoot.Children.Add(newPath);
			}
		}
		#endregion utils
	}

	public sealed class ChordAdornerEstimator : CanvasAdornerBase
	{
		public readonly static double FLAG_GAP;
		public readonly static double FLAG_WIDTH;
		public readonly static double FLAG_THICKNESS;
		public readonly static double MIN_MAST_HEIGHT;

		private Chiavi _chiave;
		private Chord _chord = null;

		#region ctor and dispose
		static ChordAdornerEstimator()
		{
			FLAG_GAP = NOTE_BALL_WIDTH * (double)App.Current.Resources["NoteFlagGapFactor"];
			FLAG_THICKNESS = NOTE_BALL_WIDTH * (double)App.Current.Resources["NoteFlagThicknessFactor"];
			FLAG_WIDTH = NOTE_BALL_WIDTH + 1; // (double)App.Current.Resources["NoteFlagWidth"];
			MIN_MAST_HEIGHT = NOTE_BALL_WIDTH * (double)App.Current.Resources["NoteMinMastHeightFactor"];
		}
		public ChordAdornerEstimator(Chiavi chiave, Chord chord)
		{
			_chiave = chiave;
			_chord = chord;
		}
		#endregion ctor and dispose

		public override double GetHeight()
		{
			return PENTAGRAM_HEIGHT;
		}

		public override double GetWidth()
		{
			if (_chord == null) throw new ArgumentNullException("ChordAdorner.GetWidth() needs a chord");
			double result = NOTE_BALL_WIDTH + FLAG_WIDTH;
			// if (_chord.Duration.PuntiDiValore != PuntiDiValore.Nil) result += NOTE_BALL_WIDTH;
			if (_chord.Tones.Any(tone => tone.Accidente != Accidenti.Nil)) result += NOTE_BALL_WIDTH;
			return result;
		}
	}
}
