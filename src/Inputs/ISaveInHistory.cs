﻿using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Denotes that an element can be saved in history and provides the necessary utilities for allowing so.
    /// </summary>
    public interface ISaveInHistory
    {
        /// <summary>
        /// Save key to use. Should theoretically be unique.
        /// </summary>
        public string SaveKey { get; }

        /// <summary>
        /// Converts the input into a format convertable to JSON via Newtonsoft.
        /// </summary>
        /// <returns>A struct containing the data for easy conversion</returns>
        public JObject ToSaveData();
        
        /// <summary>
        /// Returns the element to the state it was given the save state data.
        /// </summary>
        /// <param name="data">The save data to restore data to</param>
        public void FromSaveData(JObject data);

        /// <summary>
        /// Returns the string representation for the input on the history tab.
        /// </summary>
        /// <returns>The strings to represent the input on the history tab. Each returned string is separated by a new line. Recommended to yield return.</returns>
        public IEnumerable<string> GetHistoryLines();
    }
}
