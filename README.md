# ScreenCutter

Is is a lightweigth aplication for taking screenshots of screen part. With mouse and keyboard guide to wanted position and take a screenshot of needed dimensions.

# Usage

Left mouse button click takes a screenshot and allows to save to file.

Right mouse button down opens a menu where its is posible to choose between horizontal, vertical or both size change option. Also it is possible to quit program by releasing button on "X" area. Options are activated when right mouse button is released on appropriate area. In middle of menu nubers show dimension of shot area in pixels. Upper numer width, lower height.

It is possible to guide using keyboard:
* left, right, up or down arrows moves area in that direction;
* "+" increases area dimensions;
* "-" decreases area dimensions;
* Esc - closes application;

# Plugins

Its is possible to create and use your own plugins for storing images.

Current plugins:
* ScreenCutter.SaveFileDialogPlugin.SaveFileDialogPlugin - using file save dialog;
* ScreenCutter.FastSavePlugin.FastSavePlugin - storing images to folder "images" in app folder;
* ScreenCutter.FastSavePlugin.SaveAsGrayscalePlugin - storing greiscaled images to folder "images" in app folder;

To use one of these plugins u need to place dll in app folder and change this option in app.config file (in this example "ScreenCutter.FastSavePlugin.SaveAsGrayscalePlugin" is a plugin that will be used):

```
<userSettings>
	<ScreenCutter.App.Properties.Settings>
	    <setting name="SaveScreenAreaPluginFullName" serializeAs="String">
	        <value>ScreenCutter.FastSavePlugin.SaveAsGrayscalePlugin</value>
	    </setting>
	</ScreenCutter.App.Properties.Settings>
</userSettings>
```

To create your own plugin implement interface ISaveScreenAreaPlugin that is provided in "ScreenCutter.PluginContract" project and use steps from above to let app know of your plugin.