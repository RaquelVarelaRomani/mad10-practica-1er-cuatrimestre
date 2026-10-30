using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Es.Udc.DotNet.PracticaMAD.Model.Cache
{
    /// <summary>
    /// Generador de claves canonicas para las consultas interceptadas por <see cref="SearchCacheInterceptor"/>.
    /// </summary>
    public static class SearchCacheKeyGenerator
    {
        /// <summary>
        /// Construye la clave normalizada para una busqueda paginada de productos.
        /// </summary>
        /// <param name="keyword">Palabras clave de busqueda.</param>
        /// <param name="categoryId">Identificador opcional de categoria.</param>
        /// <param name="startIndex">Indice de inicio de paginacion.</param>
        /// <param name="count">Numero de elementos solicitados.</param>
        /// <returns>Clave canonica de la consulta.</returns>
        public static string BuildProductSearchKey(string keyword, long? categoryId, int startIndex, int count)
        {
            string normalizedKeyword = NormalizeText(keyword);
            string normalizedCategory = (!categoryId.HasValue || categoryId.Value <= 0)
                ? "all"
                : categoryId.Value.ToString(CultureInfo.InvariantCulture);

            return string.Format(
                CultureInfo.InvariantCulture,
                "ProductSearch|cat={0}|start={1}|count={2}|kw={3}:{4}",
                normalizedCategory,
                startIndex,
                count,
                normalizedKeyword.Length,
                normalizedKeyword);
        }

        /// <summary>
        /// Construye una clave canonica a partir del metodo interceptado y sus argumentos.
        /// Unifica las sobrecargas de busqueda de productos para que compartan la misma entrada de cache.
        /// </summary>
        /// <param name="method">Metodo interceptado.</param>
        /// <param name="arguments">Argumentos de la invocacion.</param>
        /// <returns>Clave canonica de la consulta.</returns>
        public static string BuildKey(MethodInfo method, object[] arguments)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            object[] args = arguments ?? new object[0];

            if ((method.Name == "FindProductByName") && (args.Length == 3)
                && (args[1] is int) && (args[2] is int))
            {
                return BuildProductSearchKey(
                    args[0] as string,
                    null,
                    (int)args[1],
                    (int)args[2]);
            }

            if ((method.Name == "FindProducts") && (args.Length == 4)
                && (args[2] is int) && (args[3] is int))
            {
                long? categoryId = args[1] != null ? (long?)Convert.ToInt64(args[1], CultureInfo.InvariantCulture) : null;

                return BuildProductSearchKey(
                    args[0] as string,
                    categoryId,
                    (int)args[2],
                    (int)args[3]);
            }

            StringBuilder builder = new StringBuilder();
            if (method.DeclaringType != null)
            {
                builder.Append(method.DeclaringType.FullName).Append('.');
            }

            builder.Append(method.Name);

            for (int i = 0; i < args.Length; i++)
            {
                builder.Append('|');
                object arg = args[i];
                if (arg == null)
                {
                    builder.Append("<null>");
                }
                else
                {
                    string stringArg = arg as string;
                    if (stringArg != null)
                    {
                        string normalized = NormalizeText(stringArg);
                        builder.Append(normalized.Length.ToString(CultureInfo.InvariantCulture))
                               .Append(':')
                               .Append(normalized);
                    }
                    else
                    {
                        builder.Append(Convert.ToString(arg, CultureInfo.InvariantCulture));
                    }
                }
            }

            return builder.ToString();
        }

        private static string NormalizeText(string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : text.Trim().ToLowerInvariant();
        }
    }
}

