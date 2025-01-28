using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace hackaton_geradorFrame_notifica_dlq.Domain.Model
{
    [ExcludeFromCodeCoverage]
    public class Email
    {
        [JsonPropertyName("destinatario")]
        public string Destinatario { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("assunto")]
        public string Assunto { get; set; }

        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; }

        public Email GerarEmailErro(Requisitante requisitante, string nomeVideo) => new Email
        {
            Destinatario = requisitante.Email,
            Nome = requisitante.Nome,
            Assunto = "Erro ao processar video.",
            Mensagem = $"O sistema nao conseguiu processar o video {nomeVideo}."
        };

        public string GerarEmailJsonErro(Requisitante requisitante, string nomeVideo)
        {
            var email = GerarEmailErro(requisitante, nomeVideo);
            string teste = JsonSerializer.Serialize(email);
            return teste;
        }
    }
}
