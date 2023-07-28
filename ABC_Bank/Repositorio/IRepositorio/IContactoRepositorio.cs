using ABC_Bank.Model;

namespace ABC_Bank.Repositorio.IRepositorio
{
    public interface IContactoRepositorio : IRepositorio<Contacto>
    {
        Task<Contacto> Actualizar(Contacto entidad);
    }
}
