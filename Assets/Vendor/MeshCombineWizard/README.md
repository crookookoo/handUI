## What does it do?
Running the wizard will combine all the meshes on the chosen gameObject and its children that share the same material. If there is more than 1 material in all sub-objects, separate gameObjects will be created so that each object has one mesh with one material unique in the group, and will be parented under a new object. A prefab will be created from the combined gameObject in the root of the Assets folder, where all the newly created merged meshes will be created also. The original will be set inactive in the scene and the combined gameObject will be put in its position.

## How it works?
Put the provided script in any Editor folder. In the top menu a new entry will appear ("E.S. Tools/Mesh Combine Wizard"). Picking this option will run the wizard. Pick the object from the scene that you wish to combine and clik _Create_.

## Benefits:
* Lowers the amount of draw calls, usually dramatically.
* Does not need to draw all objects in the same batch regardless of wheter they are on screen or not, as opposed to static batching.
* Does not compromise workflow - no need to merge modular pieces outside Unity in a 3D tool. Quick iteration time when changing the combined design.
* Works with combined objects that have more than 64k verts, as opposed to solutions that manually set the verts and UVs.
* Does not compromise the original object's pivot point, as opposed to similar solutions.

## Known limitations:
* Does not support objects with multiple materials on submeshes. Such meshes should be split in an external 3D tool  to have separate meshes, 1 for each submesh and material pair of the original.