using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Common.Requests.Identity;

public record UpdateRoleRequest(string RoleId,string Name, string Description);
