using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using FinderMod.Search;
using FinderMod.Search.Options;

namespace FinderMod
{
    /// <summary>
    /// Allows <see cref="Option"/>s to be constructed without inheritance, at the cost of a very slight amount of speed.
    /// </summary>
    /// <param name="name"></param>
    public class OptionBuilder(string name)
    {
        private readonly string name = name ?? throw new ArgumentNullException("name");
        private OptionElements elementsFunc = () => [];
        private OptionExecute executeFunc = null!;
        private OptionValues valuesFunc = null!;

        /// <summary>
        /// Sets a delegate to create the user interface. Run once at option creation when selected, and then cached for later reuse.
        /// </summary>
        /// <param name="elementsFunc">The delegate to call.</param>
        /// <returns>The <see cref="OptionBuilder"/> instance for chaining.</returns>
        public OptionBuilder Elements(OptionElements elementsFunc)
        {
            this.elementsFunc = elementsFunc ?? throw new ArgumentNullException(nameof(elementsFunc));
            return this;
        }

        /// <summary>
        /// Sets a delegate to return the distance for a given id: how close the id (initialized in <see cref="XORShift128"/>)
        /// is to matching the user inputs (preset in the passed in <c>List<IElement></IElement></c>, which are returned from <see cref="Elements(OptionElements)"/>).
        /// </summary>
        /// <remarks>It is recommended to use the various <c>DistanceIf</c> static methods located in the <see cref="Option"/> class when calculating distance.</remarks>
        /// <param name="executeFunc">The delegate to call.</param>
        /// <returns>The <see cref="OptionBuilder"/> instance for chaining.</returns>
        public OptionBuilder Execute(OptionExecute executeFunc)
        {
            this.executeFunc = executeFunc ?? throw new ArgumentNullException(nameof(executeFunc));
            return this;
        }

        /// <summary>
        /// Sets a delegate to create text display on the Values tab. Optional. Each entry is a new line. <c>null</c> is whitespace.
        /// Values returned should represent the same values offered by the search option for the particular seed the <see cref="XORShift128"/> argument is initialized to.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="valuesFunc">The delegate to call.</param>
        /// <returns>The <see cref="OptionBuilder"/> instance for chaining.</returns>
        public OptionBuilder Values(OptionValues valuesFunc)
        {
            this.valuesFunc = valuesFunc ?? throw new ArgumentNullException(nameof(valuesFunc));
            return this;
        }

        /// <summary>
        /// Registers the created option. <see cref="Execute(OptionExecute)"/> is required to have been called.
        /// </summary>
        public void Register()
        {
            API.Register(name, () => new APIBuilderOption(elementsFunc, executeFunc, valuesFunc));
        }

        /// <summary>Delegate definition for creating the user interface for an option.</summary>
        /// <returns>List of elements to put on user interface.</returns>
        public delegate IEnumerable<IElement> OptionElements();
        /// <summary>
        /// Delegate definition for finding the distance for a particular id, initialized by the <see cref="XORShift128"/> argument.
        /// Distance should be calculated such that the closer an id's inputs match, the closer the distance is to 0.
        /// Additionally, each input should ideally contribute the input's bias at most to the distance.
        /// The various <c>DistanceIf</c> static methods in the <see cref="Option"/> should take care of most use cases.
        /// </summary>
        /// <param name="Random">Random number generator, pre-initialized with an id.</param>
        /// <param name="elements">Elements created by <see cref="OptionElements"/>, with user-set values.</param>
        /// <returns>Distance to the id.</returns>
        public delegate float OptionExecute(XORShift128 Random, List<IElement> elements);
        /// <summary>
        /// Delegate definition for returning the values of a particular id, displayed on the Values tab.
        /// </summary>
        /// <param name="Random"></param>
        /// <returns></returns>
        public delegate IEnumerable<string> OptionValues(XORShift128 Random);
    }
}
