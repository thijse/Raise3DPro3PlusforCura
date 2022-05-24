![Raise3D 3 Pro for Cura logo](/Resources/logo.png?raw=true )
Raise3D Pro 3+ for Cura
====================
[![License](https://img.shields.io/badge/license-MIT%20License-blue.svg)](http://doge.mit-license.org)

*Raise3D printer profile for Cura*

This Github is an ongoing effort to create a good printer profile for the Raise3d Pro 3(+) in Cura. It defines, the printer, extruder, nozzles and materials. It also comes with an installer (script) that helps to install the printer profile for all installed instances of Cura. 

It is my hope that other people will try out and help tune the profile untill the quality is as good as can be expected from the printer.

## Installer
The installer requires InnoSetup to be present. It will  install the printer profile for all installed instances of Cura. Currently it has only been tested for Windows 10, but will likely work for other versions as well.  

## Resources
Here you can find an older but fully annotated verions of the  v1 profile. It also includes a file with only the Start- & Stop G-code, which has been traken directly from IdeaMaker

## Pro3plus_v1/Release
Basic profile created based on IdeaMaker settings and Pro2 efforts. Start & Stop G-code was extracted from Ideamaker

### Issues 
- [ ] Check the speed (°C/s) by which the nozzle heats up and cools down 
   "machine_nozzle_heat_up_speed": {
   "machine_nozzle_cool_down_speed"
- [ ] Check if "machine_head_with_fans_polygon" setting is correct. In particular how to use with 2nd nozzle
- [ ] "machine_min_cool_heat_time_window" I don't know why this was set for so long for the Pro2, check      
- [ ] fix materials warning  "no profiles matching the configuration of this extruder"
- [ ] Check the length of material retracted during a retraction move copied from Ideamaker
- [ ] Does the 2nd extruder have its own fan?  "machine_extruder_cooling_fan_number" : { "default_value": 1 }

### Result
Initial benchies failed.  ugly bottom, significant hull line, surface adhesion insufficient: benchy build from buildplate at +/- 50%

## Pro3plus_v1/Release
Updated profile with settings from Ultimaker S5 & Creatility, added quality variants, nozzle variants & materials (based on Creality)

### Issues 
- [X] fix materials warning  "no profiles matching the configuration of this extruder"

### Result
First succesfull Bency! The bottom looks very good, significant hull line, ugly overhangs with clear bridging. Current theory is that cooling fan flow is insufficient or non-existent



