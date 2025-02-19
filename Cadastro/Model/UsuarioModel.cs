namespace Cadastro.Model
{
    public class UsuarioModel
    {
        public string Email { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public string Senha { get; set; }
        public bool Admin { get; set; }
    }
}
