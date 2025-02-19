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
                .Select(u => new UsuariosModel { Login = u.Login, Nome = u.Nome })
                .ToListAsync();
            return Ok(usuarios);
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioModel>> GetUsuario(string id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }
            return new UsuarioModel
            { 
                Login = usuario.Login,
                Nome = usuario.Nome,
                Senha = usuario.Senha,
                Administrador = usuario.Administrador
            };

        }

        public class AuthChecker
        {
            public string Login { get; set; }
            public string Senha { get; set; }
        }

        [HttpPost("Auth")]
        public ActionResult<bool> GetUsuarioSenha(AuthChecker auth)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => ((u.Login == auth.Login)));

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario.Senha == auth.Senha;
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<bool>> PostUsuario(UsuarioModel usuario, string id = null)
        {
            if (id == null) { 
                _context.Usuarios.Add(new Usuario() 
                { 
                    Login = usuario.Login,
                    Nome = usuario.Nome,
                    Senha = usuario.Senha,
                    Administrador = usuario.Administrador
                });
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (UsuarioExists(usuario.Login))
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
                var user = await _context.Usuarios.FindAsync(id);
                user.Login = usuario.Login;
                user.Nome = usuario.Nome;
                user.Senha = usuario.Senha;
                user.Administrador = usuario.Administrador;
                
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
            return _context.Usuarios.Any(e => e.Login == id);
        }
    }
}
