using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using FinderMod.Search;
using FinderMod.Search.Options;

namespace FinderMod
{
    /// <summary>
    /// Public API for ID Finder. Contains methods to register your own search options.
    /// </summary>
    public static class API
    {
        /// <summary>
        /// Registers a new option using a factory method to initialize an <c><see cref="Option"/></c> class.
        /// </summary>
        /// <param name="name">Name of the search option as it will appear in the dropdown and on the menu.</param>
        /// <param name="factory">A factory method that initializes your search option.</param>
        public static void Register(string name, Func<Option> factory)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));
            OptionRegistry.RegisterOption(name, factory);
        }

        /// <summary>
        /// Registers a new option without having to create a custom Option class.
        /// 
        /// Useful if you don't want to require ID Finder as a hard dependency, but comes with the downside of less flexibility.
        /// 
        /// Replaces the <see cref="Option"/> element that would otherwise be required with a function that has:
        /// <list type="bullet">
        /// <item><description>Input <c><see cref="XORShift128"/></c> - The initial random state that would be in the code after calling Random.InitState()</description></item>
        /// <item><description>Input <c>List&lt;<see cref="IElement"/>&gt;</c> - The elements that appear on the menu, with end user-set values in the inputs.</description></item>
        /// <item><description>Returns a <c>float</c> representing the distance from what the code for the id generates is compared to the desired <see cref="Input{T}"/> values.</description></item>
        /// </list>
        /// </summary>
        /// <param name="name">Name of the search option as it will appear in the dropdown and on the menu.</param>
        /// <param name="elementsFactory">Factory to create new menu elements.</param>
        /// <param name="executeFunc">Method that will return the distance for a particular id. Parameters are: 
        /// </param>
        [Obsolete("Please use OptionBuilder instead")]
        public static void Register(string name, Func<List<IElement>> elementsFactory, APIBasicOption.APIExecute executeFunc)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));
            if (executeFunc is null) throw new ArgumentNullException(nameof(executeFunc));
            OptionRegistry.RegisterOption(name, () => new APIBasicOption(elementsFactory.Invoke(), executeFunc));
        }
    }
}
