using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BotExample.Model;
using BotExample.ViewModel.Helpers;
using Xamarin.Forms;

namespace BotExample.ViewModel
{
    public class ChatVM : INotifyPropertyChanged
    {
        BotServiceHelper botHelper;

        public ObservableCollection<ChatMessage> Messages { get; set; }

        public Command SendCommand { get; set; }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ChatVM()
        {
            botHelper = new BotServiceHelper();

            SendCommand = new Command<string>(SendMessage, SendCanExecute);
            Messages = new ObservableCollection<ChatMessage>();

            botHelper.MessageReceived += BotHelper_MessageReceived;
        }

        void BotHelper_MessageReceived(object sender, BotResponseEventArgs e)
        {
            foreach(var activity in e.Activities)
            {
                if(activity.From.Id != "usuario1")
                {
                    Messages.Add(new ChatMessage
                    {
                        IsIncoming = true,
                        Message = activity.Text
                    });
                }
            }
        }

        bool SendCanExecute(string msg)
        {
            return !string.IsNullOrWhiteSpace(msg);
        }

        void SendMessage(object obj)
        {
            ChatMessage chatMessage = new ChatMessage
            {
                Message = this.Message,
                IsIncoming = false
            };

            botHelper.SendMessage(Message);
            Messages.Add(chatMessage);

            Message = string.Empty;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
