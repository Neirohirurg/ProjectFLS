using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFLS.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Patronymic { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
