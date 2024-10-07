# Bon Voyage :: Archive

* 2016-1211: 0.11.1 (Maja) for KSP 1.2.2
	+ Fixes for KSP 1.2.2
	+ Added "Close" button to main window
	+ Toolbar button won't appear in editors
* 2016-1030: 0.11.0 (Maja) for KSP 1.2.1
	+ New part, created by [Enceos](http://forum.kerbalspaceprogram.com/index.php?/profile/110725-enceos/)
	+ Icon is now colorized, made by [Madebyoliver](http://www.flaticon.com/authors/madebyoliver) and licensed under [Creative Commons BY 3.0](http://creativecommons.org/licenses/by/3.0/)
	+ Moved BV part to Space Exploration science node, where RoveMax Model S2 is
	+ Parts that can contain BV module are not hardcoded anymore
	+ Duplicate parts on the same vessel are ignored
	+ Code cleanup
	+ KSP skin for UI available
	+ Dewarp done in two steps: instant to 50x and then gradual to 1x
	+ Solar powered rovers will idle when Sun is 0 degrees above the horizon, no more stucking at poles
	+ Serious average speed penalties at twighlight and at night time for manned rovers
	+ Some colors in arrival report
	+ Added toolbar support, fixed wrapper, no Contract Configurator conflict :D
	+ Switching to vessel with interface button will go without scene reload if vessel is already loaded
	+ 80% speed penalty for unmanned rovers
	+ UI and label are hidden if game is paused
	+ Label is hidden when all hidden (F2)
	+ Fixed crazy torpedoes nuking active vessel, rover simply won't move closer than 2400 meters to active vessel
	+ Fixed argument out of range caused sometimes and rover voyage end
	+ Route visualized with red line
	+ Route always visualized for active rover if exists
	+ Target can be set to active navigation point
	+ Added support for USI nuclear reactors
	+ Added support for NFE fission reactors
* 2016-1013: 0.10.2 (Maja) for KSP 1.2
	+ Recompile for KSP 1.2
	+ Fixed last waypoint being last step of voyage
* 2016-1003: 0.10.1 (Maja) for KSP 1.1.3
	+ Fixed control lock being applied to next switched vessel
	+ Moved "Bon Voyage control lock active" message higher on screen
	+ SAS is blocked by control lock too
	+ Path markers displayed at correct positions
	+ Fixed trying to build path to target closer than 1000 meters
	+ Fixed "yet to travel" distance for rovers awaiting sunlight being incorrect after scene switch
	+ Current rover changes in GUI list on vessel switch
	+ Fixed distance to target being incorrect if path is not staright
	+ Fixed errors raised at rover journey end when no/low time acceleration
	+ Fixed error switching to rover from Space Center
	+ Added ARES and Puma support
* 2016-1001: 0.10.0 (Maja) for KSP 1.1.3
	+ Fixed BV controller part being not in Control tab
	+ Shut down wheels are not treated as power consumers
	+ At least two wheels must be on to start BV
	+ Fixed utilites not being shown
	+ Added "Calculate average speed" and "Calculate power requirement" utilities
	+ Power production requirement diminished to 35% of powered wheels max
	+ Average speed now varies according to number of wheels on: 2 wheels - 50% of wheels' max speed, 4 wheels - 60%, 6 and more - 70%
	+ Rovers driven away from KSC by BV are not treated as landed at runway or launchpad anymore
* 2016-0929: 0.9.9.10 (Maja) for KSP 1.1.3
	+ Fixed errors in editor
	+ Fixed rover altitude being incorrect
	+ Added Malemute and Karibou compatibility
	+ Code optimization
	+ Pathfinding fully functional
	+ Interface revamp
	+ Module Manager patch to add BonVoyage to Malemute, Karibou and Buffalo cabs
* 2016-0916: 0.9.9.9 (Maja) for KSP 1.1.3
	+ Initial public release
* 2019-1010: 0.5.4 (Maja) for KSP 1.7.3
	+ Changes
		- Path is computed immediately after selecting a target (point on map, current target or waypoint)
	+ Fixes
			- Fixed null ref when closing main window after hiding the application button
			- Fixed distance display in the control panel after setting a route
* 2019-0829: 0.5.3 (Maja) for KSP 1.7.3
	+ Changes
		- Cheats for infinite electricity and propellant are accepted (your game, your rules)
		- Part upgrades were reworked to support their placement into different tech trees by MM's config
			- By default, two upgrades are in the stock tech tree (plus the base part). Third upgrade is placed into Artificial intelligence node of CTT if you have it.
* 2019-0826: 0.5.2 (Maja) for KSP 1.7.3
	+ Changes
		- BonVoyage modul added to RoveMate by default
		- Added part upgrades which raise the speed of unmanned rovers and ships. The last upgrade is only for Community Tech Tree.
* 2019-0823: 0.5.1 (Maja) for KSP 1.7.3
	+ Changes
		- Suport of servo rotors from the Breaking Ground DLC for ship controller
* 2019-0816: 0.5.0_-_Come_all_you_young_sailormen_listen_to_me (Maja) for KSP 1.7.3
	+ Changes
		- We are ready to extend our operations to the water
			- Stock jet and rocket motors are supported
			- Support of electric motors from Feline Utility Rovers and USI Exploration Pack mods (and other mods, which has motors based on stock modules)
			- Electric motors are the best ones for extended naval operations
		- MiniAVC removed from the installation, because planned updates are done. If you need notification about new versions, use AVC instead
* 2017-0317: 0.12.0 (RealGecko) for KSP 1.2.2
	+ Fixes
		- Change a few frequently called `foreach` loops to `for` by [soulsource](https://github.com/Real-Gecko/KSP-BonVoyage/pull/3)
		- Make `Bon Voyage Autopilot` part physicsless by [Suprcheese](https://github.com/Real-Gecko/KSP-BonVoyage/pull/4)
		- `Close` button calls `appLauncherButton.SetFalse`
		- Check for retracted solar panels
		- Average speed is now really average and not the maximum of any wheel's speed
		- Target 200 meters away from navpoint
		- Landing gears can be used as operable wheels by [Kerbas-ad-astra](https://github.com/Real-Gecko/KSP-BonVoyage/pull/6)
		- ModuleWheelBase used to determine if wheel is on the ground by [Kerbas-ad-astra](https://github.com/Real-Gecko/KSP-BonVoyage/pull/6)
		- Allow travelling "below" sea level if celestial body has no ocean
	+ Changes
			- KSPWheels support
			- Separate UI for module control, no mess in right click menu
			- Integrated UIFramework
			- Path compressed with [lz-string-csharp](https://github.com/jawa-the-hutt/lz-string-csharp) to use less space in save file
			- Show route only for active rover
			- Interstellar reactors support
		- [screenshot6](https://cloud.githubusercontent.com/assets/2231969/24061553/0af45ab2-0b82-11e7-81fb-807086ddb330.png)
* 2016-1211: 0.11.1 (RealGecko) for KSP 1.2.2
	+ Fixes for KSP 1.2.2
	+ Added "Close" button to main window
	+ Toolbar button won't appear in editors
* 2016-1030: 0.11.0 (RealGecko) for KSP 1.2.1
	+ New part, created by [Enceos](http://forum.kerbalspaceprogram.com/index.php?/profile/110725-enceos/)
	+ Icon is now colorized, made by [Madebyoliver](http://www.flaticon.com/authors/madebyoliver) and licensed under [Creative Commons BY 3.0](http://creativecommons.org/licenses/by/3.0/)
	+ Moved BV part to Space Exploration science node, where RoveMax Model S2 is
	+ Parts that can contain BV module are not hardcoded anymore
	+ Duplicate parts on the same vessel are ignored
	+ Code cleanup
	+ KSP skin for UI available
	+ Dewarp done in two steps: instant to 50x and then gradual to 1x
	+ Solar powered rovers will idle when Sun is 0 degrees above the horizon, no more stucking at poles
	+ Serious average speed penalties at twighlight and at night time for manned rovers
	+ Some colors in arrival report
	+ Added toolbar support, fixed wrapper, no Contract Configurator conflict :D
	+ Switching to vessel with interface button will go without scene reload if vessel is already loaded
	+ 80% speed penalty for unmanned rovers
	+ UI and label are hidden if game is paused
	+ Label is hidden when all hidden (F2)
	+ Fixed crazy torpedoes nuking active vessel, rover simply won't move closer than 2400 meters to active vessel
	+ Fixed argument out of range caused sometimes and rover voyage end
	+ Route visualized with red line
	+ Route always visualized for active rover if exists
	+ Target can be set to active navigation point
	+ Added support for USI nuclear reactors
	+ Added support for NFE fission reactors
* 2016-1013: 0.10.2 (RealGecko) for KSP 1.2
	+ Recompile for KSP 1.2
	+ Fixed last waypoint being last step of voyage
* 2016-1003: 0.10.1 (RealGecko) for KSP 1.1.3
	+ Fixed control lock being applied to next switched vessel
	+ Moved "Bon Voyage control lock active" message higher on screen
	+ SAS is blocked by control lock too
	+ Path markers displayed at correct positions
	+ Fixed trying to build path to target closer than 1000 meters
	+ Fixed "yet to travel" distance for rovers awaiting sunlight being incorrect after scene switch
	+ Current rover changes in GUI list on vessel switch
	+ Fixed distance to target being incorrect if path is not staright
	+ Fixed errors raised at rover journey end when no/low time acceleration
	+ Fixed error switching to rover from Space Center
	+ Added ARES and Puma support
* 2016-1001: 0.10.0 (RealGecko) for KSP 1.1.3
	+ Fixed BV controller part being not in Control tab
	+ Shut down wheels are not treated as power consumers
	+ At least two wheels must be on to start BV
	+ Fixed utilites not being shown
	+ Added "Calculate average speed" and "Calculate power requirement" utilities
	+ Power production requirement diminished to 35% of powered wheels max
	+ Average speed now varies according to number of wheels on: 2 wheels - 50% of wheels' max speed, 4 wheels - 60%, 6 and more - 70%
	+ Rovers driven away from KSC by BV are not treated as landed at runway or launchpad anymore
* 2016-0929: 0.9.9.10 (RealGecko) for KSP 1.1.3
	+ Fixed errors in editor
	+ Fixed rover altitude being incorrect
	+ Added Malemute and Karibou compatibility
	+ Code optimization
	+ Pathfinding fully functional
	+ Interface revamp
	+ Module Manager patch to add BonVoyage to Malemute, Karibou and Buffalo cabs
* 2016-0916: 0.9.9.9 (RealGecko) for KSP 1.1.3
	+ Initial public release
* 2019-1010: 0.5.4 (RealGecko) for KSP 1.7.3
	+ Changes
		- Path is computed immediately after selecting a target (point on map, current target or waypoint)
	+ Fixes
			- Fixed null ref when closing main window after hiding the application button
			- Fixed distance display in the control panel after setting a route
* 2019-0829: 0.5.3 (RealGecko) for KSP 1.7.3
	+ Changes
		- Cheats for infinite electricity and propellant are accepted (your game, your rules)
		- Part upgrades were reworked to support their placement into different tech trees by MM's config
			- By default, two upgrades are in the stock tech tree (plus the base part). Third upgrade is placed into Artificial intelligence node of CTT if you have it.
* 2019-0826: 0.5.2 (RealGecko) for KSP 1.7.3
	+ Changes
		- BonVoyage modul added to RoveMate by default
		- Added part upgrades which raise the speed of unmanned rovers and ships. The last upgrade is only for Community Tech Tree.
* 2019-0823: 0.5.1 (RealGecko) for KSP 1.7.3
	+ Changes
		- Suport of servo rotors from the Breaking Ground DLC for ship controller
* 2019-0816: 0.5.0_-_Come_all_you_young_sailormen_listen_to_me (RealGecko) for KSP 1.7.3
	+ Changes
		- We are ready to extend our operations to the water
			- Stock jet and rocket motors are supported
			- Support of electric motors from Feline Utility Rovers and USI Exploration Pack mods (and other mods, which has motors based on stock modules)
			- Electric motors are the best ones for extended naval operations
		- MiniAVC removed from the installation, because planned updates are done. If you need notification about new versions, use AVC instead
