using Amazon.DynamoDBv2.DataModel;
using hackaton_geradorFrame_notifica_dlq.Domain.Model;
using hackaton_geradorFrame_notifica_dlq.Infra.Interface;
using Microsoft.Extensions.Logging;

namespace hackaton_geradorFrame_notifica_dlq.Infra
{
    public class RequisitanteRepository : IRequisitanteRepository
    {
        private readonly IDynamoDBContext _context;
        private readonly ILogger<RequisitanteRepository> _logger;

        public RequisitanteRepository(IDynamoDBContext context, ILogger<RequisitanteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Requisitante> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation($"Buscando dados do requisitante na tabela.");
                return await _context.LoadAsync<Requisitante>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao consultar Requisitante {id}. {ex}");

            }
        }
    }
}
