# Preliminary information
(Note: This is not actually a report I was required to do for WWSEF! I just thought it'd be nice to have someplace to write an in-depth explanation for all my steps.)  

Rogue planets remain one of the biggest mysteries of the universe that we know so little about. After the discovery of exoplanets, it seems natural to think that planets that don't orbit a star
but are instead present orbiting the Milky Way itself are something that could exist in large abundance.  

Indeed, this abundance is true! Almost 20 years after discovering rogue planets, scientists have therorized that they may exist in the trillions. However, knowledge about their exact fates (ie.
ejected, captured, etc) is still lacking in many departments.  

Why is this? Can't we just do direct imaging or check for the planet eclipsing a star? Unfortunately, we can rarely use these methods since potential rogue planets are so far away. Any attempts
at trying to capture dips in the star's brightness will be far too subtle and rogue planets have no light from stars to reflect back to us, so direct imaging is out. The only real option left
is to utilize information from **microlensing**!

# What is microlensing?
Massive objects bend spacetime. All objects want to move in a straight path through spacetime, but the curvature of spacetime causes these objects to follow a curved path instead (relative to
our frame of reference). Luckily for us, light also moves through spacetime, so it is bent by massive objects as well!

![1567218653166-lores_43543](https://github.com/user-attachments/assets/7b48c4f4-e31f-4eff-9eb6-8f077f678cc2)![images](https://github.com/user-attachments/assets/28349aee-6818-45ea-a246-33521a9170f2)

When a massive object bends these lightrays towards us (usually from a star), we can observe a microlensing light curve, similar to the one above. This also results in a characteristic ring
called an Einstein ring, which we can determine physically (literally counting the pixels!) or mathematically, using the following formula:

$\theta_E = \sqrt{\frac{4GM}{Dc^2}}$

Where $\theta_E$ represents the Einstein radius, M represents the mass of the object, and D represents the distance from us to the lens + the lens to the source.  

The good news? We have a method to detect any object that is massive enough to warp spacetime (which includes rogue planets)! Bad news? We don't know M or D, both of which we need to classify
what kind of object we are observing. Thankfully, this is not the only way to represent the einstein radius, as there are many other ways to write the formula using different known parameters.
Using these other equations are helpful, as the only real way to solve for M and D is to find which values give the best fit to our light curve, and using other known parameters can help narrow
down our options.

