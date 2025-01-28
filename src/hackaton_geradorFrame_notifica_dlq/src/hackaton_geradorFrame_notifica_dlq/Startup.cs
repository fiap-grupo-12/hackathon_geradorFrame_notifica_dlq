using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.SQS;
using hackaton_geradorFrame_notifica_dlq.Infra;
using hackaton_geradorFrame_notifica_dlq.Infra.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace hackaton_geradorFrame_notifica_dlq
{
    [ExcludeFromCodeCoverage]
    public static class Startup
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
         
            services.AddTransient<IMessage, Message>();
            services.AddTransient<IRequisitanteRepository, RequisitanteRepository>();
            services.AddAWSService<IAmazonSQS>();
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddTransient<IDynamoDBContext, DynamoDBContext>();
            services.AddLogging();

            services.AddCors();

            return services.BuildServiceProvider();
        }
    }
}
