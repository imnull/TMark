using System;
using System.Collections.Generic;

// Please write an sequence list implements the interface with the required
// time complexity described in the comments. The users can add the same
// element as many times as they want, but it doesn't support the null item.
// You can use any types in .NET BCL but cannot use any 3rd party libraries.
// PS: You don't need to consider the multi-threaded environment.
interface IMyList<T> : IEnumerable<T>
{
    // O(1)
    // Add an item at the beginning of the list.
    void AddFirst(T item);
    
    // O(1)
    // Add an item at the end of the list.
    void AddLast(T itme);
    
    // O(1)
    // Remove the item from the list. If the list contains multiple
    // copies/references of the item, remove one of them.
    void Remove(T item);
    
    // O(1)
    // Reverse the list.
    void Reverse();
}

public class MyList<T> : IMyList<T>
{
	private class TOBJ<TT>
	{
		public TOBJ(TT t)
		{
			if(t == null) throw new Exception("no null!");
			arr = new TOBJ<TT>[2];
			v = t;
		}
		private TOBJ<TT>[] arr;
		private TT v;
		public TOBJ<TT> this[int idx]
		{
			get{ return arr[idx]; }
			set{ arr[idx] = value; }
		}
		public TT Value { get{ return v; } }
		public override int GetHashCode ()
		{
			return v.GetHashCode ();
		}
	}
	
	
	public MyList ()
	{
		dic = new Dictionary<int, List<TOBJ<T>>>();
		arr = new TOBJ<T>[2];
		port = 0;
	}
	
	private TOBJ<T>[] arr;
	private Dictionary<int, List<TOBJ<T>>> dic;
	private int port;
	
	public void AddFirst(T item)
	{
		TOBJ<T> o = new TOBJ<T>(item);
		
		int hash = item.GetHashCode();
		if(!dic.ContainsKey(hash)) dic.Add(hash, new List<TOBJ<T>>());
		dic[hash].Add(o); 
		
		if(arr[0] == null) arr[0] = o;
		else {
			o[0] = arr[0];
			arr[0][1] = o;
			arr[0] = o;
		}
		if(arr[1] == null) arr[1] = arr[0];
	}
	
	public void AddLast(T item)
	{
		TOBJ<T> o = new TOBJ<T>(item);
		
		int hash = item.GetHashCode();
		if(!dic.ContainsKey(hash)) dic.Add(hash, new List<TOBJ<T>>());
		dic[hash].Add(o); 
		
		if(arr[1] == null) arr[1] = o;
		else {
			o[1] = arr[1];
			arr[1][0] = o;
			arr[1] = o;
		}
		if(arr[0] == null) arr[0] = arr[1];
	}
	
	public void Remove(T item)
	{
		if(item == null) return;
		int hash = item.GetHashCode();
		if(!dic.ContainsKey(hash)) return;
		foreach(TOBJ<T> tb in dic[hash])
		{
			if(tb[0] != null && tb[1] != null)
			{
				tb[0][1] = tb[1];
				tb[1][0] = tb[0];
			}
			else if(tb[0] != null && tb[1] == null)
			{
				tb[0][1] = null;
			}
			else if(tb[0] == null && tb[1] != null)
			{
				tb[1][0] = null;
			}
		}
	}
	
	public void Reverse()
	{
		port = (port + 1) % 2;
	}
	
	#region IEnumerable[T] implementation
	public IEnumerator<T> GetEnumerator ()
	{
		List<T> list = new List<T>();
		TOBJ<T> o = arr[port];
		while(o != null)
		{
			list.Add(o.Value);
			o = o[port];
		}
		return list.GetEnumerator();
	}
	#endregion

	#region IEnumerable implementation
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
	{
		List<T> list = new List<T>();
		TOBJ<T> o = arr[(port + 1) % 2];
		while(o != null)
		{
			list.Add(o.Value);
			o = o[port];
		}
		return list.GetEnumerator();
	}
	#endregion
}

/*

MyList<int> list = new MyList<int>();
for(int i = 0; i < 5; i++){
	list.AddLast(i);
	list.AddLast(i + 3);
}

for(int i = -10; i > -18; i--){
	list.AddLast(i);
	list.AddLast(i + 3);
}
foreach(int i in list)
{
	Console.WriteLine(i);
}
Console.WriteLine("-----------");
list.Reverse();
foreach(int i in list)
{
	Console.WriteLine(i);
} 

0
3
1
4
2
5
3
6
4
7
-10
-7
-11
-8
-12
-9
-13
-10
-14
-11
-15
-12
-16
-13
-17
-14
-----------
-14
-17
-13
-16
-12
-15
-11
-14
-10
-13
-9
-12
-8
-11
-7
-10
7
4
6
3
5
2
4
1
3
0


 * */