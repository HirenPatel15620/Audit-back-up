using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class DeleteUserViewModel
    {
        public long Id { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Contact { get; set; }
    }
}
