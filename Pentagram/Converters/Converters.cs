using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Pentagram.Converters
{
	public class VoicesCountToHeight : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			int iint = 1;
			if (!int.TryParse(value.ToString(), out iint)) return 1;

			double pentagramHeight = (double)App.Current.Resources["PentagramHeight"];
			return Math.Max(iint, 1) * pentagramHeight; // must never return 0 or less
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new Exception("this is a one-way binding, it should never come here");
		}
	}

}
