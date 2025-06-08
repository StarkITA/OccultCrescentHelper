# OccultCrescentHelper

Repo: https://raw.githubusercontent.com/OhKannaDuh/plugins/refs/heads/master/manifest.json

## Features

-   Instance Id
-   Treasure radar
    -   Lists nearby treasure and draws a line to them
-   Carrot radar
    -   Lists nearby carrots and draws a line to them
-   Siver/Gold per hour tracker
-   Exp per hour tracker
-   Active fate/ce tracker
    -   Displays demiatma and notes dropped by fate/ce
    -   Displays fate/ce progress
    -   Displays estimated completion time
    -   Button to teleport, mount and pathfind to fate/ce
    -   Automatic return after fate/CE

## Plans

-   Auto buffs (Bard/Knight/Monk)
-   Auto chest run
-   Auto find active carrot
-   Instance hopper 
    -   `/och instance 54` (Or something like this), Then it keeps leaving and joining until you get into that instance

## Known issues

- Can fail to return if YesAlready (or similar) catches the SelectYesno window before OCH does (Race condition)
- Some have reported that pathfinding doesn't work for them, however, I am unable to replicate this or debug a reason with them, more information required
