﻿using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class MessageParser<T, T2> : IMessageParser<T, T2> where T : MessageTemplateBase<T2>, new()
    {
        private readonly IMessageTemplateParser _messageTemplateParser;
        private readonly Site _site;
        private readonly IMessageTemplateProvider _messageTemplateProvider;
        //private readonly ISession _session;
        private readonly IEmailSender _emailSender;

        public MessageParser(IMessageTemplateParser messageTemplateParser, Site site, IMessageTemplateProvider messageTemplateProvider
            , IEmailSender emailSender)
        {
            _messageTemplateParser = messageTemplateParser;
            _site = site;
            _messageTemplateProvider = messageTemplateProvider;
            _emailSender = emailSender;
        }

        public QueuedMessage GetMessage(T2 obj, string fromAddress = null, string fromName = null, string toAddress = null, string toName = null, string cc = null, string bcc = null)
        {
            var template = _messageTemplateProvider.GetMessageTemplate<T>(_site);
            if (template == null)
                return null;

            return new QueuedMessage
            {
                FromAddress = _messageTemplateParser.Parse(fromAddress ?? template.FromAddress, obj),
                FromName = _messageTemplateParser.Parse(fromName ?? template.FromName, obj),
                ToAddress = _messageTemplateParser.Parse(toAddress ?? template.ToAddress, obj),
                ToName = _messageTemplateParser.Parse(toName ?? template.ToName, obj),
                Cc = _messageTemplateParser.Parse(cc ?? template.Cc, obj),
                Bcc = _messageTemplateParser.Parse(bcc ?? template.Bcc, obj),
                Subject = _messageTemplateParser.Parse(template.Subject, obj),
                Body = _messageTemplateParser.Parse(template.Body, obj),
                IsHtml = template.IsHtml
            };
        }

        public void QueueMessage(QueuedMessage queuedMessage, bool trySendImmediately = true)
        {
            if (queuedMessage != null)
            {
                if (trySendImmediately)
                {
                    _emailSender.SendMailMessage(queuedMessage);
                }
                _emailSender.AddToQueue(queuedMessage);
            }
        }
    }
}