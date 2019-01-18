using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BotExample.Model;
using BotExample.ViewModel;
using Xamarin.Forms;

namespace BotExample.View
{
    public partial class ChatPage : ContentPage
    {
        //ChatVM ViewModel;

        public ChatPage()
        {
            InitializeComponent();

            //ViewModel = Resources["vm"] as ChatVM;
            //ViewModel.Messages.CollectionChanged += Messages_CollectionChanged;
        }

        /*void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var latestMessage = ViewModel.Messages[ViewModel.Messages.Count - 1];

            Device.BeginInvokeOnMainThread(() =>
            {
                chatListView.ScrollTo(latestMessage, ScrollToPosition.End, true);
            });
        }*/
    }
}
