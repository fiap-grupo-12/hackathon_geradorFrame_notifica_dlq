using Amazon.DynamoDBv2.DataModel;
using System.Diagnostics.CodeAnalysis;

namespace hackaton_geradorFrame_notifica_dlq.Domain.Model
{
    [ExcludeFromCodeCoverage]
    [DynamoDBTable("GerenciadorTable")]
    public class Requisitante
    {
        [DynamoDBHashKey("id")]
        public Guid Id { get; set; }

        [DynamoDBProperty("Nome")]
        public string Nome { get; set; }

        [DynamoDBProperty("Email")]
        public string Email { get; set; }
    }
}
