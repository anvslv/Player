Minimalistic mp3 player. Inspiration was the 
[awesome player][1] by Ilya Birman. 
This one has the same features, but periodic crashes because of foobar outdated core.
Plus, `foo_ui_gfx` is no longer updated. The core is a fork 
[Espera][2] by flagbug.

[![Player](https://anvs.lv/images/player-thumb.png "Player")](https://anvs.lv/images/player.png) [![Drag-n-drop](https://anvs.lv/images/player-drophere-thumb-en.png "Drag-n-drop")](https://anvs.lv/images/player-drophere-en.png) 

[![Tray](https://anvs.lv/images/player-tray-thumb-en.png "Tray")](https://anvs.lv/images/player-tray-en.png) [![Stripe](https://anvs.lv/images/player-stripe-thumb.png "Stripe")](https://anvs.lv/images/player-stripe.png)
  
Files are added by dragging to the stripe, folders and archives (zip, rar) are also supported.
Both playlist and stripe window are sticky thanks to [StickyWindow][3] library.
Dragging the stripe moves it, resizing is performed by dragging the right edge.
Moving playlist is done by dragging its upper edge.

Features:

* Play / Pause — stripe double-click
* Show / Hide playlist — pressing the right mouse button and move down
* Previous track — pressing the right mouse button and move left
* Next track — pressing the right mouse button and move right
* Volume can be adjusted with the mouse wheel
* Hide / Show player — double-click the tray icon
* For exiting there is an item in the tray context menu  


[1]: http://ilyabirman.ru/meanwhile/tags/music-player/
[2]: https://github.com/flagbug/Espera/tree/master
[3]: http://programminghacks.net/2009/10/19/download-snapping-sticky-magnetic-windows-for-wpf/

# Build dependancies
* Net Framework 4.8
* Installer build dependencies
    * WiX Toolset build tools (https://github.com/wixtoolset/wix3/releases/tag/wix3112rtm)
    * WiX Toolset Visual Studio Extension (https://wixtoolset.org/releases/)


