﻿using Amazon.SQS;
using Amazon.SQS.Model;
using hackaton_geradorFrame_notifica_dlq.Infra.Interface;
using Microsoft.Extensions.Logging;

namespace hackaton_geradorFrame_notifica_dlq.Infra
{
    public class Message : IMessage
    {
        private readonly IAmazonSQS _amazonSQS;
        private readonly string _url;
        private readonly ILogger<Message> _logger;
        public Message(IAmazonSQS amazonSQS, ILogger<Message> logger)
        {
            _amazonSQS = amazonSQS;
            _url = Environment.GetEnvironmentVariable("url_sqs_notificacao");
            _logger = logger;
        }

        public async Task<bool> SendMessage(string body)
        {
            var message = new SendMessageRequest()
            {
                QueueUrl = _url,
                MessageBody = body
            };

            try
            {
                _logger.LogInformation($"Enviando mensagem ao SQS {_url}. Mensagem: {body}");
                await _amazonSQS.SendMessageAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao enviar a mensagem: {ex}");
            }
        }
    }
}
