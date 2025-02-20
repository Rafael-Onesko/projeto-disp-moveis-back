using Cadastro.Domain;
using Cadastro.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cadastro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessoresController : ControllerBase
    {
        private readonly Contexto _context;

        public ProfessoresController(Contexto context)
        {
            _context = context;
        }

        // GET: api/Professores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfessorModel>>> GetProfessores()
        {
            var professores = await _context.Professores
                .Select(u => new ProfessorModel { Professor_ID = u.Professor_ID, Professor_Nome = u.Professor_Nome })
                .ToListAsync();
            return Ok(professores);
        }

        // GET: api/Professores/5
        [HttpGet("{email}")]
        public async Task<ActionResult<ProfessorModel>> GetProfessor(string email)
        {
            var prof = await _context.Professores.FindAsync(email);

            if (prof == null)
            {
                return NotFound();
            }

            //return new ProfessorModel
            ProfessorModel professor = new()
            {
                Professor_ID = prof.Professor_ID,
                Professor_Nome = prof.Professor_Nome,
            };

            //Materias
            var materiasProfessor = await _context.Materias.Where(m => m.Professor_ID == prof.Professor_ID)
                .Select(m => new MateriasModel
                {
                    Materia_ID = m.Materia_ID,
                    Materia_Nome = m.Materia_Nome,
                    Materia_Bloco = m.Materia_Bloco,
                    Professor_ID = m.Professor_ID,
                    Professor_Nome = m.Professor_Nome,
                }).ToListAsync();

            foreach (MateriasModel materia in materiasProfessor)
            {
                professor.MateriasProfessor.Add(materia);
            }

            return professor;
        }

        // POST: api/Professores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<bool>> PostProfessor(ProfessorModel professor, string email)
        {
            var professorExiste = await _context.Professores.FindAsync(email);
            if (professorExiste == null)
            {
                _context.Professores.Add(new Professores()
                {
                    Professor_ID = professor.Professor_ID,
                    Professor_Nome = professor.Professor_Nome,
                });
                _context.SaveChanges();

                foreach (var materias in professor.MateriasProfessor)
                {
                    _context.Materias.Add(new Materias()
                    {
                        Materia_ID = materias.Materia_ID,
                        Materia_Nome = materias.Materia_Nome,
                        Materia_Bloco = materias.Materia_Bloco,
                        Professor_ID = materias.Professor_ID,
                        Professor_Nome = materias.Professor_Nome,
                    });
                }
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (ProfessorExists(professor.Professor_ID))
                    {
                        return false;
                    }
                    else
                    {
                        throw;
                    }
                }
                return true;
            }
            else
            {
                var prof = await _context.Professores.FindAsync(email);
                //prof.Professor_ID = professor.Professor_ID;
                prof.Professor_Nome = professor.Professor_Nome;

                var materiasProfessor = await _context.Materias.Where(m => m.Professor_ID == prof.Professor_ID)
                .Select(m => new MateriasModel
                {
                    Materia_ID = m.Materia_ID,
                    Materia_Nome = m.Materia_Nome,
                    Materia_Bloco = m.Materia_Bloco,
                    Professor_ID = m.Professor_ID,
                    Professor_Nome = m.Professor_Nome,
                }).ToListAsync();

                _context.Professores.Update(prof);

                var materiasAtuaisId = materiasProfessor.Select(x => x.Materia_ID).ToList();
                var materiasUpId = professor.MateriasProfessor.Select(x => x.Materia_ID).ToList();
                var materiasDelete = materiasAtuaisId.Except(materiasUpId).ToList();

                foreach (string toDelete in materiasDelete)
                {
                    var materiaToDelete = await _context.Materias.FirstOrDefaultAsync(r => r.Professor_ID == professor.Professor_ID && r.Materia_ID == toDelete);
                    _context.Materias.Remove(materiaToDelete);
                    await _context.SaveChangesAsync();
                }

                foreach (MateriasModel materia in professor.MateriasProfessor)
                {

                    var materiaUpdate = await _context.Materias.FirstOrDefaultAsync(r => r.Professor_ID == professor.Professor_ID && r.Materia_ID == materia.Materia_ID);
                    if (materiaUpdate == null)
                    {
                        _context.Materias.Add(new Materias()
                        {
                            Materia_ID = materia.Materia_ID,
                            Materia_Nome = materia.Materia_Nome,
                            Materia_Bloco = materia.Materia_Bloco,
                            Professor_ID = materia.Professor_ID,
                            Professor_Nome = materia.Professor_Nome,
                        });
                    }
                    else
                    {
                        materiaUpdate.Materia_ID = materia.Materia_ID;
                        materiaUpdate.Materia_Nome = materia.Materia_Nome;
                        materiaUpdate.Materia_Bloco = materia.Materia_Bloco;
                        materiaUpdate.Professor_ID = materia.Professor_ID;
                        materiaUpdate.Professor_Nome = materia.Professor_Nome;
                        _context.Materias.Update(materiaUpdate);
                    }
                    await _context.SaveChangesAsync();
                }
                await _context.SaveChangesAsync();
                return true;
            }
        }

        // DELETE: api/Professores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfessor(string id)
        {
            var professor = await _context.Professores.FindAsync(id);
            if (professor == null)
            {
                return NotFound();
            }

            
            var materiasDelete = await _context.Materias
                .Where(m => m.Professor_ID == professor.Professor_ID)
                .Select(m => m.Materia_ID)
                .ToListAsync();

            foreach (string toDelete in materiasDelete)
            {
                var materiaToDelete = await _context.Materias.FirstOrDefaultAsync(r => r.Materia_ID == toDelete);
                var relacaoToDelete = await _context.MateriasUsuario.FirstOrDefaultAsync(r => r.Materia_ID == toDelete);
                _context.MateriasUsuario.Remove(relacaoToDelete);
                _context.Materias.Remove(materiaToDelete);
                await _context.SaveChangesAsync();
            }

            _context.Professores.Remove(professor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProfessorExists(string id)
        {
            return _context.Professores.Any(e => e.Professor_ID == id);
        }
    }
}

