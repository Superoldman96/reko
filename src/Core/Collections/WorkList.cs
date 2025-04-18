#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Core.Collections
{
	/// <summary>
	/// A WorkList contains a queue of items to be processed.
	/// </summary>
	public class WorkList<T>
	{
		private readonly HashSet<T> inQ;
		private readonly Queue<T> q;

        /// <summary>
        /// Creates an empty work list.
        /// </summary>
		public WorkList()
		{
			q = new Queue<T>();
			inQ = new HashSet<T>();
		}

        /// <summary>
        /// Creates a work list from an existing collection.
        /// </summary>
        /// <param name="coll"></param>
		public WorkList(IEnumerable<T> coll)
		{
			q = new Queue<T>(coll);
			inQ = new HashSet<T>(coll);
		}

        /// <summary>
        /// Adds an item to the work list, but only if it isn't there already.
        /// </summary>
        /// <param name="item">Item to add.</param>
		public void Add(T item)
		{
			if (!inQ.Contains(item))
			{
				q.Enqueue(item);
				inQ.Add(item);
			}
		}

        /// <summary>
        /// Adds a sequence of items to the work list, but only if they aren't there already.
        /// </summary>
        /// <param name="items">Items to add to the work list.</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        /// <summary>
        /// The number of items in the work list.
        /// </summary>
		public int Count
		{
			get { return inQ.Count; }
		}

        /// <summary>
        /// True if the work list is empty.
        /// </summary>
		public bool IsEmpty
		{
			get { return inQ.Count == 0; }
		}

        /// <summary>
        /// Determines if the work list contains the given item.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>
        /// If the work list contains the given item.
        /// </returns>
        public bool Contains(T item)
        {
            return inQ.Contains(item);
        }

        /// <summary>
        /// Try to extract a work item from the work list, removing it from the
        /// list if one is present.
        /// </summary>
        /// <param name="item">If there are work items in the work list, the
        /// extracted item.
        /// </param>
        /// <returns>
        /// True if an item is successfully removed; otherwise false.
        /// </returns>
		public bool TryGetWorkItem([MaybeNullWhen(false)] out T item)
		{
			while (q.TryDequeue(out var t))
			{
				if (inQ.Contains(t))
				{
					inQ.Remove(t);
                    item = t;
					return true;
				}
			}
			item = default!;
            return false;
		}

		public void Remove(T t)
		{
			inQ.Remove(t);
		}
	}

    public static class WorkList
    {
        /// <summary>
        /// Convenience function to create a worklist of items.
        /// </summary>
        public static WorkList<T> Create<T>(IEnumerable<T> items) =>
            new WorkList<T>(items);
    }


    /// <summary>
    /// A WorkStack contains a stack of items to be processed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WorkStack<T>
    {
		private readonly HashSet<T> inStack;
		private readonly Stack<T> s;

		public WorkStack()
		{
			s = new Stack<T>();
			inStack = new HashSet<T>();
		}

		public WorkStack(IEnumerable<T> coll)
		{
			s = new Stack<T>(coll);
			inStack = new HashSet<T>(coll);
		}

        /// <summary>
        /// Adds an item to the work list, but only if it isn't there already.
        /// </summary>
        /// <param name="item">Item to add.</param>
		public void Add(T item)
		{
			if (!inStack.Contains(item))
			{
				s.Push(item);
				inStack.Add(item);
			}
		}

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

		public int Count
		{
			get { return inStack.Count; }
		}

		public bool IsEmpty
		{
			get { return inStack.Count == 0; }
		}

		public bool TryGetWorkItem([MaybeNullWhen(false)] out T item)
		{
			while (!IsEmpty)
			{
				T t = s.Pop();
				if (inStack.Contains(t))
				{
					inStack.Remove(t);
                    item = t;
					return true;
				}
			}
			item = default;
            return false;
		}

		public void Remove(T t)
		{
			inStack.Remove(t);
		}
    }
}
