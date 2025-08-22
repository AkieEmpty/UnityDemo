●First,
Immediately after installing this asset, 
the character will not be displayed because there is no shader.
Install "jp.lilxyzw.liltoon-1.x.x-installer" in the "Shader_installer" folder.
“lilToon_Shader” will be added to the Package.

The physics simulation "MagicaCloth 2" has been set up.
It will work if you install a separate asset.

https://assetstore.unity.com/packages/tools/physics/magica-cloth-2-242307

=== [Notes] ====

●Please note that shape keys to prevent penetration are set in various places when putting on and taking off each costume.
You may also need to partially turn off physics simulation. Please refer to the prefab of each applicable costume.

●In this model, bone constraints are used in various places to perform more natural joint movement.
Please be careful about resetting the constraint when making modifications.

● The MagicaCloth settings use the Pre-Build function so that 
they function correctly even when the character starts playing in the middle of the animation. 
If you want to change the settings, turn off the "Pre-Build" function, 
make the changes, save the Pre-Build data again, and set it again.

●If you think you have enough capacity for the processing load of Physics by MagicaCloth, 
you can make simple setting changes such as making the reduction more fine-grained or 
turning on self-collision to achieve higher quality behavior with fewer errors.