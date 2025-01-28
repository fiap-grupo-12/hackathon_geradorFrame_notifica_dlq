using Amazon.DynamoDBv2.DataModel;
using hackaton_geradorFrame_notifica_dlq.Domain.Model;
using hackaton_geradorFrame_notifica_dlq.Infra;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace hackaton_geradorFrame_notifica_dlq.Tests
{
    public class RepositoryTest
    {
        private readonly Mock<IDynamoDBContext> _mockContext;
        private readonly Mock<ILogger<RequisitanteRepository>> _mockLogger;
        private readonly RequisitanteRepository _repository;

        public RepositoryTest()
        {
            _mockContext = new Mock<IDynamoDBContext>();
            _mockLogger = new Mock<ILogger<RequisitanteRepository>>();
            _repository = new RequisitanteRepository(_mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetById_DeveRetornarRequisitanteCorreto()
        {
            // Arrange
            var requisitanteId = Guid.NewGuid();
            var requisitante = new Requisitante { Id = requisitanteId, Nome = "Teste", Email = "teste@example.com" };
            _mockContext.Setup(c => c.LoadAsync<Requisitante>(requisitanteId, default)).ReturnsAsync(requisitante);

            // Act
            var result = await _repository.GetById(requisitanteId);

            // Assert
            Assert.Equal(requisitanteId, result.Id);
            Assert.Equal("Teste", result.Nome);
            Assert.Equal("teste@example.com", result.Email);
        }

        [Fact]
        public async Task GetById_DeveLancarExceptionParaErroNoContext()
        {
            // Arrange
            var requisitanteId = Guid.NewGuid();
            _mockContext.Setup(c => c.LoadAsync<Requisitante>(requisitanteId, default)).ThrowsAsync(new Exception("Erro no contexto"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _repository.GetById(requisitanteId));
            Assert.Contains($"Erro ao consultar Requisitante {requisitanteId}.", exception.Message);
        }
    }
}
