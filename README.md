# ID Finder
_Find the creature you want out of the 4 billion possible ids._

## How to use
The UI is in the mod's Remix interface. Select a query from the drop down and click the ADD button to add it to the search. Use the checkboxes to enable looking for a value on a specific trait. Use the "bias" multiplier to favor certain traits over others (should not need to do this most of the time).

When you are ready to search, enter your id range and click the SEARCH button. Each individual search query is returned as a separate result in the output. To search over all ids, enter the range as 0 to -1.

> [!IMPORTANT]
> It is very unlikely that an id with your exact search query actually exists. This mod will find the closest one to it in your search range; it is up to you to tweak the parameters and biases.

## Bugs & feature requests
To report bugs or request features, please use [issues](https://github.com/alduris/id-finder/issues).

When reporting bugs, please describe how to recreate the bug and include any logs if possible. The mod outputs logs to `BepInEx/LogOutput.log` from the game's root folder, though as many logs as you can help. Note that other mods can change the visuals of creatures, and that ID Finder does not recognize these changes. This is not a bug, and issues relating to such will be dismissed.

## Contributing
I welcome changes through pull requests! Code dependencies are included with the repository, so all that should be needed is to open the `.csproj` located in the `/src` folder with an IDE such as Visual Studio and to symlink the `/mod` folder into the game's `mods` folder in `StreamingAssets`.
