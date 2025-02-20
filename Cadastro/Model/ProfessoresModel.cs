namespace Cadastro.Model
{
    public class ProfessorModel
    {
        public string Professor_ID { get; set; }
        public string Professor_Nome { get; set; }
        public List<MateriasModel> MateriasProfessor { get; set; }
        public ProfessorModel()
        {
            MateriasProfessor = new List<MateriasModel>();
        }
    }
}
