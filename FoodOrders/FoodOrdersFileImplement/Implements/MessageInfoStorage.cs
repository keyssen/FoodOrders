﻿using FoodOrdersContracts.BindingModels;
using FoodOrdersContracts.SearchModels;
using FoodOrdersContracts.StoragesContracts;
using FoodOrdersContracts.ViewModels;
using FoodOrdersFileImplement.Models;

namespace FoodOrdersFileImplement.Implements
{
    public class MessageInfoStorage : IMessageInfoStorage
    {
        private readonly DataFileSingleton _source;
        public MessageInfoStorage()
        {
            _source = DataFileSingleton.GetInstance();
        }

        public MessageInfoViewModel? GetElement(MessageInfoSearchModel model)
        {
            if (model.MessageId != null)
            {
                return _source.Messages.FirstOrDefault(x => x.MessageId == model.MessageId)?.GetViewModel;
            }
            return null;
        }

        public List<MessageInfoViewModel> GetFilteredList(MessageInfoSearchModel model)
        {
            return _source.Messages
                .Where(x => x.ClientId == model.ClientId)
                .Select(x => x.GetViewModel)
                .ToList();
        }

        public List<MessageInfoViewModel> GetFullList()
        {
            return _source.Messages
                .Select(x => x.GetViewModel)
                .ToList();
        }

        public MessageInfoViewModel? Insert(MessageInfoBindingModel model)
        {
            var newMessage = MessageInfo.Create(model);
            if (newMessage == null)
            {
                return null;
            }
            _source.Messages.Add(newMessage);
            _source.SaveMessages();
            return newMessage.GetViewModel;
        }

        public MessageInfoViewModel? Update(MessageInfoBindingModel model)
        {
            var res = _source.Messages.FirstOrDefault(x => x.MessageId.Equals(model.MessageId));
            if (res != null)
            {
                res.Update(model);
                _source.SaveMessages();
            }
            return res?.GetViewModel;
        }
    }
}