using Cadastro.Domain;
using Cadastro.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cadastro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly Contexto _context;

        public RolesController(Contexto context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolesModel>>> GetRoles()
        {
            var Roles = await _context.Roles
                .Select(u => new RolesModel { Role_ID = u.Role_ID, RoleNome = u.RoleNome })
                .ToListAsync();
            return Ok(Roles);
        }

        // GET: api/Roles/5
        [HttpGet("{email}")]
        public async Task<ActionResult<RolesModel>> GetRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            //return new RolesModel
            RolesModel r = new()
            {
                Role_ID = role.Role_ID,
                RoleNome = role.RoleNome
            };
            return r;
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<bool>> PostRole(RolesModel Role, string id)
        {
            var RoleExiste = await _context.Roles.FindAsync(id);
            if (RoleExiste == null)
            {
                _context.Roles.Add(new Roles
                {
                    Role_ID = Role.Role_ID,
                    RoleNome = Role.RoleNome
                });
                _context.SaveChanges();

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (RoleExists(Role.Role_ID))
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
                var r = await _context.Roles.FindAsync(id);
                r.RoleNome = Role.RoleNome;

                _context.Roles.Update(r);

                await _context.SaveChangesAsync();
                return true;
            }
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var Role = await _context.Roles.FindAsync(id);
            if (Role == null)
            {
                return NotFound();
            }

            var relacoesDelete = await _context.RolesUsuario
                .Where(r => r.Role_ID == Role.Role_ID)
                .Select(r => r.Role_ID).ToListAsync();

            foreach (int toDelete in relacoesDelete)
            {
                var roleToDelete = await _context.RolesUsuario.FirstOrDefaultAsync(r => r.Role_ID == toDelete);
                _context.RolesUsuario.Remove(roleToDelete);
                await _context.SaveChangesAsync();
            }

            _context.Roles.Remove(Role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Role_ID == id);
        }
    }
}
