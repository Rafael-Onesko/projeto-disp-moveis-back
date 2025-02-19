using Cadastro.Domain;
using Cadastro.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cadastro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly Contexto _context;

        public UsuariosController(Contexto context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuariosModel>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new UsuariosModel { Email = u.Email, Nome = u.PrimeiroNome + " " + u.UltimoNome })
                .ToListAsync();
            return Ok(usuarios);
        }

        // GET: api/Usuarios/5
        [HttpGet("{email}")]
        public async Task<ActionResult<UsuarioModel>> GetUsuario(string email)
        {
            var usuario = await _context.Usuarios.FindAsync(email);

            if (usuario == null)
            {
                return NotFound();
            }
            return new UsuarioModel
            { 
                Email = usuario.Email,
                PrimeiroNome = usuario.PrimeiroNome,
                UltimoNome = usuario.UltimoNome,
                Senha = usuario.Senha,
                Admin = usuario.Admin
            };

        }

        public class AuthChecker
        {
            public string Email { get; set; }
            public string Senha { get; set; }
        }

        [HttpPost("Auth")]
        public ActionResult<bool> GetUsuarioSenha(AuthChecker auth)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => ((u.Email == auth.Email)));

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario.Senha == auth.Senha;
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<bool>> PostUsuario(UsuarioModel usuario, string email = null)
        {
            if (email == null) { 
                _context.Usuarios.Add(new Usuario() 
                { 
                    Email = usuario.Email,
                    PrimeiroNome = usuario.PrimeiroNome,
                    UltimoNome = usuario.UltimoNome,
                    Senha = usuario.Senha,
                    Admin = usuario.Admin
                });
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (UsuarioExists(usuario.Email))
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
                var user = await _context.Usuarios.FindAsync(email);
                user.Email = usuario.Email;
                user.PrimeiroNome = usuario.PrimeiroNome;
                user.UltimoNome = usuario.UltimoNome;
                user.Senha = usuario.Senha;
                user.Admin = usuario.Admin;
                
                _context.Usuarios.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(string id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool UsuarioExists(string id)
        {
            return _context.Usuarios.Any(e => e.Email == id);
        }
    }
}
