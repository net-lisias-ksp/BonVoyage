# Bon Voyage /L

Rovers background processing for KSP.


## In a Hurry

* Documentation
<!--	+ [Homepage](http://ksp.lisias.net/add-ons/BonVoyage/) on L Aerospace -->
	+ [Project's README](https://github.com/net-lisias-ksp/BonVoyage/blob/master/README.md)
	+ [Install Instructions](https://github.com/net-lisias-ksp/BonVoyage/blob/master/INSTALL.md)
	+ [Change Log](./CHANGE_LOG.md)
	+ [Known Issues](./KNOWN_ISSUES.md)
	+ [Road Map](https://github.com/net-lisias-ksp/BonVoyage/blob/master/ROAD_MAP.md)
* Official Distribution Sites:
<!--	+ [CurseForge](https://kerbal.curseforge.com/projects/BonVoyage) -->
<!--	+ [SpaceDock](https://spacedock.info/mod/127/BonVoyage) -->
	+ [Latest Release](https://github.com/net-lisias-ksp/BonVoyage/releases)
		- [Archive](https://github.com/net-lisias-ksp/BonVoyage/tree/Archive)
* Support
	+ [Homepage](http://ksp.lisias.net/add-ons/BonVoyage/Support/) on L Aerospace
<!--	+ [Forum](https://forum.kerbalspaceprogram.com/index.php?/topic/*-*/) -->
<!--	+ [Discussions on Github](https://github.com/net-lisias-ksp/BonVoyage/discussions/categories/support) -->
* [Source](https://github.com/net-lisias-ksp/BonVoyage)
	+ [Issue Tracker](https://github.com/net-lisias-ksp/BonVoyage/issues)


## Description

What's the best way of getting science from celestial body? The answer is DRIVE!

What's the best way of delivering ore or supplies from distant location to your base? The answer is make your wheels rolling!

But did you try to drive for 110 km for two hours, avoiding obstacles and keeping your speed in sane limits? It's not fun anymore.

Don't worry! Automagic Industries brings you a new autopilot, which reduces driving accidents by 100%. Bon Voyage is here and it will drive your rovers in background!

### How it Works

1. Rover must be landed
2. It has to have BonVoyage controller aboard, seek in Control tab or use some cool rover mod (listed below)
3. All rover wheels must be intact
4. At least three of rover's operable wheels must be on the ground
5. Rover must be landed
6. Rover will move with average speed varying from 50% to 70% of wheels max speed (can be boosted up to 95% by skilled pilot)
7. To start moving your power production must be at least 35% of wheels max consumption
8. Rover can be solar powered
9. Rover must be landed
10. Rover can have alternative power source
	* RTGs, Nuclear Reactors, Fission Reactors, Antimatter Reactors, whole bunch of reactors is supported, if it has "reactor" in it's name than it will definitely work
11. If rover cannot move without solar power it will idle until Sun is 0 degrees above the horizon
12. **ROVER MUST BE LANDED!!!**

### How to:

1. Add BonVoyage module to your rover (seek in Control tab)
2. If you have MM installed, then Malemute, Karibou, Buffalo, ARES, Puma and FUR cabs get BV automatically, no need to use additional part. 
3. Launch your rover
4. If you're just playing with BV and launch a rover at KSC, then at start your rover state will be "Prelaunch", BV won't work at that state. Move your rover a little so that MET starts ticking and then BV is ready to be activated
5. Right click on BonVoyage module (or rover cab if you use some cool rover mod listed above)
6. Click "Pick target on map" or "Set to active target" or "Set to active waypoint"
7. Path finding may take some time, be patient (C# is not the power horse :D).
8. You can go to map mode and see built route for inspection
9. If path was not found in ten seconds it will break, try some closer point
10. Click "Poehali!!!"
11. Go to tracking station...or launch a new vessel...or do whatever you want...
12. ??????
13. PROFIT!!!!!

### Some math

Power requirements calculations are really simple. Let's say you've built a rover on Malemute chassis with six Malemute Modular Wheels, each of 'em has max power consumption equal to 3.5. This means overall max consumption is 3.5 * 6 = 21. BV "requires" that your max power production is equal to 35% of max wheels' consumption 21/100*35 = 7.35 EC/s. So if your solar panels and/or RTGs provide 7.35 electric charge per second, then you're good to go. Your rover average speed will be 70% of wheels' max speed. In case of Malemute Modular Wheels it will be 35/100*70 = 24.5 m/s.

Of course 7.35 charge per second is not really easy to achieve in case of rovers. What other options do you have? The answer is pretty easy: shutdown motor on two of your wheels. This will reduce power requirement to 3.5*4/100*35 = 4.9 EC/s. But beware that your rover average speed will fall to 60% - 35/100*60 = 21 m/s.

If you disable two more wheels, then you power requirement will be just 3.5*2/100*35 = 2.45 EC/s, but average speed will fall to 50% - 35/100*50 = 17.5 m/s.

### Motes

1. Pilots, USI Scouts and crew members with Autopilot skill can increase average speed dramatically, train your crew :wink:
2. Low on power? Your rover will be sloooow
3. Night rides are slooooooooow
4. Unmanned rovers are sloooooooooooooooooow

There is a plan for average speed to vary acco﻿rding to gravity of celestial body and mass of rover. If you can propose some better math, then d﻿o it﻿

### Quick Guide

1. Build your rover as usual
2. Add solar panels
3. Add moar solar panels
4. Grab some RTGs
5. Don't forget couple of nuclear reactors
6. KSPI antimatter reactors are pretty powerful, add at least one.
7. Now for wheels: more wheels = better. Add a dozen of 'em, it does not even matter if your wheels point in different directions, just have a lot of 'em.
8. Take your most skilled pilot and send him on vacation, he already got mental trauma while you were building the rover described above.
9. Add probe core to your rover.
10. Don't forget to add some powerful radiators to dissipate heat from probe core overloaded with the mess you've done building the rover described above.
11. Done!
	* Your rover will be able to deal with any slope and any crater in The Universe....if pathfinding algorithm won't fail first...

 
## Installation

Detailed installation instructions are now on its own file (see the [In a Hurry](#in-a-hurry) section) and on the distribution file.


## Licensing

* Bon Voyage's Source code is licensed under the [GPL 3.0](https://www.gnu.org/licenses/gpl-3.0.txt).
	+ You are free to:
		- Use : unpack and use the material in any computer or device
		- Redistribute : redistribute the original package in any medium
		- Adapt : Reuse, modify or incorporate source code into your works (and redistribute it!)
	+ Under the following terms:
		- You retain any copyright notices
		- You recognise and respect any trademarks
		- You don't impersonate the authors, neither redistribute a derivative that could be misrepresented as theirs.
		- You credit the author and republish the copyright notices on your works where the code is used.
		- You relicense (and fully comply) your works using GPL 3.0
			- Please note that upgrading the license to any future license version  **IS NOT ALLOWED** for this work, as the author **DID NOT**
	 added the "or (at your option) any later version" on the license
		- You don't mix your work with GPL incompatible works.
* "Bon Voyage Pathfinder" model is licensed under CC-BY-NC-SA 4.0.
* App launcher icon is licensed under CC BY 3.0.

See [here](./LICENSE).

Please note the copyrights and trademarks in [NOTICE](./NOTICE).


## UPSTREAM

* [RealGecko](https://forum.kerbalspaceprogram.com/index.php?/profile/162682-realgecko/) ROOT
	+ [Forum](https://forum.kerbalspaceprogram.com/index.php?/topic/147876-131-bon-voyage-01321-make-your-wheels-rolling-2017-10-15/)
	+ [SpaceDock](https://spacedock.info/mod/950/BonVoyage)
	+ [Imgur](https://imgur.com/a/bonvoyage-alpha-Zq28v)
	+ [GitHub](https://github.com/Real-Gecko/BonVoyage)  
* [Maja](https://forum.kerbalspaceprogram.com/index.php?/profile/168379-maja/) Previous Maintainer
	+ [Forum](https://forum.kerbalspaceprogram.com/index.php?/topic/172447-15x-bon-voyage-make-your-wheels-rolling-01411-2018-11-10/)
	+ [SpaceDock](https://spacedock.info/mod/950/BonVoyage)
	+ [GitHub](https://github.com/jarosm/BonVoyage)
