using HRSystem.Application.Services.Identity;
using HRSystem.Common.Responses.Wrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Application.Features.Identity.Roles.Queries
{
    public class GetRolesQuery : IRequest<IResponseWrapper>
    {
    }

    public class GetRolesQueryHandler (IRoleService _roleService)
        : IRequestHandler<GetRolesQuery, IResponseWrapper>
    {
        public async Task<IResponseWrapper> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            return await _roleService.GetAllRolesAsync();
        }
    }
}
