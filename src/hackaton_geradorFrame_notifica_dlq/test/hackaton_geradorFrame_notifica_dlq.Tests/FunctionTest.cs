using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.SQSEvents;
using hackaton_geradorFrame_notifica_dlq.Domain.Model;
using hackaton_geradorFrame_notifica_dlq.Infra.Interface;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.Json;
using Xunit;

namespace hackaton_geradorFrame_notifica_dlq.Test
{
    public class FunctionTests
    {
        private readonly Mock<IMessage> _messageMock;
        private readonly Mock<IRequisitanteRepository> _requisitanteRepositoryMock;
        private readonly Mock<ILambdaContext> _contextMock;
        private readonly Mock<ILambdaLogger> _loggerMock;
        private readonly Function _function;

        public FunctionTests()
        {
            _messageMock = new Mock<IMessage>();
            _requisitanteRepositoryMock = new Mock<IRequisitanteRepository>();
            _contextMock = new Mock<ILambdaContext>();
            _loggerMock = new Mock<ILambdaLogger>();

            _contextMock.Setup(x => x.Logger).Returns(_loggerMock.Object);
            var services = new ServiceCollection();
            services.AddSingleton(_messageMock.Object);
            services.AddSingleton(_requisitanteRepositoryMock.Object);
            var serviceProvider = services.BuildServiceProvider();

            _function = new Function(serviceProvider);
        }

        [Fact(DisplayName = "ProcessMessages_success")]
        public void FunctionHandlerShouldProcessMessages()
        {
            // Arrange
            var s3Event = new S3Event
            {
                Records = new List<S3Event.S3EventNotificationRecord>
            {
                new S3Event.S3EventNotificationRecord
                {
                    S3 = new S3Event.S3Entity
                    {
                        Object = new S3Event.S3ObjectEntity
                        {
                            Key = $"{Guid.NewGuid()}/video.mp4"
                        }
                    }
                }
            }
            };

            var sqsEvent = new SQSEvent
            {
                Records = new List<SQSEvent.SQSMessage>
                {
                new SQSEvent.SQSMessage()
                {
                    Body = JsonSerializer.Serialize(s3Event).ToLower().Replace("records", "Records"),
                    ReceiptHandle = "teste"
                }
                }
            };

            _requisitanteRepositoryMock.Setup(repo => repo.GetById(It.IsAny<Guid>()))
                .ReturnsAsync(new Requisitante { Email = "teste@example.com" });
            _messageMock.Setup(x => x.SendMessage(It.IsAny<string>())).Returns(Task.FromResult(true));
            _messageMock.Setup(x => x.DeleteMessage(It.IsAny<string>())).Returns(Task.FromResult(true));

            // Act
            var result = _function.FunctionHandler(sqsEvent, _contextMock.Object);

            // Assert
            Assert.Equal(TaskStatus.RanToCompletion, result.Status);
        }
    }
}