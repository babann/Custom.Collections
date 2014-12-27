using NUnit.Framework;
using System;
using Custom.Collections;

namespace Tests
{
	[TestFixture ()]
	public class FastSortedListTest
	{
		[Test ()]
		public void TestCase ()
		{
			FastSortedList<int> list = new FastSortedList<int> ();
			list.Add (10);
			list.Add (1);
			list.Add (7);
			list.Add (955);
			list.Add (3);
			list.Add (3);

			Assert.IsTrue (list.OnceReady().Count == 6);
			Assert.IsTrue (list.OnceReady()[0] == 1);
			Assert.IsTrue (list.OnceReady()[1] == 3);
			Assert.IsTrue (list.OnceReady()[2] == 3);
			Assert.IsTrue (list.OnceReady()[3] == 7);
			Assert.IsTrue (list.OnceReady()[4] == 10);
			Assert.IsTrue (list.OnceReady()[5] == 955);

			Assert.IsTrue (list.Contains (1));
			Assert.IsTrue (list.Contains (3));
			Assert.IsTrue (list.Contains (7));
			Assert.IsTrue (list.Contains (10));
			Assert.IsTrue (list.Contains (955));

			list.Add (3);
			Assert.IsTrue (list.OnceReady().Count == 7);

			Assert.IsTrue(list.Remove (3));
			Assert.IsFalse (list.Contains (3));
			Assert.IsTrue (list.Count == 4);
			Assert.IsTrue (list.OnceReady()[0] == 1);
			Assert.IsTrue (list.OnceReady()[1] == 7);
			Assert.IsTrue (list.OnceReady()[2] == 10);
			Assert.IsTrue (list.OnceReady()[3] == 955);

			Assert.IsFalse (list.Remove (3));

			list.Remove (1);
			Assert.IsFalse (list.Contains (1));
			Assert.IsTrue (list.Count == 3);
			Assert.IsFalse (list.OnceReady()[0] == 1);
			Assert.IsTrue (list.OnceReady()[0] == 7);
			Assert.IsTrue (list.OnceReady()[1] == 10);
			Assert.IsTrue (list.OnceReady()[2] == 955);

			list.Remove (955);
			Assert.IsFalse (list.Contains (955));
			Assert.IsTrue (list.Count == 2);
			Assert.IsTrue (list.OnceReady()[0] == 7);
			Assert.IsTrue (list.OnceReady()[1] == 10);

			list.Clear ();
			Assert.IsTrue (list.Count == 0);
		}
	}
}

