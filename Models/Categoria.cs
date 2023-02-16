using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_Productos_Categorias.Models;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string? Descripcion { get; set; }

    [JsonIgnore] // No mostrará el objeto productos nulo cuando se resuelvan las referencias cíclicas
    public virtual ICollection<Producto> Productos { get; } = new List<Producto>();
}
