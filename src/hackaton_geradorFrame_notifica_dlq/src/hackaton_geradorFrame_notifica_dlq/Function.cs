using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using hackaton_geradorFrame_notifica_dlq.Domain.Model;
using hackaton_geradorFrame_notifica_dlq.Infra.Interface;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace hackaton_geradorFrame_notifica_dlq
{
    public class Function
    {
        private readonly IMessage _message;
        private readonly IRequisitanteRepository _requisitanteRepository;

        public Function()
        : this(Startup.ConfigureServices())
        {
        }

        public Function(IServiceProvider serviceProvider)
        {
            _message = serviceProvider.GetService<IMessage>();
            _requisitanteRepository = serviceProvider.GetService<IRequisitanteRepository>();
        }

        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            context.Logger.LogInformation($"Mensagem Recebida: {JsonSerializer.Serialize(evnt)}");

            var dadosS3 = JsonSerializer.Deserialize<S3MessageEvent>(evnt.Records[0].Body);

            var receiptHandle = evnt.Records[0].ReceiptHandle;

            foreach (var message in dadosS3.Records)
            {
                await ProcessMessageAsync(message.s3, receiptHandle, context);
            }
        }

        private async Task ProcessMessageAsync(S3 message, string receiptHandle, ILambdaContext context)
        {
            context.Logger.LogInformation($"Processando a mensagem: {message._object.key}");

            var arquivoRequest = new Arquivo().GetArquivo(message._object.key);

            context.Logger.LogInformation($"Buscando dados requisitante. {message._object.key}");
            var requisitante = await _requisitanteRepository.GetById(arquivoRequest.IdRequest);

            if (requisitante == null)
                throw new Exception("Requisitante nao encontrado na base.");

            var jsonBody = new Email().GerarEmailJsonErro(requisitante, arquivoRequest.Video);

            context.Logger.LogInformation($"Enviando notificação. {message._object.key}");
            await _message.SendMessage(jsonBody);

            context.Logger.LogInformation($"Deletando mensagem do SQS. {message._object.key}");
            await _message.DeleteMessage(receiptHandle);

            await Task.CompletedTask;
        }
    }
}