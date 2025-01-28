namespace hackaton_geradorFrame_notifica_dlq.Domain.Model
{
    public class Arquivo
    {
        public Guid IdRequest { get; set; }
        public string Video { get; set; }

        public Arquivo GetArquivo(string KeyS3)
        {

            if (string.IsNullOrEmpty(KeyS3))
                throw new ArgumentNullException("KeyS3 está nulo");

            return new Arquivo
            {
                IdRequest = Guid.Parse(KeyS3.Split("/")[0]),
                Video = KeyS3.Split("/")[1]
            };

        }
    }
}