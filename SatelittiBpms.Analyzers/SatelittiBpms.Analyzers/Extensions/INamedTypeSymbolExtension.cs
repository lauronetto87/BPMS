using Microsoft.CodeAnalysis;

namespace SatelittiBpms.Analyzers.Extensions
{
    public static class INamedTypeSymbolExtension
    {
        public static bool IsRepository(this ITypeSymbol namedTypeSymbol)
        {
            foreach (var item in namedTypeSymbol.AllInterfaces)
            {
                if (item.Name == "IRepositoryBase" && item.TypeArguments.Length == 1)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsService(this ITypeSymbol namedTypeSymbol)
        {
            foreach (var item in namedTypeSymbol.AllInterfaces)
            {
                if (item.Name == "IServiceBase" && item.TypeArguments.Length == 2)
                {
                    return true;
                }
            }
            return false;
        }

        public static ITypeSymbol GetRespositoryEntityFromService(this ITypeSymbol namedTypeSymbol)
        {
            foreach (var item in namedTypeSymbol.AllInterfaces)
            {
                if (item.Name == "IServiceBase" && item.TypeArguments.Length == 2)
                {
                    return item.TypeArguments[1];
                }
            }
            return null;
        }
        public static ITypeSymbol GetRespositoryEntityFromRepository(this ITypeSymbol namedTypeSymbol)
        {
            foreach (var item in namedTypeSymbol.AllInterfaces)
            {
                if (item.Name == "IRepositoryBase" && item.TypeArguments.Length == 1)
                {
                    return item.TypeArguments[0];
                }
            }
            return null;
        }
    }
}
