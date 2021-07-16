using System.Collections.Generic;
using System.Linq;

namespace SerilogMongoDbConsole.Infrastructure
{
    public static class ParamerterExtentions<T>
    {
        public static T[] GetArray(IEnumerable<object> entities)
        {
            return entities.Cast<T>().ToArray();
        }
    }
}
