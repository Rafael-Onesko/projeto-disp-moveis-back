namespace Cadastro.Model
{
    public class ColaboradorModel
    {
        public List<EnderecoModel> Enderecos { get; set; }
        public List<TelefoneModel> Telefones { get; set; }
        public PessoaFisicaModel PessoaFisica { get; set; }
        public ColaboradorModel() 
        { 
            Enderecos = new List<EnderecoModel>();
            Telefones = new List<TelefoneModel>();
            PessoaFisica = new PessoaFisicaModel();
        }
        public int IdColab { get; set; }
        public int Matricula { get; set; }
        public decimal? ValorContribuicao { get; set; }
        public short TipoColaborador { get; set; }
        public DateTime? DataAdmissao { get; set; }
    }
    public class PessoaFisicaModel
    {
        public int IdPf { get; set; }
        public string Nome { get; set; }
        public decimal CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public string RG { get; set; }
    }
    public class TelefoneModel
    {
        public int IdTel { get; set; }
        public string NumeroTelefone { get; set; }
        public short TipoTelefone { get; set; }
    }
    public class EnderecoModel
    {
        public int IdEnd { get; set; }
        public decimal CEP { get; set; }
        public string Logradouro { get; set; }
        public string NumeroEndereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public short TipoEndereco { get; set; }
    }
}
