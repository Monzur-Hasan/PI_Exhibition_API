using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Administrator.DTO
{
    public class DeleteRoleRequestDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
}
