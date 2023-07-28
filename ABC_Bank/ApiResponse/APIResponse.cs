using System.Net;

namespace ABC_Bank.Response
{
    public class APIResponse
    {
        public HttpStatusCode statusCode { get; set; }

        public bool IsExitoso { get; set; } = true;

        public List<String> ErrorMessages { get; set; }

        public object Resultado { get; set; }
    }
}
