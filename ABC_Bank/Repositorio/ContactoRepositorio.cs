using ABC_Bank.Model;
using ABC_Bank.Repositorio.IRepositorio;

using Microsoft.EntityFrameworkCore;

namespace ABC_Bank.Repositorio
{
    public class ContactoRepositorio : Repositorio<Contacto>, IContactoRepositorio
    {
        private readonly ApplicationDbContext _db;

        public ContactoRepositorio(ApplicationDbContext db) : base(db)
        {
            _db= db;
        }

        public async Task<Contacto> Actualizar(Contacto entidad)
        {
            _db.Contactos.Update(entidad);
            await _db.SaveChangesAsync();
            return entidad;
        }
    }
}
