using System;
using FinderMod.Search.Options;

namespace FinderMod
{
    public static class API
    {
        public static void Register(string name, Func<Option> factory)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));

#warning implemernt
            throw new NotImplementedException();
        }
    }
}
