using Cadastro.Domain;
using Cadastro.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing.Drawing2D;

namespace Cadastro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriasController : ControllerBase
    {
        private readonly Contexto _context;

        public MateriasController(Contexto context)
        {
            _context = context;
        }

        // GET: api/Materias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MateriasModel>>> GetMaterias()
        {
            var materias = await _context.Materias
                .Select(u => new MateriasModel
                {
                    Materia_ID = u.Materia_ID,
                    Materia_Nome = u.Materia_Nome,
                    Materia_Bloco = u.Materia_Bloco,
                    Professor_Nome = u.Professor_Nome,
                    Professor_ID = u.Professor_ID
                })
                .ToListAsync();
            return Ok(materias);
        }

        // GET: api/Materias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MateriasModel>> GetMateria(string id)
        {
            var materia = await _context.Materias.FindAsync(id);

            if (materia == null)
            {
                return NotFound();
            }

            //return new MateriasModel
            MateriasModel m = new()
            {
                Materia_ID = materia.Materia_ID,
                Materia_Nome = materia.Materia_Nome,
                Materia_Bloco = materia.Materia_Bloco,
                Professor_ID = materia.Professor_ID,
                Professor_Nome = materia.Professor_Nome,
            };
            return m;
        }

        // POST: api/Materias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<bool>> PostMateria(MateriasModel Materia, string id)
        {
            var materiaExiste = await _context.Materias.FindAsync(id);
            if (materiaExiste == null)
            {
                _context.Materias.Add(new Materias
                {
                    Materia_ID = Materia.Materia_ID,
                    Materia_Nome = Materia.Materia_Nome,
                    Materia_Bloco = Materia.Materia_Bloco,
                    Professor_ID = Materia.Professor_ID,
                    Professor_Nome = Materia.Professor_Nome,
                });
                _context.SaveChanges();

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (MateriaExists(Materia.Materia_ID))
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
                var m = await _context.Materias.FindAsync(id);
                //m.Materia_ID = Materia.Materia_ID;
                m.Materia_Nome = Materia.Materia_Nome;
                m.Materia_Bloco = Materia.Materia_Bloco;
                m.Professor_ID = Materia.Professor_ID;
                m.Professor_Nome = Materia.Professor_Nome;

                _context.Materias.Update(m);

                await _context.SaveChangesAsync();
                return true;
            }
        }

        // DELETE: api/Materias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMateria(string id)
        {
            var materia = await _context.Materias.FindAsync(id);
            if (materia == null)
            {
                return NotFound();
            }
            var relacoesDelete = await _context.MateriasUsuario
                .Where(r => r.Materia_ID == materia.Materia_ID)
                .Select(r => r.Materia_ID).ToListAsync();

            foreach (string toDelete in relacoesDelete)
            {
                var materiaToDelete = await _context.MateriasUsuario.FirstOrDefaultAsync(r => r.Materia_ID == toDelete);
                _context.MateriasUsuario.Remove(materiaToDelete);
                await _context.SaveChangesAsync();
            }

            _context.Materias.Remove(materia);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MateriaExists(string id)
        {
            return _context.Materias.Any(e => e.Materia_ID == id);
        }
    }
}
