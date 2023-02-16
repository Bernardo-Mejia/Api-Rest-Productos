using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using API_Productos_Categorias.Models;
using Microsoft.AspNetCore.Cors;

namespace API_Productos_Categorias.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly NetcoreapiProductosCategoriasContext _DBContext;

        public ProductoController(NetcoreapiProductosCategoriasContext dBContext)
        {
            _DBContext = dBContext;
        }

        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            List<Producto> productos = new();
            try
            {
                // Ignorar referencias cíclicas
                productos = _DBContext.Productos.Include(c => c.oCategoria).ToList();

                return StatusCode(StatusCodes.Status200OK, new
                {
                    info = new
                    {
                        status = true,
                        statusText = "Datos consultados correctamente",
                    },
                    data = productos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    status = false,
                    statusText = "Error al consultar los datos",
                    data = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("Obtener/{id:int}")]
        public IActionResult Obtener(int id)
        {
            Producto producto = new();
            //producto = (Producto)_DBContext.Productos.Where(p => p.IdProducto == id).Include(c => c.oCategoria).FirstOrDefault();
            producto = _DBContext.Productos.Find(id);
            if (producto == null)
            {
                return BadRequest(new
                {
                    status = false,
                    statusText = "Producto no encontrado"
                });
            }
            try
            {
                producto = _DBContext.Productos.Include(c => c.oCategoria).Where(p => p.IdProducto == id).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK, new
                {
                    info = new
                    {
                        status = true,
                        statusText = "Datos consultados correctamente",
                    },
                    data = producto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    status = false,
                    statusText = "Error al consultar los datos",
                    data = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("ObtenNombre/{nombre}")]
        public IActionResult ObtenNombre(string nombre)
        {
            IEnumerable<Producto> productos = _DBContext.Productos
                   .Where(p => p.CodigoBarra.Contains(nombre) || p.Descripcion.Contains(nombre) || p.Marca.Contains(nombre) || p.oCategoria.Descripcion.Contains(nombre))
                   .Include(c => c.oCategoria).ToList();

            if (productos.Count() == 0)
            {
                return BadRequest(new
                {
                    status = false,
                    statusText = "No se encontraron productos"
                });
            }

            try
            {
                return StatusCode(StatusCodes.Status200OK, new
                {
                    info = new
                    {
                        status = true,
                        statusText = "Datos consultados correctamente",
                    },
                    data = productos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    status = false,
                    statusText = "Error al consultar los datos",
                    data = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("Guardar")]
        public IActionResult Guardar([FromBody] Producto model)
        {
            try
            {
                _DBContext.Productos.Add(model);
                _DBContext.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new
                {
                    info = new
                    {
                        status = true,
                        statusText = "Proucto guardado correctamente"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    info = new
                    {
                        status = false,
                        statusText = "Error al guardar producto",
                        data = ex.Message
                    }
                });
                throw;
            }
        }

        [HttpPut]
        [Route("Editar")]
        public IActionResult Editar([FromBody] Producto model)
        {
            Producto producto = _DBContext.Productos.FirstOrDefault(x => x.IdProducto.Equals(model.IdProducto));
            if (producto == null)
            {
                return BadRequest(new
                {
                    status = false,
                    statusText = "Producto no encontrado"
                });
            }
            try
            {
                producto.CodigoBarra = model.CodigoBarra is null ? producto.CodigoBarra : model.CodigoBarra;
                producto.Descripcion = model.Descripcion is null ? producto.Descripcion : model.Descripcion;
                producto.Marca = model.Marca is null ? producto.Marca : model.Marca;
                producto.IdCategoria = model.IdCategoria is null ? producto.IdCategoria : model.IdCategoria;
                producto.Precio = model.Precio is null ? producto.Precio : model.Precio;

                _DBContext.Productos.Update(producto);
                _DBContext.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, new
                {
                    info = new
                    {
                        status = true,
                        statusText = "Proucto editado correctamente"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    info = new
                    {
                        status = false,
                        statusText = "Error al guardar producto",
                        data = ex.Message
                    }
                });
            }
        }


        [HttpDelete]
        [Route("Eliminar/{id:int}")]
        public IActionResult Eliminar(int id)
        {
            Producto producto = _DBContext.Productos.Find(id);
            if (producto == null)
            {
                return BadRequest(new
                {
                    status = false,
                    statusText = "Producto no encontrado"
                });
            }
            try
            {
                _DBContext.Productos.Remove(producto);
                _DBContext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new
                {
                    info = new
                    {
                        status = true,
                        statusText = "Proucto eliminado correctamente"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    info = new
                    {
                        status = false,
                        statusText = "Error al guardar producto",
                        data = ex.Message
                    }
                });
            }
        }
    }
}
