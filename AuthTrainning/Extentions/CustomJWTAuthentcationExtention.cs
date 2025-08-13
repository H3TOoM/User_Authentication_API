namespace AuthTrainning.Extentions
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;

    
    public static class CustomJWTAuthentcationExtention
    {
        
        public static void AddCustomJWTAuth( this IServiceCollection services, ConfigurationManager configurationManager )
        {
            services.AddAuthentication( options =>
            {
                // Add Auth schemes
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            } ).AddJwtBearer( options =>
            {

                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true; // Save the token in the authentication properties
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configurationManager["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey( System.Text.Encoding.UTF8.GetBytes( configurationManager["Jwt:Key"] ) ) // Set your secret key
                };

            } );
        }

        
        public static void AddSwagger( this IServiceCollection services )
        {
            services.AddSwaggerGen( c =>
            {
                c.SwaggerDoc( "v1", new OpenApiInfo
                {
                    Title = "Authencation API",
                    Version = "v1",
                    Description = "API for Authentication Training"
                } );

                c.AddSecurityDefinition( "Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                } );

                // Security Requirement
                c.AddSecurityRequirement( new OpenApiSecurityRequirement
                {
                      {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                 Type = ReferenceType.SecurityScheme,
                                 Id = "Bearer"
                              },

                                Name = "Bearer",
                                In = ParameterLocation.Header
                         },

                            new List<string>()
                      }
                } );

            } );
        }
    }
}
