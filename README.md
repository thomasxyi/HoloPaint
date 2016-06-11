Steps to run:
Open Unity scene named "final"
Input computer ip address in sharing prefab
Run sharing service
Deploy to hololens

If need to add new holograms, add a mesh collider, P3D_Paintable.cs and TexturePainter.cs to the model. In P3D_Paintable, make sure to check Duplicate on Awake and Create on Awake for the texture.
Also add PreInstantiateHologram.cs and add a unique number to identify the model for sharing in Reserved UID field in the script.
If you would like to rotate and move the hologram add HologramPlacement and HologramRotation.cs.
Once the new hologram has the scripts that you want, drag it to become a child of the DefaultModels gameobject.