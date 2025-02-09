using hackaton_geradorFrame_notifica_dlq.Domain.Model;
using System.Text.Json;
using Xunit;

namespace hackaton_geradorFrame_notifica_dlq.Tests
{
    public class ModelTest
    {
        [Fact]
        public void GerarEmailErro_DeveRetornarEmailCorreto()
        {
            // Arrange
            var requisitante = new Requisitante { Email = "teste@example.com" };
            var nomeVideo = "video.mp4";
            var emailService = new Email();

            // Act
            var email = emailService.GerarEmailErro(requisitante, nomeVideo);

            // Assert
            Assert.Equal("teste@example.com", email.Destinatario);
            Assert.Equal("Usuario", email.Nome);
            Assert.Equal("Erro ao processar video.", email.Assunto);
            Assert.Equal("O sistema nao conseguiu processar o video video.mp4.", email.Mensagem);
        }

        [Fact]
        public void GerarEmailJsonErro_DeveRetornarJsonCorreto()
        {
            // Arrange
            var requisitante = new Requisitante { Email = "teste@example.com" };
            var nomeVideo = "video.mp4";
            var emailService = new Email();
            var email = new Email
            {
                Destinatario = "teste@example.com",
                Nome = "Usuario",
                Assunto = "Erro ao processar video.",
                Mensagem = "O sistema nao conseguiu processar o video video.mp4."
            };
            var expectedJson = JsonSerializer.Serialize(email);

            // Act
            var jsonResult = emailService.GerarEmailJsonErro(requisitante, nomeVideo);

            // Assert
            Assert.Equal(expectedJson, jsonResult);
        }

        [Fact]
        public void GetArquivo_DeveRetornarArquivoCorreto()
        {
            // Arrange
            var keyS3 = "123e4567-e89b-12d3-a456-426614174000/video.mp4";
            var expectedIdRequest = Guid.Parse("123e4567-e89b-12d3-a456-426614174000");
            var expectedVideo = "video.mp4";
            var arquivo = new Arquivo();

            // Act
            var result = arquivo.GetArquivo(keyS3);

            // Assert
            Assert.Equal(expectedIdRequest, result.IdRequest);
            Assert.Equal(expectedVideo, result.Video);
        }

        [Fact]
        public void GetArquivo_DeveLancarFormatExceptionParaGuidInvalido()
        {
            // Arrange
            var keyS3 = "invalid-guid/video.mp4";
            var arquivo = new Arquivo();

            // Act & Assert
            Assert.Throws<FormatException>(() => arquivo.GetArquivo(keyS3));
        }

        [Fact]
        public void GetArquivo_DeveLancarIndexOutOfRangeExceptionParaKeyS3Invalido()
        {
            // Arrange
            var keyS3 = "123e4567-e89b-12d3-a456-426614174000";
            var arquivo = new Arquivo();

            // Act & Assert
            Assert.Throws<IndexOutOfRangeException>(() => arquivo.GetArquivo(keyS3));
        }

        [Fact]
        public void GetArquivo_DeveLancarArgumentNullExceptionParaKeyS3Nulo()
        {
            // Arrange
            string keyS3 = null;
            var arquivo = new Arquivo();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => arquivo.GetArquivo(keyS3));
        }
    }
}
