using ABC_Bank.Dtos;
using ABC_Bank.Model;
using ABC_Bank.Repositorio.IRepositorio;
using ABC_Bank.Response;

using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Net;

namespace ABC_Bank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactoController : ControllerBase
    {
        private readonly ILogger<ContactoController> _logger;
        private readonly IContactoRepositorio _contactoRepositorio;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public ContactoController(ILogger<ContactoController> logger, IContactoRepositorio contactoRepositorio,
            IMapper mapper)
        {            
            _logger= logger;
            _contactoRepositorio= contactoRepositorio;
            _mapper= mapper;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetContactos()
        {
            try
            {
                _logger.LogInformation("Obtener todos los contactos");
                IEnumerable<Contacto> contactoList = await _contactoRepositorio.ObtenerTodos();
                _response.Resultado = _mapper.Map<IEnumerable<ContactoDto>>(contactoList);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);

            }
            catch (Exception ex)
            {

                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpGet("id:int", Name = "GetContacto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetContacto(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer Contacto con Id=" + id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
                }
                //var villa = VillaStore.villaLista.FirstOrDefault(x => x.Id == id);
                var contacto = await _contactoRepositorio.Obtener(x => x.Id == id);
                if (contacto == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response);
                }

                _response.Resultado = _mapper.Map<ContactoDto>(contacto);
                _response.statusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {

                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CrearContacto([FromBody] ContactoDto createContacto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (await _contactoRepositorio.Obtener(x => x.Nombres.ToLower() == createContacto.Nombres.ToLower()) != null)
                {
                    ModelState.AddModelError("NombreExiste", "El contacto con ese nombre ya existe");
                    return BadRequest(ModelState);

                }

                if (createContacto == null)
                {
                    return BadRequest();
                }

                Contacto modelo = _mapper.Map<Contacto>(createContacto);
                await _contactoRepositorio.Crear(modelo);
                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetContacto", new { id = modelo.Id }, _response);

            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteContacto(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var contacto = await _contactoRepositorio.Obtener(x => x.Id == id);
                if (contacto == null)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                await _contactoRepositorio.Remover(contacto);

                _response.statusCode = HttpStatusCode.NoContent;
                return Ok(_response);

            }
            catch (Exception ex)
            {

                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return BadRequest(_response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarContacto(int id, [FromBody] ContactoDto updateContacto)
        {
            if (updateContacto == null || id != updateContacto.Id)
            {
                _response.IsExitoso = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            Contacto modelo = _mapper.Map<Contacto>(updateContacto);

            await _contactoRepositorio.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> ActualizarContacto(int id, JsonPatchDocument<ContactoDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {

                return BadRequest();
            }

            var contacto = await _contactoRepositorio.Obtener(x => x.Id == id, tracked: false);

            ContactoDto contactoDto = _mapper.Map<ContactoDto>(contacto);

            if (contacto == null) return BadRequest();

            patchDto.ApplyTo(contactoDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Contacto modelo = _mapper.Map<Contacto>(contactoDto);

            await _contactoRepositorio.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }

        [HttpGet("id:int nombres:string direccion:string", Name = "GetContactoPorNombreYDireccion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetContactoPorNombreYDireccion( string nombres, string direccion)
        {
            try
            {
                if (nombres == null && direccion == null )
                {
                    _logger.LogError("Error al traer Contacto con Nombre=" + nombres +" y la Direccion" + direccion);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
                }

                var contacto = await _contactoRepositorio.ObtenerPorNombreYDireccion(x => x.Nombres == nombres && x.Direccion == direccion);
                if (contacto == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response);
                }

                _response.Resultado = _mapper.Map<ContactoDto>(contacto);
                _response.statusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {

                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;

        }

        [HttpGet("id:int fech_Nac: DateTime", Name = "GetRangoDeEdad")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetRangoDeEdad(int id,DateTime fecha_Nac)
        {
            try
            {
                if ( fecha_Nac == null)
                {
                    _logger.LogError("Error al traer Contacto con Fecha de Nacimiento=" + fecha_Nac );
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
                }

                var contacto = await _contactoRepositorio.ObtenerFechaNac(x => x.Id== id && x.Fecha_Nac == fecha_Nac);
                var fecha = contacto.Fecha_Nac.Year;
                var fecha_actual = DateTime.Now.Year;
                var rangodeEdad = fecha_actual - fecha;

                if (contacto == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response);
                }

                _response.Resultado = rangodeEdad;
                _response.statusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {

                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;

        }
    }
}
