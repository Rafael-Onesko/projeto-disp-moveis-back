using Cadastro.Domain;
using Cadastro.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cadastro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColaboradoresController : ControllerBase
    {
        private readonly Contexto _context;

        public ColaboradoresController(Contexto context)
        {
            _context = context;
        }

        // GET: api/Colaboradores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Colaborador>>> GetColaboradores(CancellationToken cancellation)
        {
            var colaboradores = await _context.Colaboradores
                .Join(_context.PessoasFisicas, c => c.PessoaFisica, p => p.Id, (c, p) => new { c, p })
                .Where(cp => cp.c.PessoaFisica == cp.p.Id)
                .Select(cp => new ColaboradoresModel { Id = cp.c.Id, Nome = cp.p.Nome, Matricula = cp.c.Matricula, TipoColaborador = cp.c.Tipo })
                .ToListAsync(cancellation);

            return Ok(colaboradores);

        }

        [HttpGet("matricula/{matricula}")] 
        public async Task<ActionResult<bool>> MatriculaExiste(int matricula)
        {
            var colaborador = await _context.Colaboradores.Where(x => x.Matricula == matricula).ToListAsync();
            return colaborador[0].Matricula == matricula;
        }

        // GET: api/Colaboradores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ColaboradorModel>> GetColaborador(int id, CancellationToken cancellation)
        {
            var colaborador = await _context.Colaboradores.FindAsync(new object[] { id }, cancellationToken: cancellation);
            if (colaborador == null)
            {
                return NotFound();
            }
            var pessoafisica = await _context.PessoasFisicas.FindAsync(new object[] { colaborador.PessoaFisica }, cancellationToken: cancellation);

            ColaboradorModel colab = new()
            {
                IdColab = colaborador.Id,
                Matricula = colaborador.Matricula,
                ValorContribuicao = colaborador.ValorContribuicao,
                TipoColaborador = colaborador.Tipo,
                DataAdmissao = colaborador.DataAdmissao,
                PessoaFisica = new PessoaFisicaModel()
                {
                    IdPf = pessoafisica.Id,
                    Nome = pessoafisica.Nome,
                    CPF = pessoafisica.CPF,
                    DataNascimento = pessoafisica.DataNascimento,
                    RG = pessoafisica.RG,
                }
            };

            //Endereços
            var enderecos = await _context.Enderecos
                .Join(_context.Colaboradores, e => e.PessoaFisica, c => colaborador.PessoaFisica, (e, c) => new { e, c })
                .Where(ec => ec.e.PessoaFisica == ec.c.PessoaFisica)
                .Select(ec => new EnderecoModel
                {
                    IdEnd = ec.e.Id,
                    CEP = ec.e.CEP,
                    Logradouro = ec.e.Logradouro,
                    NumeroEndereco = ec.e.Numero,
                    Bairro = ec.e.Bairro,
                    Cidade = ec.e.Cidade,
                    TipoEndereco = ec.e.Tipo
                }).ToListAsync(cancellation);

            foreach (EnderecoModel endereco in enderecos)
            {
                colab.Enderecos.Add(endereco);
            }

            //Telefones
            var telefones = await _context.Telefones
                .Join(_context.Colaboradores, t => t.PessoaFisica, c => colaborador.PessoaFisica, (t, c) => new { t, c })
                .Where(tc => tc.t.PessoaFisica == tc.c.PessoaFisica)
                .Select(tc => new TelefoneModel { IdTel = tc.t.Id, NumeroTelefone = tc.t.Numero, TipoTelefone = tc.t.Tipo }).ToListAsync(cancellation);

            foreach (TelefoneModel telefone in telefones)
            {
                colab.Telefones.Add(telefone);
            }
            return colab;
        }

        //POST: api/Colaboradores
        //To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task< ActionResult<bool>> PostColaborador(ColaboradorModel colaborador, int id = 0, CancellationToken cancellation = default )
        {
            if (id == 0)
            {
                _context.PessoasFisicas.Add(new PessoaFisica()
                {
                    CPF = colaborador.PessoaFisica.CPF,
                    Nome = colaborador.PessoaFisica.Nome,
                    DataNascimento = new DateTime(colaborador.PessoaFisica.DataNascimento.Year, 
                                                  colaborador.PessoaFisica.DataNascimento.Month, 
                                                  colaborador.PessoaFisica.DataNascimento.Day),
                    RG = colaborador.PessoaFisica.RG
                });
                _context.SaveChanges();
                var pf = _context.PessoasFisicas.OrderByDescending(p => p.Id).First();
                _context.Colaboradores.Add(new Colaborador()
                {
                    PessoaFisica = pf.Id,
                    Tipo = colaborador.TipoColaborador,
                    Matricula = colaborador.Matricula,
                    DataAdmissao = new DateTime(colaborador.DataAdmissao.Value.Year, colaborador.DataAdmissao.Value.Month, colaborador.DataAdmissao.Value.Day),
                    ValorContribuicao = colaborador.ValorContribuicao
                });

                foreach (var endereco in colaborador.Enderecos)
                {
                    _context.Enderecos.Add(new Endereco()
                    {
                        PessoaFisica = pf.Id,
                        Logradouro = endereco.Logradouro,
                        Numero = endereco.NumeroEndereco,
                        Bairro = endereco.Bairro,
                        Cidade = endereco.Cidade,
                        CEP = endereco.CEP,
                        Tipo = endereco.TipoEndereco
                    });
                }
                foreach (var telefone in colaborador.Telefones)
                {
                    _context.Telefones.Add(new Telefone() { PessoaFisica = pf.Id, Numero = telefone.NumeroTelefone, Tipo = telefone.TipoTelefone });
                }
                await _context.SaveChangesAsync(cancellation);
                return true;
            }
            else
            {
                var colab = await _context.Colaboradores.FindAsync(new object[] { id }, cancellationToken: cancellation);
                if (colab == null)
                {
                    return NotFound();
                }
                var pessoafisica = await _context.PessoasFisicas.FindAsync(new object[] { colab.PessoaFisica }, cancellationToken: cancellation);

                var enderecosColab = await _context.Enderecos
                    .Join(_context.Colaboradores, e => e.PessoaFisica, c => colab.PessoaFisica, (e, c) => new { e, c })
                    .Where(ec => ec.e.PessoaFisica == ec.c.PessoaFisica)
                    .Select(ec => new Endereco
                    {
                        Id = ec.e.Id,
                        CEP = ec.e.CEP,
                        Logradouro = ec.e.Logradouro,
                        Numero = ec.e.Numero,
                        Bairro = ec.e.Bairro,
                        Cidade = ec.e.Cidade,
                        Tipo = ec.e.Tipo
                    }).ToListAsync(cancellationToken: cancellation);     

                var telefonesColab = await _context.Telefones
                    .Join(_context.Colaboradores, t => t.PessoaFisica, c => colab.PessoaFisica, (t, c) => new { t, c })
                    .Where(tc => tc.t.PessoaFisica == tc.c.PessoaFisica)
                    .Select(tc => new Telefone { Id = tc.t.Id, Numero= tc.t.Numero, Tipo= tc.t.Tipo }).ToListAsync(cancellationToken: cancellation);

                pessoafisica.Id = pessoafisica.Id;
                pessoafisica.CPF = colaborador.PessoaFisica.CPF;
                pessoafisica.Nome = colaborador.PessoaFisica.Nome;
                pessoafisica.DataNascimento = new DateTime(colaborador.PessoaFisica.DataNascimento.Year, colaborador.PessoaFisica.DataNascimento.Month, colaborador.PessoaFisica.DataNascimento.Day);
                pessoafisica.RG = colaborador.PessoaFisica.RG;
                
                _context.PessoasFisicas.Update(pessoafisica);

                colab.Id = colab.Id;
                colab.PessoaFisica = pessoafisica.Id;
                colab.Tipo = colaborador.TipoColaborador;
                colab.Matricula = colaborador.Matricula;
                colab.DataAdmissao = new DateTime(colaborador.DataAdmissao.Value.Year, colaborador.DataAdmissao.Value.Month, colaborador.DataAdmissao.Value.Day);
                colab.ValorContribuicao = colaborador.ValorContribuicao;

                _context.Colaboradores.Update(colab);

                var endAtuaisId = enderecosColab.Select(x => x.Id).ToList();
                var endUpId = colaborador.Enderecos.Select(x => x.IdEnd).ToList();
                var enderecoDelete = endAtuaisId.Except(endUpId).ToList();
           
                foreach (int toDelete in enderecoDelete)
                {
                    var enderecoToDelete = await _context.Enderecos.FindAsync(new object[] { toDelete }, cancellationToken: cancellation);
                    _context.Enderecos.Remove(enderecoToDelete);
                    await _context.SaveChangesAsync(cancellation);
                }

                foreach (EnderecoModel endereco in colaborador.Enderecos)
                {
   
                    var enderecoUpdate = await _context.Enderecos.FindAsync(new object[] { endereco.IdEnd }, cancellationToken: cancellation);
                    if (enderecoUpdate == null)
                    {
                        _context.Enderecos.Add(new Endereco()
                        {
                            PessoaFisica = pessoafisica.Id,
                            Logradouro = endereco.Logradouro,
                            Numero = endereco.NumeroEndereco,
                            Bairro = endereco.Bairro,
                            Cidade = endereco.Cidade,
                            CEP = endereco.CEP,
                            Tipo = endereco.TipoEndereco
                        });

                    }
                    else
                    {
                        enderecoUpdate.Id = endereco.IdEnd;
                        enderecoUpdate.PessoaFisica = pessoafisica.Id;
                        enderecoUpdate.Logradouro = endereco.Logradouro;
                        enderecoUpdate.Numero = endereco.NumeroEndereco;
                        enderecoUpdate.Bairro = endereco.Bairro;
                        enderecoUpdate.Cidade = endereco.Cidade;
                        enderecoUpdate.CEP = endereco.CEP;
                        enderecoUpdate.Tipo = endereco.TipoEndereco;
                        _context.Enderecos.Update(enderecoUpdate);
                    }
                    await _context.SaveChangesAsync(cancellation);
                }

                var telAtuaisId = telefonesColab.Select(x => x.Id).ToList();
                var telUpId = colaborador.Telefones.Select(x => x.IdTel).ToList();
                var telDelete = telAtuaisId.Except(telUpId).ToList();

                foreach (int toDelete in telDelete)
                {
                    var telefoneToDelete = await _context.Telefones.FindAsync(new object[] { toDelete }, cancellationToken: cancellation);
                    _context.Telefones.Remove(telefoneToDelete);
                    await _context.SaveChangesAsync(cancellation);
                }

                foreach (TelefoneModel telefone in colaborador.Telefones)
                {

                    var telefoneUpdate = await _context.Telefones.FindAsync(new object[] { telefone.IdTel }, cancellationToken: cancellation);
                    if (telefoneUpdate == null)
                    {
                        _context.Telefones.Add(new Telefone()
                        {
                            PessoaFisica = pessoafisica.Id,
                            Numero = telefone.NumeroTelefone,
                            Tipo = telefone.TipoTelefone
                        });
                    }
                    else
                    {
                        telefoneUpdate.Id  = telefone.IdTel;
                        telefoneUpdate.PessoaFisica = pessoafisica.Id;
                        telefoneUpdate.Numero = telefone.NumeroTelefone;
                        telefoneUpdate.Tipo = telefone.TipoTelefone;
                        _context.Telefones.Update(telefoneUpdate);
                    }
                    await _context.SaveChangesAsync(cancellation);
                }
                await _context.SaveChangesAsync(cancellation);
                return true;
            }  
        }

        // DELETE: api/Colaboradores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColaborador(int id, CancellationToken cancellation)
        {
            var colaborador = await _context.Colaboradores.FindAsync(new object[] { id }, cancellationToken: cancellation);
            if (colaborador == null)
            {
                return NotFound();
            }
            var pessoafisica = await _context.PessoasFisicas.FindAsync(new object[] { colaborador.PessoaFisica }, cancellationToken: cancellation);

            var enderecoDelete = await _context.Enderecos
                .Join(_context.Colaboradores, e => e.PessoaFisica, c => colaborador.PessoaFisica, (e, c) => new { e, c })
                .Where(ec => ec.e.PessoaFisica == ec.c.PessoaFisica)
                .Select(ec => ec.e.Id)
                .ToListAsync(cancellation);

            foreach (int toDelete in enderecoDelete)
            {
                var enderecoToDelete = await _context.Enderecos.FindAsync(new object[] { toDelete }, cancellationToken: cancellation);
                _context.Enderecos.Remove(enderecoToDelete);
                await _context.SaveChangesAsync(cancellation);
            }

            var telDelete = await _context.Telefones
                .Join(_context.Colaboradores, t => t.PessoaFisica, c => colaborador.PessoaFisica, (t, c) => new { t, c })
                .Where(tc => tc.t.PessoaFisica == tc.c.PessoaFisica)
                .Select(tc => tc.t.Id)
                .ToListAsync(cancellation);

            foreach (int toDelete in telDelete)
            {
                var telefoneToDelete = await _context.Telefones.FindAsync(new object[] { toDelete }, cancellationToken: cancellation);
                _context.Telefones.Remove(telefoneToDelete);
                await _context.SaveChangesAsync(cancellation);
            }

            _context.Colaboradores.Remove(colaborador);
            await _context.SaveChangesAsync(cancellation);

            _context.PessoasFisicas.Remove(pessoafisica);
            await _context.SaveChangesAsync(cancellation);

            return NoContent();
        }
    }
}




