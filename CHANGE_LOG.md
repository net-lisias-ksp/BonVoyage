# Bon Voyage :: Archive

* 2021-0309: 1.3.1 (Maja) for KSP 1.11.1
	+ Changes
		- Chinese localization updated
		- Russian localization by kovakthemost
		- Added BonVoyage to Mk2 Lander Can
		- Added height offset for rovers to the Settings
	+ Fixes
		- Fixed usage of fuel cells with infinite propellant cheat enabled and no fuel tanks present
		- Fixed wrong count of operable wheels with KSPWheel module when "Wear and damage" setting was set to NONE
		- Fixed detection of usable converters when fuel cell contains more modules of type ModuleResourceConverter
* 2021-0207: 1.3.0 (Maja) for KSP 1.11.1
	+ Changes
		- Rebuild for KSP 1.11.1
		- Kerbals can move behind the scene too (with a blinding speed of 0.5m/s - depends on the celestial body)
		- Usage of batteries is switched off during system check and user is notified about it when there isn't enough power to recharge a rover
		- Chinese localization by Raven-233486
		- French localization by vinix38
		- Added module for stock EVA construction
* 2020-0812: 1.2.0 (Maja) for KSP 1.10.1
	+ Changes
		- Rebuild for KSP 1.10.1
		- Pathfinder optimization - it's quicker :)
		- New configuration value for pathfinding timeout was added. Default value is 10 seconds. You can change it in config.xml of BonVoyage by changing the value on this line: `<double name="pathfinderTimer">10</double>`
* 2020-0812: 1.1.1 (Maja) for KSP 1.9.1
	+ Changes
		- Pathfinder optimization - it's quicker :)
		- New configuration value for pathfinding timeout was added. Default value is 10 seconds. You can change it in config.xml of BonVoyage by changing the value on this line: `<double name="pathfinderTimer">10</double>`
* 2020-0810: 1.1.0 (Maja) for KSP 1.9.1
	+ Changes
		- Kerbalism compatibility
		- Critter Crawler compatibility
		- Added the option to show biome info in the map view on or off. Off by default.
		- KSP 1.9.1 compatibility
	+ Fixes
			- Fixed wrong check of output EC ratio of fuel cells
* 2019-1104: 1.0.1.1 (Maja) for KSP 1.8.1
	+ Changes
		- Minimum height of a rover over the terrain was changed to avoid some clipping issues
		- KSP 1.8.1 compatibility
* 2019-1020: 1.0.0 (Maja) for KSP 1.8.0
	+ Changes
		- KSP 1.8 compatibility
* 2019-0814: 0.14.9 (Maja) for KSP 1.7.3
	+ Fixes
		- Fixed coordinates when a target is selected on the map
	+ Changes
			- KSP 1.7.3 compatible
* 2019-0623: 0.14.8 (Maja) for KSP 1.7.2
	+ Changes
		- KSP 1.7.2 compatible
		- Fuel cells are switched off during a night, when they don't have enough power to recharge batteries (when they are used as a complement to solar panels), to not waste a fuel.
* 2019-0610: 0.14.7 (Maja) for KSP 1.7.2
	+ Fixes
		- Fixed null ref for the new game
		- Fixed loading of controllers into the main window
		- Fixed issue with control window moving out of screen
		- Max speed of stock wheels (including Making History wheels) is reported properly
* 2019-0609: 0.14.6 (Maja) for KSP 1.7.1
	+ Fixes
		- Some nullrefs and indexes out of range were fixed
		- Fixed max speed check for Making History expansion foldable wheels (typo in the name of wheels)
* 2019-0603: 0.14.5 (Maja) for KSP 1.7.1
	+ Fixes
		- Fixed bulk profile category
		- Fixed compatibility with KSPWheelTracks
	+ Changes
			- Recompile for KSP 1.7.1
* 2019-0428: 0.14.4 (Maja) for KSP 1.7.0
	+ Recompile for KSP 1.7
* 2018-1204: 0.14.3_-_Power_of_LOx (Maja) for KSP 1.5.1
	+ Changes
		- Fuel cells support
		- Added Reload button to the main window to refresh list of vessels without scene switch
* 2018-1117: 0.14.2 (Maja) for KSP 1.5.1
	+ Changes
		- Unmanned rover must have an active connection to set a target or issue the GO command if you are using the CommNet or RemoteTech
			- Batteries can be used during a night, if there is enough solar power to recharge them. Up to 50% of the total capacity of all enabled batteries will be used.
		- Added toggle to the Settings to disable rotation of a rover perpendicularly to the terrain after arriving to a target and during a ride
			- Added Rotation vector advanced tweakable
			- Rotation of a rover depends on the orientation of the root part. You can now set the vector used for rotating the rover.
			- This setting is accessible after enabling Advanced tweakables in the KSP settings
			- Default value is "Back" - for rovers, whose root part is a probe or a cab oriented in such a way, that you see horizont line on the navball
			- Other usual values are "Up" and "Down", if the default setting is putting your rover on it's (usually) shorter side. You need experiment a little bit in this case to find the right setting.
* 2018-1110: 0.14.1.1 (Maja) for KSP 1.5.1
	+ Fixes
		- Fixed detection of KSP Interstellar Extended generators
	+ Changes
			- Kopernicus support - solar panels are working even when you are around other stars
			- Added support for Bison Cab from Wild Blue Industries
			- Bon Voyage will try to rotate a rover perpendicularly to a terrain
* 2018-1104: 0.14.0_-_New_voyage (Maja) for KSP 1.5.1
	+ Changes
		- KSP 1.5.1 compatibility
		- Major overhaul - We are getting ready to extend our operations to the water in the future.
		- Localization support
		- KSPWheel module system check change
			- Required EC is scaled by a motor's output setting
			- Maximum speed is taken from maxDrivenSpeed field, which is scaled by gear setting, and capped at max safe speed
		- Direct input of target coordinates
		- Stabilization of a rover during scene switching into flight, if it's moving or just arrived at a destination. The function is switched off, if World Stabilizer or BD Armory is present.
		- Added MiniAVC
		- Updated Wiki
* 2018-1105: 0.14.0.1 (Maja) for KSP 1.5.1
	+ Fixes
		- Fixed rover skipping kilometers forward to the target under some circumstances
		- Removed forgotten harmless debug warning message
	+ Changes
			- Added tooltip to the *System check* button to better explain it's function
* 2018-0316: 0.13.3 (Maja) for KSP 1.4.1
	+ Recompile for KSP 1.4.1
* 2017-1014: 0.13.2.1 (Maja) for KSP 1.3.1
	+ Fixes
		- Fixed required power of wheels with KSPWheel module
* 2017-1007: 0.13.2 (Maja) for KSP 1.3.1
	+ Changes
		- KSP 1.3.1 compatibility
		- Tooltip change
		- Pilots, USI Scouts and anyone with AutopilotSkill affect speed
	+ Fixes
			- Various fixes
* 2017-0929: 0.13.1 (Maja) for KSP 1.3.0
	+ Changes
		- Displayed information revision
		- Change in background processing (TAC-LS compatibility!)
		- Pilots and USI Scouts affect rover speed depending on their level
	+ Fixes
			- Fixed drawing of the line to a target
			- Fixed wrong path to images on Linux
			- Various fixes
* 2017-0913: 0.13.0 (Maja) for KSP 1.3.0
	+ Changes
		- KSP 1.3 compatibility
		- WBI reactors and MKS power pack compatibility
		- Support for tricycles
		- Hide BV window in map view
		- Average speed change - reduction based on power
		- Shutdown/Activate BV Controller
	+ Fixes
			- Fixed target longitude display
			- App launcher button fix
			- Adjusted vessel altitude from terrain
			- Night time ride fix
* 2017-0317: 0.12.0 (Maja) for KSP 1.2.2
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
