using AuthTrainning.Data;
using AuthTrainning.DTOs;
using AuthTrainning.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthTrainning.Controllers
{
    [Route( "api/[controller]" )]
    [ApiController]
    public class AccountController : ControllerBase
    {

        // Injecting UserManager to manage user operations

        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        public AccountController( UserManager<User> userManager, IConfiguration configuration )
        {
            _userManager = userManager;
            _configuration = configuration;
        }






        [HttpPost( "Register" )]
        public async Task<IActionResult> register( [FromBody] RegisterDto dto )
        {
            var user = new User
            {
                UserName = dto.Name,
                Email = dto.Email,

            };

            IdentityResult result = await _userManager.CreateAsync( user, dto.Password );
            if (!result.Succeeded)
            {
                return BadRequest( result.Errors );
            }


            var token = await GenerateToken( user );

            return Ok(
                new
                {
                    token,
                    user = new
                    {
                        user.Id,
                        user.UserName,
                        user.Email
                    }
                }
            );

        }


        [HttpPost( "Login" )]
        public async Task<IActionResult> login( LoginDto dto )
        {

            var user = await _userManager.FindByNameAsync( dto.UserName );
            if (user == null)
            {
                return Unauthorized( "Invalid username or password" );
            }

            var result = await _userManager.CheckPasswordAsync( user, dto.Password );
            if (!result)
            {
                return Unauthorized( "Invalid username or password" );
            }




            var token = await GenerateToken( user );



            return Ok( new
            {
                token,

                user = new
                {
                    user.Id,
                    user.UserName,
                    user.Email
                }
            } );

        }



        // Genrate a new token
        private async Task<string> GenerateToken( User user )
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var roles = await _userManager.GetRolesAsync( user );
            foreach (var role in roles)
            {
                claims.Add( new Claim( ClaimTypes.Role, role.ToString() ) );
            }


            // signingCredentials
            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( _configuration["Jwt:Key"] ) );
            var sc = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );
            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddDays( 1 ),
                signingCredentials: sc
             );


            return new JwtSecurityTokenHandler().WriteToken( token );

        }
    }
}