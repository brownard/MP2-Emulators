## MP2-Emulators

A plugin for the open source media centre [MediaPortal 2](https://www.team-mediaportal.com/) to display metadata and launch games.

### Features
* Retrieve and display metadata for your games from various online databases
* [Libretro](https://www.libretro.com/) support to allow you to run libretro cores inside MediaPortal 2, just like playing a video
* Launch PC games and external emulators

### History
This repo is a continuation of the development started [here](https://github.com/brownard/MediaPortal-2/), the first commit here is at the same state as [this](https://github.com/brownard/MediaPortal-2/tree/1b18873727cb043ca81940709ae2720d224cc641) commit in the original repo.

### Compilation

In order to compile the plugin you first need to pull the MediaPortal 2 submodule and build the MediaPortal 2 solutions for both Debug and Release, the Emulators solution should then pick up the appropriate references and compile. 
