using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilz;

namespace Pentagram.Utilz
{
	public class SortedSwitchableObservableCollection<T> : SwitchableObservableCollection<T> where T : IComparable
	{
		public SortedSwitchableObservableCollection() : base() { }
		public SortedSwitchableObservableCollection(IEnumerable<T> collection) : base(collection) { }
		public SortedSwitchableObservableCollection(int capacity) : base(capacity) { }
		public SortedSwitchableObservableCollection(bool isObserving) : base(isObserving) { }
		public SortedSwitchableObservableCollection(bool isObserving, IEnumerable<T> collection) : base(isObserving, collection) { }
		public SortedSwitchableObservableCollection(bool isObserving, int capacity) : base(isObserving, capacity) { }
		// LOLLO NOTE the serialiser calls this
		new public void Add(T item)
		{
			// get out if no new items
			if (item == null) return;

			// add the items, firing the events as usual
			InsertIntoSortedPosition(item);
		}

		new public void AddRange(IEnumerable<T> range)
		{
			// get out if no new items
			if (range == null || !range.Any()) return;

			// prepare data for firing the events
			int newStartingIndex = Count;
			var newItems = new List<T>();
			newItems.AddRange(range);

			// add the items, making sure no events are fired
			_isObserving = false;
			foreach (var item in range)
			{
				InsertIntoSortedPosition(item);
			}
			_isObserving = true;

			// fire the events
			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			// this is tricky: call Reset first to make sure the controls will respond properly and not only add one item
			// LOLLO NOTE I took out the following so the list viewers don't lose the position.
			//OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Reset));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Add, changedItems: newItems, startingIndex: newStartingIndex));
		}

		new public void ReplaceAll(IEnumerable<T> range)
		{
			base.ReplaceAll(range.OrderBy(item => item));
		}

		private void InsertIntoSortedPosition(T item)
		{
			if (item == null) return;
			var itemsBefore = Items?.Where(it => it.CompareTo(item) < 1);
			if (itemsBefore == null)
			{
				InsertItem(0, item);
			}
			else
			{
				InsertItem(itemsBefore.Count(), item);
			}
		}
	}
}
