using Cadastro.Domain;
using Cadastro.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

            //return new UsuarioModel
            UsuarioModel user = new()
            {
                Email = usuario.Email,
                PrimeiroNome = usuario.PrimeiroNome,
                UltimoNome = usuario.UltimoNome,
                Senha = usuario.Senha,
                Admin = usuario.Admin
            };
            //Roles
            var rolesUsuario = await _context.RolesUsuario
                .Join(_context.Roles, r => r.Role_ID, u => u.Role_ID, (r, u) => new { r, u })
                .Where(ru => ru.r.user_ID == usuario.Email)
                .Select(ru => new RolesModel
                {
                    Role_ID = ru.u.Role_ID,
                    RoleNome = ru.u.RoleNome,
                }).ToListAsync();

            foreach (RolesModel role in rolesUsuario)
            {
                user.RolesUsuario.Add(role);
            }

            //Materias
            var materiasUsuario = await _context.MateriasUsuario
                .Join(_context.Materias, r => r.Materia_ID, u => u.Materia_ID, (r, u) => new { r, u })
                .Where(ru => ru.r.user_ID == usuario.Email)
                .Select(ru => new MateriasModel
                {
                    Materia_ID = ru.u.Materia_ID,
                    Materia_Nome = ru.u.Materia_Nome,
                    Materia_Bloco = ru.u.Materia_Bloco,
                    Professor_ID = ru.u.Professor_ID,
                    Professor_Nome = ru.u.Professor_Nome,

                }).ToListAsync();

            foreach (MateriasModel materia in materiasUsuario)
            {
                user.MateriasUsuario.Add(materia);
            }

            return user;
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
        public async Task<ActionResult<bool>> PostUsuario(UsuarioModel usuario, string email)
        {
            var usuarioExiste = await _context.Usuarios.FindAsync(email);
            if (usuarioExiste == null)
            {
                _context.Usuarios.Add(new Usuario()
                {
                    Email = usuario.Email,
                    PrimeiroNome = usuario.PrimeiroNome,
                    UltimoNome = usuario.UltimoNome,
                    Senha = usuario.Senha,
                    Admin = usuario.Admin
                });
                _context.SaveChanges();

                foreach (var roles in usuario.RolesUsuario)
                {
                    _context.RolesUsuario.Add(new RolesUsuario()
                    {
                        Role_ID = roles.Role_ID,
                        user_ID = usuario.Email
                    });
                }

                foreach (var materias in usuario.MateriasUsuario)
                {
                    _context.MateriasUsuario.Add(new MateriasUsuario()
                    {
                        Materia_ID = materias.Materia_ID,
                        user_ID = usuario.Email
                    });
                }
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
                //user.Email = usuario.Email;
                user.PrimeiroNome = usuario.PrimeiroNome;
                user.UltimoNome = usuario.UltimoNome;
                user.Senha = usuario.Senha;
                user.Admin = usuario.Admin;

                var rolesUsuario = await _context.RolesUsuario
                .Join(_context.Roles, r => r.Role_ID, u => u.Role_ID, (r, u) => new { r, u })
                .Where(ru => ru.r.user_ID == usuario.Email)
                .Select(ru => new RolesModel
                {
                    Role_ID = ru.u.Role_ID,
                    RoleNome = ru.u.RoleNome,
                }).ToListAsync();

                var materiasUsuario = await _context.MateriasUsuario
                    .Join(_context.Materias, r => r.Materia_ID, u => u.Materia_ID, (r, u) => new { r, u })
                    .Where(ru => ru.r.user_ID == usuario.Email)
                    .Select(ru => new MateriasModel
                    {
                        Materia_ID = ru.u.Materia_ID,
                        Materia_Nome = ru.u.Materia_Nome,
                        Materia_Bloco = ru.u.Materia_Bloco,
                        Professor_ID = ru.u.Professor_ID,
                        Professor_Nome = ru.u.Professor_Nome,

                    }).ToListAsync();

                _context.Usuarios.Update(user);

                var rolesAtuaisId = rolesUsuario.Select(x => x.Role_ID).ToList();
                var rolesUpId = usuario.RolesUsuario.Select(x => x.Role_ID).ToList();
                var rolesDelete = rolesAtuaisId.Except(rolesUpId).ToList();

                foreach (int toDelete in rolesDelete)
                {
                    var roleToDelete = await _context.RolesUsuario.FirstOrDefaultAsync(r => r.user_ID == usuario.Email && r.Role_ID == toDelete);
                    _context.RolesUsuario.Remove(roleToDelete);
                    await _context.SaveChangesAsync();
                }

                foreach (RolesModel role in usuario.RolesUsuario)
                {

                    var roleUpdate = await _context.RolesUsuario.FirstOrDefaultAsync(r => r.user_ID == usuario.Email && r.Role_ID == role.Role_ID);
                    
                    if (roleUpdate == null)
                    {
                        _context.RolesUsuario.Add(new RolesUsuario()
                        {
                            Role_ID = role.Role_ID,
                            user_ID = usuario.Email
                        });
                    }
                    else
                    {
                        roleUpdate.Role_ID = role.Role_ID;
                        roleUpdate.user_ID = usuario.Email;
                        _context.RolesUsuario.Update(roleUpdate);
                    }
                    await _context.SaveChangesAsync();
                }

                var materiasAtuaisId = materiasUsuario.Select(x => x.Materia_ID).ToList();
                var materiasUpId = usuario.MateriasUsuario.Select(x => x.Materia_ID).ToList();
                var materiasDelete = materiasAtuaisId.Except(materiasUpId).ToList();

                foreach (string toDelete in materiasDelete)
                {
                    var materiaToDelete = await _context.MateriasUsuario.FirstOrDefaultAsync(r => r.user_ID == usuario.Email && r.Materia_ID == toDelete);
                    _context.MateriasUsuario.Remove(materiaToDelete);
                    await _context.SaveChangesAsync();
                }

                foreach (MateriasModel materia in usuario.MateriasUsuario)
                {

                    var materiaUpdate = await _context.MateriasUsuario.FirstOrDefaultAsync(r => r.user_ID == usuario.Email && r.Materia_ID == materia.Materia_ID);
                    if (materiaUpdate == null)
                    {
                        _context.MateriasUsuario.Add(new MateriasUsuario()
                        {
                            Materia_ID = materia.Materia_ID,
                            user_ID = usuario.Email
                        });
                    }
                    else
                    {
                        materiaUpdate.Materia_ID = materia.Materia_ID;
                        materiaUpdate.user_ID = usuario.Email;
                        _context.MateriasUsuario.Update(materiaUpdate);
                    }
                    await _context.SaveChangesAsync();
                }

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
            var roleDelete = await _context.RolesUsuario
                .Join(_context.Roles, r => r.Role_ID, u => u.Role_ID, (r, u) => new { r, u })
                .Where(ru => ru.r.user_ID == usuario.Email)
                .Select(ru => ru.r.relacao_ID).ToListAsync();

            foreach (int toDelete in roleDelete)
            {
                var roleToDelete = await _context.RolesUsuario.FirstOrDefaultAsync(r => r.relacao_ID == toDelete);
                _context.RolesUsuario.Remove(roleToDelete);
                await _context.SaveChangesAsync();
            }

            var materiasDelete = await _context.MateriasUsuario
                .Join(_context.Materias, r => r.Materia_ID, u => u.Materia_ID, (r, u) => new { r, u })
                .Where(ru => ru.r.user_ID == usuario.Email)
                .Select(ru => ru.r.relacao_ID).ToListAsync();

            foreach (int toDelete in materiasDelete)
            {
                var materiaToDelete = await _context.MateriasUsuario.FirstOrDefaultAsync(r => r.relacao_ID == toDelete);
                _context.MateriasUsuario.Remove(materiaToDelete);
                await _context.SaveChangesAsync();
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
