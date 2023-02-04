using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Data
{
    public static class RepositoryErrors
    {
        public const string ObjectNotFound = "Object of type '{0}' not found";

        public const string ObjectNotFoundWithSnowflake = "Object of type '{0}' with snowflake '{1}' not found.";
    }
}
