# How to use

- Put physicsEventListener on every object that can enable physics.
- If it's a single object, put SinglePhysicsObject on it too
- For a group where physics should be enabled at the same time, put everything inside an empty object and attach GroupPhysicsObject
- If object is being supported, attach SupportedObject to it
- If object is a support, attach SupportingObjects to it, and add all objects it's supporting to its list