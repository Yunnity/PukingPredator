# How to use

- Put physicsEventListener on every object that can enable physics.
- If it's a single object, put SinglePhysicsObject on it too
- For a group where physics should be enabled at the same time, put everything inside an empty object and attach GroupPhysicsObject
 - if object is NOT being supported, eating a single object of the group will cause all children to enable physics too
 - if object IS being supported, we will wait until we lose supports before enabling all the physics
- If object is being supported, attach SupportedObject to it
- If object is a support, attach SupportingObjects to it, and add all objects it's supporting to its list