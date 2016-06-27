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
using Windows.UI.Xaml.Shapes;

// LOLLO TODO add feature to shift above or below by one or more octaves
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

				var ballXsTuple = ChordAdornerHelper.GetBallXs(_chord);
				bool hasShiftedBalls = ballXsTuple.Item1;
				var ballXs = ballXsTuple.Item2;
				var ballYs = ChordAdornerHelper.GetBallYs(_chiave, _chord);
				int idx = 0;

				foreach (var tone in _chord.Tones)
				{
					DrawBall(ballImageSource, ballXs, ballYs, idx);
					DrawAccidente(ballYs, idx, tone.Accidente);
					DrawPuntiDiValore(ballXs, ballYs, idx, pdvImageSource);
					// 		<!--<Path Stroke="Black" StrokeThickness="2"
					// Data = "M24,12.5 V-80 H50 V-78 H24 V-70 H50 V-68 H24 V-60" /> -->

					idx++;
				}

				// draw mast and (flags or curls)
				ChordAdornerHelper helper = _chord.IsChromaFlagsBelow
					? new LinkedChromaHelper_Below(_chord, _layoutRoot) as ChordAdornerHelper
					: new LinkedChromaHelper_Above(_chord, _layoutRoot) as ChordAdornerHelper;

				if (ChordAdornerHelper.GetIsNeedsMast(_chord))
				{
					var mastTopY = helper.GetTallestMastTop(_chiave, _chord);
					helper.DrawMast(ballXs, ballYs, mastTopY);

					int howManyFlagsOrCurls = ChordAdornerHelper.GetHowManyFlagsOrCurls(_chord);
					if (ChordAdornerHelper.GetIsNeedsFlags(_chord))
					{
						for (int i = 1; i <= howManyFlagsOrCurls; i++)
						{
							helper.DrawFlag(i, ballXs, mastTopY, pdvImageSource != null, hasShiftedBalls);
						}
					}
					else if (ChordAdornerHelper.GetIsNeedsCurl(_chord))
					{
						for (int i = 1; i <= howManyFlagsOrCurls; i++)
						{
							helper.DrawCurl(i, ballXs, mastTopY);
						}
					}
				}

				DrawCutsAbove(ballXs, ballYs, idx, hasShiftedBalls);
				DrawCutsBelow(ballXs, ballYs, idx, hasShiftedBalls);
			});
		}

		private void DrawBall(BitmapImage ballImageSource, List<double> ballXs, List<double> ballYs, int idx)
		{
			var newBall = new Image()
			{
				Source = ballImageSource,
				Height = LINE_GAP
			};
			_layoutRoot.Children.Add(newBall);

			Canvas.SetLeft(newBall, ballXs[idx]);
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

		private void DrawPuntiDiValore(List<double> ballXs, List<double> ballYs, int idx, BitmapImage pdvImageSource)
		{
			if (pdvImageSource == null) return;

			var newPDV = new Image()
			{
				Source = pdvImageSource,
				Height = LINE_GAP
			};
			_layoutRoot.Children.Add(newPDV);

			Canvas.SetLeft(newPDV, ballXs[idx] + NOTE_BALL_WIDTH);
			Canvas.SetTop(newPDV, ballYs[idx] - 2.0);
		}
		private void DrawCutsAbove(List<double> ballXs, List<double> ballYs, int idx, bool hasShiftedBalls)
		{
			var pentagramTopLineY = ChordAdornerHelper.GetTopLineY();
			var cutX0 = ballXs.Min() - NOTE_BALL_WIDTH * .5;
			var cutX1 = ballXs.Max() + NOTE_BALL_WIDTH * 1.5;

			var cutY = pentagramTopLineY - LINE_GAP;
			while (cutY >= ballYs.Last())
			{
				var cutLine = new Line() { X1 = cutX0, X2 = cutX1, Y1 = cutY, Y2 = cutY, Stroke = BattutaVStackAdorner.LINE_BRUSH, StrokeThickness = BattutaVStackAdorner.LINE_STROKE_THICKNESS };
				_layoutRoot.Children.Add(cutLine);
				cutY -= LINE_GAP;
			}
		}
		private void DrawCutsBelow(List<double> ballXs, List<double> ballYs, int idx, bool hasShiftedBalls)
		{
			var pentagramBottomLineY = ChordAdornerHelper.GetBottomLineY();
			var cutX0 = ballXs.Min() - NOTE_BALL_WIDTH * .5;
			var cutX1 = ballXs.Max() + NOTE_BALL_WIDTH * 1.5;

			var cutY = pentagramBottomLineY + LINE_GAP;
			while (cutY <= ballYs.First())
			{
				var cutLine = new Line() { X1 = cutX0, X2 = cutX1, Y1 = cutY, Y2 = cutY, Stroke = BattutaVStackAdorner.LINE_BRUSH, StrokeThickness = BattutaVStackAdorner.LINE_STROKE_THICKNESS };
				_layoutRoot.Children.Add(cutLine);
				cutY += LINE_GAP;
			}
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
	}
	#region utils
	internal abstract class ChordAdornerHelper
	{
		protected readonly Chord _chord = null;
		protected readonly Canvas _layoutRoot = null;
		internal ChordAdornerHelper(Chord chord, Canvas layoutRoot)
		{
			if (chord == null) throw new ArgumentNullException("LinkedChromasHelper needs a chord");
			if (layoutRoot == null) throw new ArgumentNullException("LinkedChromasHelper needs a canvas");
			_chord = chord;
			_layoutRoot = layoutRoot;
		}
		internal static Tuple<bool, List<double>> GetBallXs(Chord chord)
		{
			double ballsX0 = chord.Tones.Any(tone => tone.Accidente != Accidenti.Nil) ?
				CanvasAdornerBase.NOTE_BALL_WIDTH : 0.0;
			var ballXs = new List<double>();
			foreach (var tone in chord.Tones)
			{
				ballXs.Add(ballsX0);
			}
			bool hasShiftedNotes = false;

			for (int i = 1; i < chord.Tones.Count; i += 2)
			{
				if ((chord.Tones[i].Ottava == chord.Tones[i - 1].Ottava && chord.Tones[i].NotaBianca == chord.Tones[i - 1].NotaBianca + 1)
					|| (chord.Tones[i].Ottava == chord.Tones[i - 1].Ottava + 1 && chord.Tones[i].NotaBianca == NoteBianche.@do && chord.Tones[i - 1].NotaBianca == NoteBianche.si))
				{
					ballXs[i] += (CanvasAdornerBase.NOTE_BALL_WIDTH - 2.0);
					hasShiftedNotes = true;
				}
			}

			return Tuple.Create(hasShiftedNotes, ballXs);
		}
		internal static List<double> GetBallYs(Chiavi chiave, Chord chord)
		{
			var ballYs = new List<double>();

			foreach (var tone in chord.Tones)
			{
				ballYs.Add(CanvasAdornerBase.GetLineY(chiave, tone));
			}

			//for (int i = 1; i < ballYs.Count; i++)
			//{
			//	if (ballYs[i] > ballYs[i - 1]) { var sss = "WWW"; }
			//}
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
		internal static double GetTopLineY()
		{
			return CanvasAdornerBase.GetLineY(new Tone(4, NoteBianche.fa, Accidenti.Nil));
		}
		internal static double GetBottomLineY()
		{
			return CanvasAdornerBase.GetLineY(new Tone(3, NoteBianche.mi, Accidenti.Nil));
		}
		internal abstract double GetTallestMastTop(Chiavi chiave, Chord chord);
		internal abstract void DrawMast(List<double> ballXs, List<double> ballYs, double minY);
		internal abstract void DrawFlag(int flagNo, List<double> ballX0s, double mastTopY, bool hasPDV, bool hasShiftedBalls);
		internal abstract void DrawCurl(int flagNo, List<double> ballXs, double mastTopY);
	}

	internal class LinkedChromaHelper_Above : ChordAdornerHelper
	{
		internal LinkedChromaHelper_Above(Chord chord, Canvas layoutRoot) : base(chord, layoutRoot) { }
		private static double GetMastTop(List<double> ballYs)
		{
			return ballYs.Last() + CanvasAdornerBase.LINE_GAP_HALF - ChordAdorner.MIN_MAST_HEIGHT;
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
		internal override void DrawMast(List<double> ballXs, List<double> ballYs, double flagY)
		{
			double ballY = ballYs.First() + CanvasAdornerBase.LINE_GAP_HALF;
			double mastX0 = CanvasAdornerBase.NOTE_BALL_WIDTH - 1.0 + ballXs.Min();

			var mast = new PathFigure()
			{
				StartPoint = new Point(mastX0, ballY)
			};
			mast.Segments.Add(new LineSegment() { Point = new Point(mastX0, flagY) });
			var geom = new PathGeometry();
			geom.Figures.Add(mast);

			var newPath = new Path()
			{
				Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
				StrokeThickness = 2.0,
				Data = geom
			};
			_layoutRoot.Children.Add(newPath);
		}
		internal override void DrawFlag(int flagNo, List<double> ballXs, double mastTopY, bool hasPDV, bool hasShiftedBalls)
		{
			double ballsX0 = ballXs.Min();
			double flagY = mastTopY + (flagNo - 1) * ChordAdorner.FLAG_GAP;
			double extendedFlagWidth = !hasPDV || !hasShiftedBalls ? 2.0 * ChordAdorner.FLAG_WIDTH : 3.0 * ChordAdorner.FLAG_WIDTH;
			PathFigure flagLeft = new PathFigure() { StartPoint = new Point(ChordAdorner.NOTE_BALL_WIDTH + ballsX0, flagY) };
			PathFigure flagRight = new PathFigure() { StartPoint = new Point(ChordAdorner.NOTE_BALL_WIDTH + ballsX0 - 2.0, flagY) };

			if (_chord.NextJoinedChord != null && _chord.PrevJoinedChord == null)
			{
				/*
				 * Se la seguente ha la barretta che sto facendo, aggiungi una barra a destra
				 * altrimenti aggiungi una mezza barretta a destra
				 */
				if (GetHowManyFlagsOrCurls(_chord.NextJoinedChord) >= flagNo)
					flagRight.Segments.Add(new LineSegment() { Point = new Point(ChordAdorner.NOTE_BALL_WIDTH + ballsX0 - 2.0 + extendedFlagWidth, flagY) });
				else
					flagRight.Segments.Add(new LineSegment() { Point = new Point(ChordAdorner.NOTE_BALL_WIDTH + ballsX0 - 2.0 + ChordAdorner.FLAG_WIDTH, flagY) });
			}
			else if (_chord.PrevJoinedChord != null && _chord.NextJoinedChord == null)
			{
				/*
				 * Se la precedente ha la barretta che sto facendo, aggiungi una barra a sinistra
				 * altrimenti aggiungi una mezza barretta a sinistra
				 */
				if (GetHowManyFlagsOrCurls(_chord.PrevJoinedChord) >= flagNo)
					flagLeft.Segments.Add(new LineSegment() { Point = new Point(ChordAdorner.NOTE_BALL_WIDTH + ballsX0 - extendedFlagWidth, flagY) });
				else
					flagLeft.Segments.Add(new LineSegment() { Point = new Point(ChordAdorner.NOTE_BALL_WIDTH + ballsX0 - ChordAdorner.FLAG_WIDTH, flagY) });
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
					flagLeft.Segments.Add(new LineSegment() { Point = new Point(ChordAdorner.NOTE_BALL_WIDTH + ballsX0 - extendedFlagWidth, flagY) });
				else
					flagLeft.Segments.Add(new LineSegment() { Point = new Point(ChordAdorner.NOTE_BALL_WIDTH + ballsX0 - ChordAdorner.FLAG_WIDTH, flagY) });
				if (GetHowManyFlagsOrCurls(_chord.NextJoinedChord) >= flagNo)
					flagRight.Segments.Add(new LineSegment() { Point = new Point(ChordAdorner.NOTE_BALL_WIDTH + ballsX0 - 2.0 + extendedFlagWidth, flagY) });
			}

			var geom = new PathGeometry();
			if (flagLeft?.Segments != null && flagLeft.Segments.Count > 0) geom.Figures.Add(flagLeft);
			if (flagRight?.Segments != null && flagRight.Segments.Count > 0) geom.Figures.Add(flagRight);
			if (geom.Figures.Count < 1) return;

			var newPath = new Windows.UI.Xaml.Shapes.Path()
			{
				Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
				StrokeThickness = ChordAdorner.FLAG_THICKNESS,
				Data = geom
			};
			_layoutRoot.Children.Add(newPath);
		}
		internal override void DrawCurl(int flagNo, List<double> ballXs, double mastTopY)
		{
			double ballsX0 = ballXs.Min();
			var flag = new PathFigure()
			{
				StartPoint = new Point(ballsX0 + ChordAdorner.NOTE_BALL_WIDTH - 2.0, mastTopY + (flagNo - 1) * ChordAdorner.FLAG_GAP)
			};
			flag.Segments.Add(new LineSegment() { Point = new Point(ballsX0 + ChordAdorner.NOTE_BALL_WIDTH - 2.0 + ChordAdorner.FLAG_WIDTH, mastTopY + (flagNo - 1) * ChordAdorner.FLAG_GAP) });

			var geom = new PathGeometry();
			geom.Figures.Add(flag);

			var newPath = new Windows.UI.Xaml.Shapes.Path()
			{
				Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
				StrokeThickness = ChordAdorner.FLAG_THICKNESS,
				Data = geom
			};
			_layoutRoot.Children.Add(newPath);
		}
	}

	internal class LinkedChromaHelper_Below : ChordAdornerHelper
	{
		internal LinkedChromaHelper_Below(Chord chord, Canvas layoutRoot) : base(chord, layoutRoot) { }
		private static double GetMastTop(List<double> ballYs)
		{
			return ballYs.First() + CanvasAdornerBase.LINE_GAP_HALF + ChordAdorner.MIN_MAST_HEIGHT;
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
		internal override void DrawMast(List<double> ballXs, List<double> ballYs, double flagY)
		{
			double ballY = ballYs.Last() + CanvasAdornerBase.LINE_GAP_HALF;
			double mastX0 = 1.0 + ballXs.Min();

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
		internal override void DrawFlag(int flagNo, List<double> ballXs, double mastTopY, bool hasPDV, bool hasShiftedBalls)
		{
			double ballsX0 = ballXs.Min();
			double flagY = mastTopY - (flagNo - 1) * ChordAdorner.FLAG_GAP;
			double extendedFlagWidth = !hasPDV || !hasShiftedBalls ? 2.0 * ChordAdorner.FLAG_WIDTH : 3.0 * ChordAdorner.FLAG_WIDTH;
			PathFigure flagLeft = new PathFigure() { StartPoint = new Point(2.0 + ballsX0, flagY) };
			PathFigure flagRight = new PathFigure() { StartPoint = new Point(ballsX0, flagY) };

			if (_chord.NextJoinedChord != null && _chord.PrevJoinedChord == null)
			{
				/*
				 * Se la seguente ha la barretta che sto facendo, aggiungi 1 barra a destra
				 * altrimenti aggiungi una mezza barretta a destra
				 */
				if (GetHowManyFlagsOrCurls(_chord.NextJoinedChord) >= flagNo)
					flagRight.Segments.Add(new LineSegment() { Point = new Point(ballsX0 + extendedFlagWidth, flagY) });
				else
					flagRight.Segments.Add(new LineSegment() { Point = new Point(ballsX0 + ChordAdorner.FLAG_WIDTH, flagY) });
			}
			else if (_chord.PrevJoinedChord != null && _chord.NextJoinedChord == null)
			{
				/*
				 * Se la precedente ha la barretta che sto facendo, aggiungi una barra a sinistra
				 * altrimenti aggiungi una mezza barretta a sinistra
				 */
				if (GetHowManyFlagsOrCurls(_chord.PrevJoinedChord) >= flagNo)
					flagLeft.Segments.Add(new LineSegment() { Point = new Point(2.0 + ballsX0 - extendedFlagWidth, flagY) });
				else
					flagLeft.Segments.Add(new LineSegment() { Point = new Point(2.0 + ballsX0 - ChordAdorner.FLAG_WIDTH, flagY) });
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
					flagLeft.Segments.Add(new LineSegment() { Point = new Point(2.0 + ballsX0 - extendedFlagWidth, flagY) });
				else
					flagLeft.Segments.Add(new LineSegment() { Point = new Point(2.0 + ballsX0 - ChordAdorner.FLAG_WIDTH, flagY) });
				if (GetHowManyFlagsOrCurls(_chord.NextJoinedChord) >= flagNo)
					flagRight.Segments.Add(new LineSegment() { Point = new Point(ballsX0 + extendedFlagWidth, flagY) });
			}

			var geom = new PathGeometry();
			if (flagLeft?.Segments != null && flagLeft.Segments.Count > 0) geom.Figures.Add(flagLeft);
			if (flagRight?.Segments != null && flagRight.Segments.Count > 0) geom.Figures.Add(flagRight);
			if (geom.Figures.Count < 1) return;

			var newPath = new Windows.UI.Xaml.Shapes.Path()
			{
				Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
				StrokeThickness = ChordAdorner.FLAG_THICKNESS,
				Data = geom
			};
			_layoutRoot.Children.Add(newPath);
		}
		internal override void DrawCurl(int flagNo, List<double> ballXs, double mastTopY)
		{
			double ballsX0 = ballXs.Min();
			var flag = new PathFigure()
			{
				StartPoint = new Point(ballsX0, mastTopY - (flagNo - 1) * ChordAdorner.FLAG_GAP)
			};
			flag.Segments.Add(new LineSegment() { Point = new Point(ballsX0 + ChordAdorner.FLAG_WIDTH, mastTopY - (flagNo - 1) * ChordAdorner.FLAG_GAP) });

			var geom = new PathGeometry();
			geom.Figures.Add(flag);

			var newPath = new Windows.UI.Xaml.Shapes.Path()
			{
				Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
				StrokeThickness = ChordAdorner.FLAG_THICKNESS,
				Data = geom
			};
			_layoutRoot.Children.Add(newPath);
		}
	}
	#endregion utils

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
			// double result = NOTE_BALL_WIDTH + FLAG_WIDTH;
			//if (_chord.Tones.Any(tone => tone.Accidente != Accidenti.Nil)) result += NOTE_BALL_WIDTH;

			var ballXsTuple = ChordAdornerHelper.GetBallXs(_chord);
			double result = ballXsTuple.Item2.Max() + NOTE_BALL_WIDTH + FLAG_WIDTH;

			return result;
		}
	}
}
