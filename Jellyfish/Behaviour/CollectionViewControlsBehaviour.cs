using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;
using JellyFish.Model;

/* Nicht gemergte Änderung aus Projekt "JellyFish (net7.0-ios)"
Vor:
using System.Collections.ObjectModel;
Nach:
using System.Collections.ObjectModel;
using JellyFish;
using JellyFish.ControlExtension;
*/

/* Nicht gemergte Änderung aus Projekt "JellyFish (net7.0-ios)"
Vor:
using System.Collections.ObjectModel;
Nach:
using System.Collections.ObjectModel;
using JellyFish;
using JellyFish.ControlExtension;
using JellyFish.Behaviour;
*/
using System.Collections.ObjectModel;

namespace JellyFish.Behaviour
{
    public static class CollectionViewControlsBehaviour
    {
        public static readonly BindableProperty ScrollToEndProperty =
            BindableProperty.CreateAttached("ScrollToEnd", typeof(bool), typeof(CollectionViewControlsBehaviour), false, BindingMode.TwoWay, null, OnScrollToEndPropertyChanged);

        public static bool GetScrollToEnd(BindableObject obj)
        {
            return (bool)obj.GetValue(ScrollToEndProperty);
        }

        public static void SetScrollToEnd(BindableObject obj, bool value)
        {
            obj.SetValue(ScrollToEndProperty, value);
        }

        private static void OnScrollToEndPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var sv = bindable as CollectionView;
            if (sv == null) return;
            var iss = sv.ItemsSource.GetEnumerator();
            var childs = sv.GetVisualTreeDescendants();
            if (childs != null)
            {
                var clv = childs.OfType<CollectionView>().FirstOrDefault();
                if (clv != null)
                {
                    var isr = clv.ItemsSource;
                }
            }
            if ((bool)newValue)
            {
                var i = sv.ItemsSource.GetEnumerator();
                var data = sv.ItemsSource.Cast<object>().ToList();
                int count = data.Count;
                bool isLoaded = sv.IsLoaded;
                bool isVisbile = sv.IsVisible;
                bool isFocued = sv.IsFocused;
                if (count > 0)
                {
                    if (sv.IsGrouped)
                    {
                        var subData = data.LastOrDefault();
                        if (subData != null)
                        {
                            bool isMsgGrp = subData.GetType() == typeof(MessageGroup);
                            if (isMsgGrp)
                            {
                                var d = (MessageGroup)subData;
                                if (d != null && d.Count > 0)
                                {
                                    sv.ScrollTo(d.Count - 1, count - 1, ScrollToPosition.MakeVisible, false);
                                }
                            }
                        }
                    }
                    else
                    {

                        sv.ScrollTo(count - 1, -1, ScrollToPosition.End, false);
                    }
                }
            }
        }
    }
}
