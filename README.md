# Picker3D

-Game play Video

https://user-images.githubusercontent.com/38994661/231297666-a346c80c-eb52-43b1-8cb5-4238b4742a5c.mp4

-Level editor video

https://user-images.githubusercontent.com/38994661/231297714-8e50513d-5994-4c0f-8e48-aeb2d6dd92ea.mp4

-SUMMARY-

A new level can be created from the level editor. For now, since each level contains 3 drop
areas, we can create the level after adding straight platform, level end platform, 3 drop platforms
and collectables.
Existing levels can be viewed, edited and deleted in the same level editor. When new levels are
added or deleted, they are automatically integrated into the level system after confirming them
in the editor window.
I used the facade pattern in the level system, i used json data format for the save system and
generally progressed according to solid principles in class structures.
I proceeded with emphasis on code optimization and memory management.

My own assessment of the tasks at the end of the project;

Start Screen

• Wait for input with tap to start script ✓

• Game starts when input is given ✓

Game Screen

• Player collects and carries objects with a tool to end of the game in an infinite platform like the
reference ✓

• Levels are shown with an indicator such as level 1, level 2 ✓

• Player can continue where the game is left when game restarts ✓

• Game has at least 10+ levels and then continues to infinity in a random order ✓

• Levels gets harder ✓

Game Over Screen

• If player reaches the designated number of collected objects, a button appears to proceed to
the next level ✓

• If player can not reach the designated number of collected objects, a button appears to replay
same level ✓

Level Editor Scene

• New Levels can be added from the editor scene ✓

• Old levels can be updated from the editor scene ✓

• 3+ different objects could be added or deleted from the editor scene ✓

• Level objects’ positions and rotations could be updated ✓

• Level complete counts can be editable from the editor scene ?
