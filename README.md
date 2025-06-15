# NPC-AI-Simulation-Game
A downloadable simulation game where NPCs do tasks based on their needs and jobs.
Every 30 seconds, the npcs might do something different. 



Link to download: https://severedstars.itch.io/npc-ai-simulation

## Technologies Used:
C#, Godot

## Challanges
- Talking between NPCs was hard to implement as a task since a Task could only support one NPC at a time. But by handling the multiple NPCs in the NPC class instead of the PartnerTask or TalkTask class, I was able to have more than one NPC on a task.
- Optimizing finding tasks was difficult because there are so many variables that determine which Task gets picked. In the beginning, I tried to sense them with only collision, and would have to filter out tasks that were not usable. Then, I tried to find tasks that were attached to Places, but this meant that a task outside of a Place, like a SitTask on a bench, could not be found, or would have to rely on the old way. I eventually decided to find tasks using ITaskHolder, which would hold all of the tasks for an object and allow them to be found. And a more detailed GetTaskScore() function that considered a Task's needs, personality, and completeness meant that there wasn't much filtering needed when the tasks were found. 
- NPC avoidance was difficult. NPCs would never navigate around each other, even if they were stuck. And if they got stuck on a wall, they would never move. To solve this, I check if their previous position is the same as their current position, and teleport them further along their path. Since this does not involve the NPCs actually touching the ground and moving, it's not much of a final solution. But it does work and will not soft-lock the game.
