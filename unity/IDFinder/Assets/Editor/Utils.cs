using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

public static class Utils
{
    public static VisualElement GetInput<T>(this BaseField<T> field)
    {
        return (VisualElement)typeof(BaseField<T>).GetProperty("visualInput", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true).Invoke(field, null);
    }

    public static IEnumerable<T> Duplicate<T>(this IEnumerable<T> enumerable)
    {
        return new Duplicator<T>(enumerable);
    }

    private class Duplicator<T> : IEnumerable<T>, IEnumerator<T>
    {
        private readonly IEnumerable<T> _source;
        private IEnumerator<T> _enumerator;
        private LinkedList<T> _buffer = new LinkedList<T>();
        private T _current;
        private bool _readingLinkedList = false;
        private bool _disposed = false;

        public Duplicator(IEnumerable<T> source)
        {
            _source = source;
        }

        public T Current => _current;

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_disposed) return false;

            if (!_readingLinkedList)
            {
                _enumerator ??= _source.GetEnumerator();
                if (_enumerator.MoveNext())
                {
                    _buffer.AddLast(new LinkedListNode<T>(_enumerator.Current));
                    return true;
                }
                else
                {
                    _readingLinkedList = true;
                }
            }

            // If we are here, we are reading the linked list
            if (_buffer.Count > 0)
            {
                _current = _buffer.First.Value;
                _buffer.RemoveFirst();
                return true;
            }
            else
            {
                _current = default;
                Dispose();
                return false;
            }
        }

        public void Reset() => throw new NotSupportedException();

        public void Dispose()
        {
            _disposed = true;
            _buffer = null;
        }

        public IEnumerator<T> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
