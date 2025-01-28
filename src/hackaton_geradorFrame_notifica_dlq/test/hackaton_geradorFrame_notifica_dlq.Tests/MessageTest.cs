using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Message = hackaton_geradorFrame_notifica_dlq.Infra.Message;

namespace hackaton_geradorFrame_notifica_dlq.Tests
{
    public class MessageTest
    {
        private readonly Mock<IAmazonSQS> _mockAmazonSQS;
        private readonly Mock<ILogger<Message>> _mockLogger;
        private readonly Message _message;

        public MessageTest()
        {
            _mockAmazonSQS = new Mock<IAmazonSQS>();
            _mockLogger = new Mock<ILogger<Message>>();
            _message = new Message(_mockAmazonSQS.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SendMessage_DeveRetornarTrueQuandoMensagemEnviadaComSucesso()
        {
            // Arrange
            var body = "Test message";
            var sendMessageResponse = new SendMessageResponse();
            _mockAmazonSQS.Setup(sqs => sqs.SendMessageAsync(It.IsAny<SendMessageRequest>(), default)).ReturnsAsync(sendMessageResponse);

            // Act
            var result = await _message.SendMessage(body);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendMessage_DeveLancarExceptionQuandoErroAoEnviarMensagem()
        {
            // Arrange
            var body = "Test message";
            _mockAmazonSQS.Setup(sqs => sqs.SendMessageAsync(It.IsAny<SendMessageRequest>(), default)).ThrowsAsync(new Exception("Erro ao enviar mensagem"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _message.SendMessage(body));
            Assert.True(exception.Message.Contains("Erro ao enviar mensagem"));
        }
    }
}
