Custom.Collections
==================
Sometimes standard System.Collections.Generic is not enough.

KeyValueValuePair
------------------
Extends standard KeyValuePair with a second Value property. Can be extended to multiple values the same way like Tuple but I will think how to do it in a better way.
Usage:
```
  KeyValueValuePair<int, int, int> pair1 = new KeyValueValuePair<int, int, int> (1, 2, 3);
  KeyValueValuePair<int, int, int> pair2 = new KeyValueValuePair<int, int, int> (1, 2, 3);
  Console.WriteLine ("Equals: {0}, pair1: {1}; pair2: {2}", 
    pair1.Equals (pair2), //true
    pair1.ToString (), 
    pair2.ToString ());
```
FastSortedList
------------------
Replacement for standard generic SortedList. Can be used if it needed to add items fast and reading time does not matter. Implements generic ICollection.
Usage:
```
  FastSortedList<int> list = new FastSortedList<int> ();
  list.Add (10);
  list.Add (1);
  list.Add (7);
  Console.WriteLine(list.OnceReady().Count); //3
```
