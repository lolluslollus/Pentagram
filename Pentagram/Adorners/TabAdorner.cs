﻿using Pentagram.PersistentData;
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
	public sealed class TabAdorner : Adorner
	{
		public readonly static double ARMATURA_WIDTH;
		public readonly static double CHIAVE_WIDTH;
		public readonly static double REFRAIN_WIDTH;
		public readonly static double RITMO_WIDTH;
		public readonly static double VERTICAL_BAR_WIDTH;

		private static readonly BitmapImage _chiaveDiViolino = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/chiaveDiViolino.png", UriKind.Absolute) };
		private static readonly BitmapImage _chiaveDiBasso = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/chiaveDiBasso.png", UriKind.Absolute) };
		private static readonly BitmapImage _qq = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/qq.png", UriKind.Absolute) };
		private static readonly BitmapImage _tq = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/tq.png", UriKind.Absolute) };
		private static readonly BitmapImage _dq = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/dq.png", UriKind.Absolute) };

		private readonly Chiavi _chiave;
		private readonly Ritmi _ritmo;

		private Tab _tab = null;
		public Tab Tab
		{
			get { return _tab; }
			private set
			{
				bool isChanged = value != _tab;
				if (isChanged)
				{
					if (_tab != null) _tab.PropertyChanged -= OnTab_PropertyChanged;
					_tab = value;
					if (_tab != null) _tab.PropertyChanged += OnTab_PropertyChanged;
				}
			}
		}
		private void OnTab_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Draw();
		}

		#region ctor and dispose
		static TabAdorner()
		{
			ARMATURA_WIDTH = (double)App.Current.Resources["ArmaturaWidth"];
			CHIAVE_WIDTH = (double)App.Current.Resources["ChiaveWidth"];
			REFRAIN_WIDTH = (double)App.Current.Resources["RefrainWidth"];
			RITMO_WIDTH = (double)App.Current.Resources["RitmoWidth"];
			VERTICAL_BAR_WIDTH = (double)App.Current.Resources["VerticalBarWidth"];
		}
		public TabAdorner(Canvas parentLayoutRoot, Tab tab, Chiavi chiave, Ritmi ritmo) : base(parentLayoutRoot)
		{
			_chiave = chiave;
			_ritmo = ritmo;
			Tab = tab;
			Draw();
		}
		public override void Dispose()
		{
			if (_tab != null) _tab.PropertyChanged -= OnTab_PropertyChanged;
		}
		#endregion ctor and dispose

		private void Draw()
		{
			Task upd = RunInUiThreadAsync(() =>
			{
				_layoutRoot.Children.Clear();
				if (_tab == null) return;

				// draw symbol
				BitmapImage imageSource = null;
				switch (_tab.TabSymbol)
				{
					case TabSymbols.Nil:
						break;
					case TabSymbols.Chiave:
						if (_chiave == Chiavi.Violino) imageSource = _chiaveDiViolino;
						else if (_chiave == Chiavi.Basso) imageSource = _chiaveDiBasso;
						break;
					case TabSymbols.Armatura:
						break;
					case TabSymbols.Ritmo:
						if (_ritmo == Ritmi.dq) imageSource = _dq;
						else if (_ritmo == Ritmi.tq) imageSource = _tq;
						else if (_ritmo == Ritmi.qq) imageSource = _qq;
						break;
					case TabSymbols.TwoVerticalBars:
						break;
					case TabSymbols.Refrain:
						break;
					default:
						break;
				}
				if (imageSource == null) return;

				_layoutRoot.Children.Add(new Image() { Source = imageSource });
			});
		}

		public override double GetHeight()
		{
			return PENTAGRAM_HEIGHT;
		}

		public override double GetWidth()
		{
			if (_tab == null) throw new ArgumentNullException("TabAdorner.GetWidth() needs a tab");
			switch (_tab.TabSymbol)
			{
				case TabSymbols.Nil:
					return NOTE_BALL_WIDTH;
				case TabSymbols.Chiave:
					return CHIAVE_WIDTH;
				case TabSymbols.Armatura:
					return ARMATURA_WIDTH;
				case TabSymbols.Ritmo:
					return RITMO_WIDTH;
				case TabSymbols.TwoVerticalBars:
					return VERTICAL_BAR_WIDTH;
				case TabSymbols.Refrain:
					return REFRAIN_WIDTH;
				default:
					return NOTE_BALL_WIDTH;
			}

		}
	}
}