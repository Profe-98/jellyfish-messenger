using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyFish.ControlExtension
{
    public static class ListExtension
    {

        public static ObservableCollection<T> ToObservableCollection<T>(this IList<T> values)
        {
            var list = new ObservableCollection<T>();

            foreach (var item in values)
            {
                list.Add(item);
            }

            return list;
        }
        public static int IndexOf<T>(this IList<T> list, Predicate<T> predicate)
        {
            if (list == null) throw new ArgumentNullException("list");

            for (int i = 0; i < list.Count; i++)
            {

                if (predicate(list[i])) return i;
            }

            return -1;
        }
        public static int IndexOf<T>(this Collection<T> list, Predicate<T> predicate)
        {
            if (list == null) throw new ArgumentNullException("list");

            for (int i = 0; i < list.Count; i++)
            {

                if (predicate(list[i])) return i;
            }

            return -1;
        }
    }
}
