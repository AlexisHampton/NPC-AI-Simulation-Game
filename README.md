# NPC-AI-Simulation-Game

Work in progress

A downloadable simulation game where NPCs do tasks based on their needs and jobs.

The new version, which supports more recent changes, will not be deployed yet; however, the old version still retains the simulation vision, albeit without all the new RPG elements.

Link to download: https://severedstars.itch.io/npc-ai-simulation

## Screenshots:
<img src="Screenshots/Screenshot1.png?raw=true" alt="An NPC feeding her kid" width="600">
<img src="Screenshots/Screenshot2.png?raw=true" alt="Two NPCs talking" width="600">

### From the new version
<img src="Screenshots/chopping wood screenshot.png?raw=true" alt="Player chopping trees" width="600">
<img src="Screenshots/crafting screenshot.png?raw=true" alt="Player crafting via a UI" width="600">
<img src="Screenshots/gardening screenshot.png?raw=true" alt="Player after planting a seed" width="600">


## Technologies Used:
C#, Godot

## Challenges
- One of the biggest challenges was figuring out if Inventories should hold ItemRs or Items. An ItemR is the name and sprite data of an Item. An Item is the physical object that would exist in the game world.  If Inventories held ItemRs, Items could be instantiated at runtime, but it would be very difficult to “pick up” Items since referencing an ItemR whilst an ItemR referenced that Item would create a circular dependency. On the other hand, if they held Items, gathering would be easy, but it would be difficult to instantiate Items. I tried the first approach of using Items, which made it easy to gather Items, but made interacting with crafting systems difficult, since they relied on ItemRs. In the end, I went with holding both of them. That way, I could have the Items that were already in the world and thus didn’t need to be instantiated, while also having the data of the ItemR for quick comparison in crafting and cooking systems. 
- Talking between NPCs was hard to implement as a task since a Task could only support one NPC at a time. But by handling the multiple NPCs in the NPC class instead of the PartnerTask or TalkTask class, I was able to have more than one NPC on a task.
- Optimizing finding tasks was difficult because there are so many variables that determine which Task gets picked. In the beginning, I tried to sense them with only collision, and would have to filter out tasks that were not usable. Then, I tried to find tasks that were attached to Places, but this meant that a task outside of a Place, like a SitTask on a bench, could not be found, or would have to rely on the old way. I eventually decided to find tasks using ITaskHolder, which would hold all of the tasks for an object and allow them to be found. And a more detailed GetTaskScore() function that considered a Task's needs, personality, and completeness meant that there wasn't much filtering needed when the tasks were found. 
- NPC avoidance was difficult. NPCs would never navigate around each other, even if they were stuck. And if they got stuck on a wall, they would never move. To solve this, I check if their previous position is the same as their current position, and teleport them further along their path. Since this does not involve the NPCs actually touching the ground and moving, it's not much of a final solution. But it does work and will not soft-lock the game.
