using Pentagram.PersistentData;
using Pentagram.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Utilz.Controlz;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pentagram.Views
{
    public sealed partial class VoiceControl : ObservableControl
    {
		public readonly int MinY0 = 50;
		public readonly int MinY1 = 50 + LineGap;
		public readonly int MinY2 = 50 + LineGap + LineGap;
		public readonly int MinY3 = 50 + LineGap + LineGap + LineGap;
		public readonly int MinY4 = 50 + LineGap + LineGap + LineGap + LineGap;
		public readonly int MinX = 0;
		public readonly int MaxX = 600;
		public const int LineGap = 25;

		public Voice Voice
		{
			get { return (Voice)GetValue(VoiceProperty); }
			set { SetValue(VoiceProperty, value); }
		}
		public static readonly DependencyProperty VoiceProperty =
			DependencyProperty.Register("Voice", typeof(Voice), typeof(VoiceControl), new PropertyMetadata(null, OnVoiceChanged));
		private static void OnVoiceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var instance = obj as VoiceControl;
			instance.VM = new VoiceVM(args.NewValue as Voice);
			instance.UpdateChiave();
		}


		private VoiceVM _vm = null;
		public VoiceVM VM { get { return _vm; } private set { _vm = value; RaisePropertyChanged_UI(); } }

		public VoiceControl()
        {
			this.InitializeComponent();
        }
		private void UpdateChiave()
		{
			ChiaveDiBasso.Visibility = Voice.Chiave == Chiavi.Basso ? Visibility.Visible : Visibility.Collapsed;
			ChiaveDiViolino.Visibility = Voice.Chiave == Chiavi.Violino ? Visibility.Visible : Visibility.Collapsed;
			DueQuarti.Visibility = Voice.Ritmo == Ritmi.dq ? Visibility.Visible : Visibility.Collapsed;
			TreQuarti.Visibility = Voice.Ritmo == Ritmi.tq ? Visibility.Visible : Visibility.Collapsed;
			QuattroQuarti.Visibility = Voice.Ritmo == Ritmi.qq ? Visibility.Visible : Visibility.Collapsed;
		}

		private void OnPentagram_Tapped(object sender, TappedRoutedEventArgs e)
		{
			_vm.AddNoteAndChord(DurateCanoniche.Croma, 3, NoteBianche.@do, Accidenti.Diesis);
		}
	}
}
