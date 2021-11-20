using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonCode.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAvalonCodeServices(this ServiceCollection services)
        { 
            services.AddSingleton<ISolutionExplorer, Implementation.SolutionExplorer>();
        }
    }
}
