using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class Response<T>
    {
        public Response()
        {

        }

        public Response(T data)
        {
            Data = data;
            Succeeded = true;
            Message = string.Empty;
            Errors = null;
        }

        public bool Succeeded { get; set; }

        public string Message { get; set; }

        public string[] Errors { get; set; }

        public T Data { get; set; }
    }
}
