using System;
using System.Collections.Generic;
using System.Threading;

namespace Custom.Collections
{
	/// <summary>
	/// Fast sorted list implementation.
	/// </summary>
	[Serializable]
	public class FastSortedList<T> : ICollection<T>, IDisposable
	{
		#region Constants

		private const int DEFAULT_ARRAY_SIZE = 0;

		#endregion

		#region Fields

		private Thread _sortThread;
		private Queue<T> _unsorted;
		private T[] _sorted;
		private bool _isDisposing = false;
		private object _locker = new object();
		private int _size;
		private IComparer<T> _comparer;
		private ManualResetEventSlim _sortDone;

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether this list is sorted and ready to be read.
		/// </summary>
		/// <value><c>true</c> if this list is ready; otherwise, <c>false</c>.</value>
		public bool IsReady {
			get;
			private set;
		}

		/// <summary>
		/// Gets the item at the specified index.
		/// </summary>
		/// <param name="index">Index</param>
		public T this[int index]
		{
			get
			{
				return _sorted [index];
			}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Custom.Collections.FastSortedList`1"/> class.
		/// </summary>
		public FastSortedList ()
		{
			_size = 0;
			_comparer = Comparer<T>.Default;
			_unsorted = new Queue<T> ();
			_sorted = new T[DEFAULT_ARRAY_SIZE];
			_sortDone = new ManualResetEventSlim (true);
			_sortThread = new Thread(new ThreadStart(SortMethod));
			_sortThread.IsBackground = true;
			IsReady = true;
			_sortThread.Start ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Custom.Collections.FastSortedList`1"/> class.
		/// </summary>
		/// <param name="comparer">Comparer</param>
		public FastSortedList(IComparer<T> comparer)
			: this()
		{
			if (comparer != null)
				_comparer = comparer;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Custom.Collections.FastSortedList`1"/> class.
		/// </summary>
		/// <param name="collection">Source items</param>
		public FastSortedList(IEnumerable<T> collection)
			:this(collection, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Custom.Collections.FastSortedList`1"/> class.
		/// </summary>
		/// <param name="collection">Source items</param>
		/// <param name="comparer">Comparer</param>
		public FastSortedList(IEnumerable<T> collection, IComparer<T> comparer)
			:this()
		{
			foreach (var item in collection)
				_unsorted.Enqueue (item);
		}

		#endregion

		#region Destructor

		~FastSortedList(){
			Dispose(false);
		}

		#endregion

		#region IDisposable implementation

		public void Dispose ()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		#endregion

		#region Implementation

		private void Dispose(bool disposing)
		{
			if (!this._isDisposing) {
				if (disposing) {
					this._isDisposing = true;
					Clear ();
					_sortThread.Join (100);
					_sortThread = null;
				}
			}
		}

		/// <summary>
		/// Sort method running in backround.
		/// </summary>
		private void SortMethod()
		{
			while (!_isDisposing) {
				while (_unsorted.Count > 0) {
					if (_isDisposing)
						return;

					_sortDone.Reset ();
					IsReady = false;

					lock (_locker) {
						if (_unsorted.Count == 0)
							break;
						EnsureCapacity (_size + 1);

						T item = _unsorted.Dequeue ();
						_sorted [_size] = item;
						BubbleSort ();
						_size++;
					}

				}

				_sortDone.Set ();
				IsReady = true;
			}
		}

		/// <summary>
		/// Ensures the capacity of internal sorted array.
		/// </summary>
		/// <param name="requiredLength">Required length.</param>
		private void EnsureCapacity(int requiredLength)
		{
			if (_sorted.Length <= requiredLength) {
				T[] newArray = new T[(_sorted.Length == 0 ? 1 : _sorted.Length) * 2];
				Array.Copy (_sorted, newArray, _sorted.Length);
				_sorted = newArray;
			}
		}

		/// <summary>
		/// Custom bubble sort implementation.
		/// </summary>
		private void BubbleSort()
		{
			for (int ii = _size; ii > 0; ii--) {
				bool swaped = false;

				if (_comparer.Compare (_sorted [ii], _sorted [ii - 1]) < 0) {
					swaped = true;

					T item = _sorted [ii - 1];
					_sorted [ii - 1] = _sorted [ii];
					_sorted [ii] = item;
				}

				if (!swaped)
					return;
			}
		}

		/// <summary>
		/// Binary search implementation.
		/// </summary>
		/// <returns>Index of first found element (not necessary it is first same element in array)</returns>
		/// <param name="item">Item to search.</param>
		private int BinarySearch(T item)
		{
			if (_size == 0)
				return -1;

			int[] bounds = new int[] { 0, _size-1 };

			int lastIdx = -1, index = (bounds[1] + bounds[0])/ 2;
			while(index != lastIdx)
			{
				//first check bounds
				if(_comparer.Compare(item, this[bounds[0]]) == 0)
					return bounds[0];
				if(_comparer.Compare(item, this[bounds[1]]) == 0)
					return bounds[1];

				lastIdx = index;
				switch (_comparer.Compare (item, this [index])) {
				case 0:
					return index;
				case 1:
					bounds [0] = index;
					break;
				case -1:
					bounds [1] = index;
					break;
				}

				index = (bounds[1] + bounds[0])/ 2;
			}

			return -1;
		}

		#endregion

		#region Public Interface

		/// <summary>
		/// Allow to continue working with a list one sort is complete.
		/// </summary>
		public FastSortedList<T> OnceReady()
		{
			_sortDone.Wait ();
			return this;
		}

		#region ICollection implementation

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item to add</param>
		public void Add (T item)
		{
			IsReady = false;
			_unsorted.Enqueue (item);
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear ()
		{
			lock (_locker) {
				_unsorted.Clear ();
				_sorted = new T[DEFAULT_ARRAY_SIZE];
				_size = 0;
			}
		}

		/// <summary>
		/// Contains the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public bool Contains (T item)
		{
			lock (_locker) {
				return BinarySearch (item) >= 0;
			}
		}

		/// <summary>
		/// Copies the items to array.
		/// </summary>
		/// <param name="array">Destination array</param>
		/// <param name="arrayIndex">Index of the items where the copyig begins</param>
		public void CopyTo (T[] array, int arrayIndex)
		{
			_sorted.CopyTo (array, arrayIndex);
		}
			
		/// <summary>
		/// Removes the specified item.
		/// </summary>
		/// <param name="item">Item to remove.</param>
		public bool Remove (T item)
		{
			lock (_locker) {
				var itemIndex = BinarySearch (item);
				if (itemIndex < 0)
					return false;

				bool[] other = new bool[]{ false, false };
				int[] bounds = new int[] { itemIndex, itemIndex };

				for (int distance = 1; distance < _size - itemIndex; distance++) {
					if (!other [0] && (itemIndex - distance) >= 0
					   && _comparer.Compare (item, _sorted [itemIndex - distance]) == 0)
						bounds [0]--;
					else
						other [0] = true;

					if (!other [1] && itemIndex + distance < _size - 1
					   && _comparer.Compare (item, this [itemIndex + distance]) == 0)
						bounds [1]++;
					else
						other [1] = true;

					if (other [0] && other [1])
						break;
				}

				T[] newArray = new T[_sorted.Length];
				Array.Copy (_sorted, 0, newArray, 0, bounds [0]);
				Array.Copy (_sorted, bounds [1] + 1, newArray, bounds [0], _size - bounds [1] - 1);

				_sorted = newArray;
				_size = _size - (1 + bounds [1] - bounds [0]);
			}

			return true;
		}

		/// <summary>
		/// Gets the count of sorted items.
		/// </summary>
		/// <value>The count of sortded.</value>
		public int Count {
			get {
				return _size;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
		public bool IsReadOnly {
			get {
				return false;
			}
		}

		#endregion

		#region IEnumerable implementation

		/// <summary>
		/// GetEnumerator returns an IEnumerator over this FastSortedList
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<T> GetEnumerator ()
		{
			return new Enumerator (this);
		}

		#endregion

		#region IEnumerable implementation

		/// <summary>
		/// GetEnumerator returns an IEnumerator over this FastSortedList
		/// </summary>
		/// <returns>The enumerator.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return new Enumerator (this);
		}

		#endregion

		#endregion

		/// <summary>
		/// Implements the enumerator for the FastSortedList
		/// </summary>
		public struct Enumerator : IEnumerator<T>, System.Collections.IEnumerator
		{
			private FastSortedList<T> _list;
			private int _index;   // -1 = not started, -2 = ended/disposed
			private T _currentElement;

			internal Enumerator(FastSortedList<T> q) {
				_list = q;
				_index = -1;
				_currentElement = default(T);
			}

			public void Dispose()
			{
				_index = -2;
				_currentElement = default(T);
			}

			public bool MoveNext() {
				if (_index == -2)
					return false;

				_index++;

				if (_index == _list._size) {
					_index = -2;
					_currentElement = default(T);
					return false;
				}

				_currentElement = _list[_index];
				return true;
			}

			public T Current {
				get {
					if (_index < 0)
						throw new InvalidOperationException ();

					return _currentElement;
				}
			}

			Object System.Collections.IEnumerator.Current {
				get {
					if (_index < 0)
						throw new InvalidOperationException ();

					return _currentElement;
				}
			}

			void System.Collections.IEnumerator.Reset() {
				_index = -1;
				_currentElement = default(T);
			}
		}
	}
}

