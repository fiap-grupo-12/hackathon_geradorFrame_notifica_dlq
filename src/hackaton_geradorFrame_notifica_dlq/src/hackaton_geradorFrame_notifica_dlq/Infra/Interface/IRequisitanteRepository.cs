using hackaton_geradorFrame_notifica_dlq.Domain.Model;

namespace hackaton_geradorFrame_notifica_dlq.Infra.Interface
{
    public interface IRequisitanteRepository
    {
        public Task<Requisitante> GetById(Guid id);
    }
}
