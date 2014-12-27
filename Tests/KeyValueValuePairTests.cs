using NUnit.Framework;
using System;
using Custom.Collections;

namespace Tests
{
	[TestFixture()]
	public class KeyValueValuePairTests
	{
		[Test()]
		public void TestCase ()
		{
			KeyValueValuePair<int, int, int> pair1 = new KeyValueValuePair<int, int, int> (1, 2, 3);
			KeyValueValuePair<int, int, int> pair2 = new KeyValueValuePair<int, int, int> (1, 2, 3);
			KeyValueValuePair<int, int, int> pair3 = new KeyValueValuePair<int, int, int> (2, 2, 3);
			KeyValueValuePair<int, int, int> pair4 = new KeyValueValuePair<int, int, int> (1, 3, 3);
			KeyValueValuePair<int, int, int> pair5 = new KeyValueValuePair<int, int, int> (1, 2, 4);

			KeyValueValuePair<string, int, int> pair6 = new KeyValueValuePair<string, int, int> ("test", 2, 3);

			KeyValueValuePair<string, string, string> pair7 = new KeyValueValuePair<string, string, string> ("test", "test1", "test2");
			KeyValueValuePair<string, string, string> pair8 = new KeyValueValuePair<string, string, string> ("test", "test1", "test2");
			KeyValueValuePair<string, string, string> pair9 = new KeyValueValuePair<string, string, string> ("mykey", "test1", "test2");

			Assert.IsTrue (pair1.Equals(pair2));
			Assert.IsFalse (pair2.Equals(pair3));
			Assert.IsFalse (pair1.Equals(pair4));
			Assert.IsFalse (pair1.Equals(pair5));
			Assert.IsFalse (pair1.Equals(pair6));
			Assert.IsTrue (pair7.Equals(pair8));
			Assert.IsFalse (pair7.Equals(pair9));
		}
	}
}

