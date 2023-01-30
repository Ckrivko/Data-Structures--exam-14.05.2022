using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.Discord
{
    public class Discord : IDiscord
    {
        //private HashSet<Message> messages= new HashSet<Message>();
        private Dictionary<string, Message> messagesById = new Dictionary<string, Message>();
        private Dictionary<string, List<Message>> chanelList = new Dictionary<string, List<Message>>();

        public int Count => this.messagesById.Count;

        public bool Contains(Message message)
        {
            return this.messagesById.ContainsKey(message.Id);
        }

        public void DeleteMessage(string messageId)
        {
            if (!this.messagesById.ContainsKey(messageId))
            {
                throw new ArgumentException();
            }

            var currMessage = this.messagesById[messageId];

            this.messagesById.Remove(messageId);
            this.chanelList[currMessage.Channel].Remove(currMessage);
        }

        public IEnumerable<Message> GetAllMessagesOrderedByCountOfReactionsThenByTimestampThenByLengthOfContent()
        {

            return this.messagesById.Values.OrderByDescending(m => m.Reactions.Count)
                .ThenBy(m => m.Timestamp)
                .ThenBy(m => m.Content.Length);
        }

        public IEnumerable<Message> GetChannelMessages(string channel)
        {
            if (!chanelList.ContainsKey(channel))
            {
                throw new ArgumentException();
            }

            if (this.chanelList[channel].Count == 0)
            {
                throw new ArgumentException();
            }

            return this.chanelList[channel];
        }

        public Message GetMessage(string messageId)
        {
            if (!this.messagesById.ContainsKey(messageId))
            {
                throw new ArgumentException();
            }

            return messagesById[messageId];
        }

        public IEnumerable<Message> GetMessageInTimeRange(int lowerBound, int upperBound)
        {
            return this.messagesById.Values
                .Where(m => m.Timestamp >= lowerBound && m.Timestamp <= upperBound)
                .OrderByDescending(m => this.chanelList[m.Channel].Count)
                .ToArray();
        }

        public IEnumerable<Message> GetMessagesByReactions(List<string> reactions)
        {
            return this.messagesById.Values.Where(m => reactions.All(r => m.Reactions.Contains(r)))
                 .OrderByDescending(m => m.Reactions.Count)
                 .ThenBy(m => m.Timestamp)
                 .ToArray();                    //!days.Except(r.DaysOff).Any())
        }

        public IEnumerable<Message> GetTop3MostReactedMessages()
        {
            return this.messagesById.Values.OrderByDescending(m => m.Reactions.Count).Take(3).ToArray();
        }

        public void ReactToMessage(string messageId, string reaction)
        {
            if (!this.messagesById.ContainsKey(messageId))
            {
                throw new ArgumentException();
            }
            this.messagesById[messageId].Reactions.Add(reaction);
        }

        public void SendMessage(Message message)
        {
            if (messagesById.ContainsKey(message.Id))
            {
                throw new ArgumentException();
            }

            this.messagesById[message.Id] = message;


            if (!chanelList.ContainsKey(message.Channel))
            {
                chanelList[message.Channel] = new List<Message>();
            }

            chanelList[message.Channel].Add(message);

        }
    }
}
