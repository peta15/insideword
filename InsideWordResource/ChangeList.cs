using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace InsideWordResource
{
    //Special class to keep track of pending adds and removes
    [Serializable]
    public class ChangeList<T>
    {
        protected List<T> _addList;
        protected List<T> _removeList;

        public ChangeList()
        {
            Clear();
        }

        public List<T> AddList
        {
            get { return new List<T>(_addList); }
        }

        public List<T> RemoveList
        {
            get { return new List<T>(_removeList); }
        }

        public bool Add(T item)
        {
            if (_removeList.RemoveAll(removeItem => removeItem.Equals(item)) == 0)
            {
                _addList.Add(item);
            }

            return true;
        }

        public bool Remove(T item)
        {
            if (_addList.RemoveAll(addItem => addItem.Equals(item)) == 0)
            {
                _removeList.Add(item);
            }

            return true;
        }

        public bool Clear()
        {
            _addList = new List<T>();
            _removeList = new List<T>();
            return true;
        }

        public bool Copy(ChangeList<T> aChangeList)
        {
            _addList = new List<T>(aChangeList._addList);
            _removeList = new List<T>(aChangeList._removeList);
            return true;
        }

        public bool HasItems()
        {
            return _addList.Count > 0 || _removeList.Count > 0;
        }
    }
}
