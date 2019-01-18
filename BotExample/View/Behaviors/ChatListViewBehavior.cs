using System;
using System.Collections.ObjectModel;
using BotExample.Model;
using Xamarin.Forms;

namespace BotExample.View.Behaviors
{
    public class ChatListViewBehavior : Behavior<ListView>
    {
        ObservableCollection<ChatMessage> messages = new ObservableCollection<ChatMessage>();
        ListView listView;

        protected override void OnAttachedTo(ListView bindable)
        {
            base.OnAttachedTo(bindable);

            listView = bindable;

            listView.PropertyChanged += ListView_PropertyChanged;
        }

        void ListView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ItemsSource")
            {
                messages = listView.ItemsSource as ObservableCollection<ChatMessage>;
                messages.CollectionChanged += Messages_CollectionChanged;
            }
        }

        void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var latestMessage = messages[messages.Count - 1];

            Device.BeginInvokeOnMainThread(() =>
            {
                listView.ScrollTo(latestMessage, ScrollToPosition.End, true);
            });
        }

        protected override void OnDetachingFrom(ListView bindable)
        {
            messages.CollectionChanged -= Messages_CollectionChanged;
        }
    }
}
