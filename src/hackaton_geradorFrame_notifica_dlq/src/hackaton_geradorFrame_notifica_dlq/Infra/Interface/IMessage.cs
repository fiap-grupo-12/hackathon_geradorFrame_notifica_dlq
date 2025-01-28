namespace hackaton_geradorFrame_notifica_dlq.Infra.Interface
{
    public interface IMessage
    {
        public Task<bool> SendMessage(string body);
    }
}
