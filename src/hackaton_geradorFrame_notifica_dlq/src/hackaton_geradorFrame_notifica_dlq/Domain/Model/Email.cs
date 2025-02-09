using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace hackaton_geradorFrame_notifica_dlq.Domain.Model
{
    [ExcludeFromCodeCoverage]
    public class Email
    {
        [JsonPropertyName("Email")]
        public string Destinatario { get; set; }

        [JsonPropertyName("Nome")]
        public string Nome { get; set; }

        [JsonPropertyName("Assunto")]
        public string Assunto { get; set; }

        [JsonPropertyName("Corpo")]
        public string Mensagem { get; set; }

        public Email GerarEmailErro(Requisitante requisitante, string nomeVideo) => new Email
        {
            Destinatario = requisitante.Email,
            Nome = "Usuario",
            Assunto = "Erro ao processar video.",
            Mensagem = $"O sistema nao conseguiu processar o video {nomeVideo}."
        };

        public string GerarEmailJsonErro(Requisitante requisitante, string nomeVideo)
        {
            var email = GerarEmailErro(requisitante, nomeVideo);
            return JsonSerializer.Serialize(email);
        }
    }
}
