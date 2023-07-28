namespace ABC_Bank.Model
{
    public class Contacto
    {
        public int Id { get; set; }

        public string Nombres { get; set; }

        public string Direccion { get; set; }

        public DateTime Fecha_Nac { get; set; }

        public int Telefono { get; set; }

        public string? UrlImagen { get; set; }
    }
}
