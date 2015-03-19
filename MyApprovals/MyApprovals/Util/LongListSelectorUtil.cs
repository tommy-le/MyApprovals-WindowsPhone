using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Windows.Controls;

namespace MyApprovals.Util
{
    class LongListSelectorUtil
    {
        public static void HighlightSelectionItem(LongListSelector longListSelector, SelectionChangedEventArgs e)
        {
             // Get item of LongListSelector. 
            List<UserControl> listItems = new List<UserControl>();
            LongListSelectorUtil.GetItemsRecursive<UserControl>(longListSelector, ref listItems);
        
            // Selected. 
            if (e.AddedItems.Count > 0 && e.AddedItems[0] != null)
            {
                foreach (UserControl userControl in listItems)
                    if (e.AddedItems[0].Equals(userControl.DataContext))
                        VisualStateManager.GoToState(userControl, "Selected", true);
            }
            // Unselected. 
            if (e.RemovedItems.Count > 0 && e.RemovedItems[0] != null)
            {
                foreach (UserControl userControl in listItems)
                    if (e.RemovedItems[0].Equals(userControl.DataContext))
                        VisualStateManager.GoToState(userControl, "Normal", true);
            }

        }
       
        /// <summary> 
        /// Recursive get the item. 
        /// </summary> 
        /// <typeparam name="T">The item to get.</typeparam> 
        /// <param name="parents">Parent container.</param> 
        /// <param name="objectList">Item list</param> 
        public static void GetItemsRecursive<T>(DependencyObject parents, ref List<T> objectList) where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parents);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parents, i);


                if (child is T)
                {
                    objectList.Add(child as T);
                }


                GetItemsRecursive<T>(child, ref objectList);
            }


            return;
        } 
    }
}
