# Greetings, Commanders!

## Summary
Twitch Clip : [E:D GPS Demonstration](https://www.twitch.tv/videos/430282763)

Recent Updates : [Updates - Index](https://github.com/ComputerMaster1st/EdGPS/wiki)

The tool works by constantly watching the newest "journal*.log" for any changes and processes those changes into usable data to be displayed. It's basically a system map combined with galaxy map, although it's pretty basic. It also saves the system data to a sub-directory so you have your own little "Universal Cartography" database. It'll also load up the last system you were in on startup. From a user's perspective, it can be really handy to have running in the background as you can just take a quick glance at it and know what you want to DSS as well as what you've first discovered.

Thanks to how simple it is, it requires no interaction at all from the user other than needing the journal directory. Plus it has zero clutter and is very lightweight.

## Important Bits
When it comes to first discovery, the GPS will mark it with a tag. However, this can sometimes not be true as it is after all, reading from journal log files. The same could apply to mapping. The GPS was never designed to be mass used, therefore lacking some features. There may also be missing body types such as neutron stars, etc as I don't know what they come under as inside the journal log.

The E:D GPS is just a concept and was never designed for production usage. Please expect bugs/issues.

## Console UI (Display)

* System Name
* Target System Name (*Only displayed as you enter hyperspace*)
* System Co-ordinates
* Scan Status
* Number of (non-)bodies (*Only shown after discovery scan*)
* (Non-)Bodies & their position in the system.

There are various "tags" which are shown, depending on various conditions.

`[Discovered]` - Shows you've first discovered the body

`[BlackHole]` - Black hole

`[NeutronStar]` - Neutron Star

`[WhiteDwarf]` - White Dwarf

`[* Class]` - Star Class

`[* ls]` - Distance to body in light-seconds

`[* World]` - World/Moon Class

`[Is Mapped]` - Body already DSS Scanned

`[DSS Complete]` - Body fully DSS Scanned

`[Terraformable]` - Body is candidate for terraforming

`[Awaiting FSS Discovery Scan]` - Waiting for discovery scan

`[System Scan Complete]` - System Scan Completed

You'll also find there'll `(x)` where the body name should be. This is to indicate that it's either not discovered yet, or is a ring/barycenter. It's not that important basically. The tree in which the bodies are displayed in are ordered the same way as the system map (*although questionable due to how the logs are written*).

## Download
Windows : [x86](https://www.dropbox.com/s/xrwtf94y4f71njx/EdGPS-x86.zip?dl=0) | [x64](https://www.dropbox.com/s/cumn62aeyiewyjj/EdGPS-x64.zip?dl=0) (Last Compiled @ 8th June 1:20PM UTC 0)

### Compile From Source
If you wish to download & compile from source, please be sure to have the [.NETCore 3.0 SDK](https://dotnet.microsoft.com/download) installed and have knowledge on using it.

## Start E:D GPS
To launch the GPS, find & run "EdGps.exe". All builds are self-contained. This means you do not need to download any additional software. When you first launch, the GPS will begin auto-building all the system data from the journal logs. This process will take some time depending on how many & how big the logs are. If you wish to rebuild the system data, simply delete the directory and re-launch the GPS.

## How to turn off voice?
When you launch the GPS, it'll generate a `config.json` file. Inside, change the value of `VoiceEnabled` from `true` to `false`.
