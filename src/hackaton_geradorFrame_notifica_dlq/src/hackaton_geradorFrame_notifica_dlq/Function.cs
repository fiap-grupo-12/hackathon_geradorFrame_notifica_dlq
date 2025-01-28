using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
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

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function(IServiceProvider serviceProvider)
        {
            _message = serviceProvider.GetService<IMessage>();
            _requisitanteRepository = serviceProvider.GetService<IRequisitanteRepository>();
        }


        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="evnt">The event for the Lambda function handler to process.</param>
        /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
        /// <returns></returns>
            //[LambdaFunction]
        public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            context.Logger.LogInformation($"Mensagem Recebida: {JsonSerializer.Serialize(evnt)}");

            foreach (var message in evnt.Records)
            {
                await ProcessMessageAsync(message.S3, context);
            }
        }

        private async Task ProcessMessageAsync(S3Event.S3Entity message, ILambdaContext context)
        {
            context.Logger.LogInformation($"Processando a mensagem: {message.Object.Key}");

            var arquivoRequest = new Arquivo().GetArquivo(message.Object.Key);

            var requisitante = await _requisitanteRepository.GetById(arquivoRequest.IdRequest);

            if (requisitante == null)
                throw new Exception("Requisitante nao encontrado na base.");

            var jsonBody = new Email().GerarEmailJsonErro(requisitante, arquivoRequest.Video);

            await _message.SendMessage(jsonBody);

            await Task.CompletedTask;
        }
    }
}