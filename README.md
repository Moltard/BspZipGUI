# BspZipGUI
A clean GUI tool that use Valve's BSPZIP command line utility. It is used to embed a folder with custom files (materials, models, sound,..) into a BSP file.

<img src="Images/ExampleCustomFolder.jpg" width="60%" />

You can also repack a map to compress (or decompress it).

It can be used for any Source Engine game that provide a bspzip.exe.

This tool idea is based on [Geotavros's BspZipGui](https://github.com/geotavros/BspZipGui) .

## Download

[BspZipGUI v1.0](https://github.com/Moltard/BspZipGUI/releases/latest)

## How to use - Bsp Packer

Select a game you defined (3 are setup per default), a custom folder you defined and load a BSP.

Check 'Use Directory Whitelist' (recommended) to only pack files from whitelisted directories (see Settings).

<img src="Images/MainMenu.jpg" width="60%" />

Press Pack Bsp.

<img src="Images/LogsEnd.jpg" width="60%" />

## How to use - Bsp Repack

Select a game you defined (3 are setup per default) and load a BSP.

<img src="Images/RepackMenu.jpg" width="60%" />

Press Compress Bsp or Decompress Bsp.

<img src="Images/RepackLogs.jpg" width="60%" />

## How to setup

Add new games with the 'Add...' button. Delete them with the 'Delete' button.

Setup your games by loading the gameinfo.txt and bspzip.exe.

<img src="Images/SettingsMenu.jpg" width="60%" />

Add new custom folders with the 'Add...' button. Delete them with the 'Delete' button.

Load the folder with the custom files that you want to embed in your map.

<img src="Images/SettingsCustomFolder.jpg" width="60%" />

Define a whitelist of subfolders and type of files (through their extensions) that can be packed. 

<img src="Images/SettingsWhitelist.jpg" width="60%" />

(All the subfolders you would need are already setup, but you can edit to your needs)


### Developers

- [Moltard](https://github.com/Moltard)



