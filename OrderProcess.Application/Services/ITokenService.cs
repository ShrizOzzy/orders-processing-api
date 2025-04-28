using OrderProcess.Core.Models.LoginDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcess.Application.Services
{
    public interface ITokenService
    {
        LoginResponseDto GenerateToken(string username);
    }
}
