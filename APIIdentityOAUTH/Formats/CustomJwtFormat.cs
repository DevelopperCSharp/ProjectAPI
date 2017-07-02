using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Thinktecture.IdentityModel.Tokens;
//using System.IdentityModel.Tokens;

namespace APIIdentityOAUTH.Formats
{
    //n'y a pas de support direct pour l'émission de JWT dans ASP.NET Web API, afin de commencer à publier JWT, 
    //nous devons l'implémenter manuellement en mettant en œuvre l'interface "ISecureDataFormat" et implémentons la méthode "Protéger"
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {

        private readonly string _issuer = string.Empty;

        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;  // example: http://localhost:59142/   
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            //string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];

            //string symmetricKeyAsBase64 = ConfigurationManager.AppSettings["as:AudienceSecret"];

            //var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);
            //var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyByteArray);
            //var signatureCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //var issued = data.Properties.IssuedUtc;

            //var expires = data.Properties.ExpiresUtc;

            //var token = new JwtSecurityToken(_issuer,
            //                                   audienceId,
            //                                   data.Identity.Claims,
            //                                   issued.Value.UtcDateTime,
            //                                   expires.Value.UtcDateTime,
            //                                   signatureCredentials);

            //var handler = new JwtSecurityTokenHandler();

            //var jwt = handler.WriteToken(token);

            //return jwt;


            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];

            string symmetricKeyAsBase64 = ConfigurationManager.AppSettings["as:AudienceSecret"];

            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyByteArray);
            var signingKey = new HmacSigningCredentials(keyByteArray);

            var issued = data.Properties.IssuedUtc;

            var expires = data.Properties.ExpiresUtc;
            var token = new JwtSecurityToken(_issuer,
                                               audienceId,
                                               data.Identity.Claims,
                                               issued.Value.UtcDateTime,
                                               expires.Value.UtcDateTime,
                                                 signingKey);
            // var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.WriteToken(token);

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
  
}
}