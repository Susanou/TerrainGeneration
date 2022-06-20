# Landmass Generation

Procedural terrain generation project.

Using threads to produce the terrain chunks and letting the GPU handle some of the mesh generation for optimization.
This is a POC project to explore procedural generation.

## TODO

- [x] Shaders for texture using HLSL
- [x] Fix gaps in between chunks because of the difference in Level of detail
- [ ] Fix bug where only start chunk has a rigid mesh 

![terrain](images/terrain.png)