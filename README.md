![](landing_page_images/cover.png?raw=true)

You’re in charge of curating for tonight’s art show and you only just started working at the gallery. It probably doesn’t help that this place just seems to get deeper and deeper. 
Well, what are you waiting for? Head on back, and grab some paintings for tonight’s show! Remember… just don’t get lost…

[![](https://img.youtube.com/vi/mAFM4sTPE2Q/maxresdefault.jpg)](https://www.youtube.com/watch?v=mAFM4sTPE2Q)


**AI Generated Art:**
 
![](landing_page_images/art.gif?raw=true)

All of the artwork is generated using a neural network. That means each painting you see is completely unique and one of a kind! Find one you like? Hold on tight and don’t lose it! 

**Objective:**

Curate 6 pieces of artwork from an infinite museum. Choose paintings of the right size and synergy. Will tonight’s art show be a success?

**Controls:**

WASD – Movement

Left Click – Grab/Place

T - Teleport

Q - Quit

**Screenshots:**

![](landing_page_images/screen1.png?raw=true)
![](landing_page_images/screen2.png?raw=true)
![](landing_page_images/screen3.png?raw=true)

**Hardware**:

We are working on trying to optimize the game for lower-end hardware. Right now the neural network will slow down the game at some points if your hardware isn't fast enough.

**About**:

This game was an entry to the Ludum Dare 48 Jam which took place on April 23-26, 2021. The entry page can be found here: https://ldjam.com/events/ludum-dare/48/arterial

To run this game, load it as a Unity project and then click play in the Unity editor. Alternatively you can use Unity to build your own version to package up. This project is also a good example of how to get started using neural networks in Unity! The code is relatively clean for a jam so it should give you some guidance. The model used in this version was a 32x32 DCGAN trained on ImageNet. Better models could be used instead, but you would have to work on setting up some better scheduling for the image generation to avoid chunk loading lag.

**Follow Us**

If you're interested in keeping up with more of our work, you can find us on twitter [here](https://twitter.com/VRealitySucks).

**Post-deadline edits and fixes**:
4/26/21: bug fix for start room crashing when going too far and player spawning glitch.
