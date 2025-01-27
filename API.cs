using System;
using FinderMod.Search.Options;

namespace FinderMod
{
    public static class API
    {
        public static void Register(string name, Type option)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));
            if (option is null) throw new ArgumentNullException(nameof(option));
            if (!option.IsSubclassOf(typeof(Option))) throw new ArgumentException($"Type must derive {typeof(Option).FullName}!");

#warning implemernt
            throw new NotImplementedException();
        }
    }
}
