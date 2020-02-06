using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExternalClient.IDP
{
    public static class SiginingCertificateExtension
    {
        public static void AddCustomSiginingCertificate(
            this IIdentityServerBuilder builder,
            IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                builder.AddDeveloperSigningCredential();
            else
                builder.AddDeveloperSigningCredential();
        }
    }
}
