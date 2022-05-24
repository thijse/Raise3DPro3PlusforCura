# Raise3D Pro 3+ for Cura
Ongoing effort to create a printer profile for the Raise3d Pro 3(+) in Cura

## V1
Basic profile created based on IdeaMaker settings and Pro2 efforts. Start & Stop G-code was extracted from Ideamaker

### Issues 
- [ ] Check the speed (°C/s) by which the nozzle heats up and cools down 
   "machine_nozzle_heat_up_speed": {
   "machine_nozzle_cool_down_speed"
- [ ] Check if "machine_head_with_fans_polygon" setting is correct. In particular how to use with 2nd nozzle
- [ ] "machine_min_cool_heat_time_window" I don't know why this was set for so long for the Pro2, check      
- [ ] fix materials warning  "no profiles matching the configuration of this extruder"
- [ ] Check the length of material retracted during a retraction move copied from Ideamaker
- [ ]- Does the 2nd extruder have its own fan?  "machine_extruder_cooling_fan_number" : { "default_value": 1 }

### Result
Initial benchies failed

## V2
Updated profile with settings from Ultimaker S5 & Creatility, added quality variants, nozzle variants & materials (based on Creality)

### Issues 
- [X] fix materials warning  "no profiles matching the configuration of this extruder"

### Result
Benchies succeeded. Bottom looks very good, significant hull line, ugly overhangs with clear bridging. Possibly the cooling has not worked? 


