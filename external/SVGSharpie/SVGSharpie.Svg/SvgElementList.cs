using System;
using System.Collections;
using System.Collections.Generic;

namespace SVGSharpie
{
    /// <summary>
    /// Represents a collection of <see cref="SvgElement"/> that can be individually accessed by index.
    /// </summary>
    public class SvgElementList<TElement> : IList<TElement>
        where TElement : SvgElement
    {
        public IEnumerator<TElement> GetEnumerator() 
            => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public void Add(TElement item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            item.Parent = _parent;
            _list.Add(item);
        }

        public void Clear()
        {
            _list.ForEach(i => i.Parent = null);
            _list.Clear();
        }

        public bool Contains(TElement item) 
            => _list.Contains(item);

        public void CopyTo(TElement[] array, int arrayIndex) 
            => _list.CopyTo(array, arrayIndex);

        public bool Remove(TElement item)
        {
            if (item == null) return false;
            item.Parent = null;
            return _list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public int IndexOf(TElement item) 
            => _list.IndexOf(item);

        public void Insert(int index, TElement item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            item.Parent = _parent;
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            var item = _list[index];
            item.Parent = null;
            _list.RemoveAt(index);
        }

        public TElement this[int index]
        {
            get => _list[index];
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                value.Parent = _parent;
                _list[index] = value;
            }
        }

        public void AddRange(IEnumerable<TElement> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        internal SvgElementList(SvgElement parent) => _parent = parent ?? throw new ArgumentNullException(nameof(parent));

        private readonly SvgElement _parent;
        private readonly List<TElement> _list = new List<TElement>();
    }

    /// <summary>
    /// Represents a collection of <see cref="SvgElement"/> that can be individually accessed by index.
    /// </summary>
    public sealed class SvgElementList : SvgElementList<SvgElement>
    {
        internal SvgElementList(SvgElement parent) : base(parent)
        {
        }
    }
}