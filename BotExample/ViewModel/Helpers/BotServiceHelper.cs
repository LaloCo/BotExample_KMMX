using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BotExample.ViewModel.Helpers
{
    public class BotServiceHelper
    {
        private string base_url = "https://directline.botframework.com";

        public Conversation Conversation;

        public event EventHandler<BotResponseEventArgs> MessageReceived;

        public BotServiceHelper()
        {
            CreateConversation();
        }

        private async void CreateConversation()
        {
            string endpoint = "/v3/directline/conversations";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(base_url);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer X4WLSf0xHFs.cwA.I_8.xMawyout4qfuozRVHlQuvW61URWs469mB9Oz_XyEHYc");

                var response = await client.PostAsync(endpoint, null);
                string json = await response.Content.ReadAsStringAsync();

                Conversation = JsonConvert.DeserializeObject<Conversation>(json);
            }

            StartListeningForMessages();
        }

        private async void StartListeningForMessages()
        {
            var webSocket = new ClientWebSocket();
            var cancellationToken = new CancellationTokenSource();

            await webSocket.ConnectAsync(new Uri(Conversation.StreamUrl), cancellationToken.Token);

            await Task.Factory.StartNew(async () =>
            {
                while(true)
                {
                    WebSocketReceiveResult result;
                    var message = new ArraySegment<byte>(new byte[4096]);
                    do
                    {
                        result = await webSocket.ReceiveAsync(message, cancellationToken.Token);

                        try
                        {
                            if (result.MessageType != WebSocketMessageType.Text)
                                break;

                            var cleanedMessage = message.Skip(message.Offset).Take(result.Count).ToArray();
                            string messageJson = Encoding.UTF8.GetString(cleanedMessage);

                            BotResponse botResponse = JsonConvert.DeserializeObject<BotResponse>(messageJson);

                            var args = new BotResponseEventArgs();
                            args.Activities = botResponse.Activities;

                            MessageReceived?.Invoke(this, args);
                        }
                        catch (Exception) { }
                    } while (!result.EndOfMessage);
                }
            });
        }

        public async void SendMessage(string message)
        {
            string endpoint = $"/v3/directline/conversations/{Conversation.ConversationId}/activities";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(base_url);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer X4WLSf0xHFs.cwA.I_8.xMawyout4qfuozRVHlQuvW61URWs469mB9Oz_XyEHYc");

                Activity activity = new Activity
                {
                    Text = message,
                    Type = "message",
                    From = new ChannelAccount
                    {
                        Id = "usuario1"
                    }
                };

                string jsonActivity = JsonConvert.SerializeObject(activity);
                var buffer = Encoding.UTF8.GetBytes(jsonActivity);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(endpoint, byteContent);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(jsonResponse);
                string messageId = (string)obj.SelectToken("id");
            }
        }
    }

    public class BotResponseEventArgs : EventArgs
    {
        public List<Activity> Activities { get; set; }
    }

    public class BotResponse
    {
        public List<Activity> Activities { get; set; }
        public string Watermark { get; set; }
    }

    public class ChannelAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Activity
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public ChannelAccount From { get; set; }
        public DateTime LocalTimestamp { get; set; }
    }

    public class Conversation
    {
        public string ConversationId { get; set; }
        public string Token { get; set; }
        public int Expires_in { get; set; }
        public string StreamUrl { get; set; }
        public string ReferenceGrammarId { get; set; }
    }
}
