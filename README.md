# Resonite Modding
This is where I spend much of my time trying to make Resonite a tolerable platform.

## What should this mod even do?
Not much yet.  
Right now, the core ideas are all over the place. Knowing myself, its gonna contain a bunch of general-purpose utilities that need to be split up into their own mods.

On paper I intend for this to become an avatar creation utility. Broad goals are as follows:
- [ ] Customize the avatar build process
  - [ ] Non-destructive build pipeline (like VRChat+VRCFury)
  - [ ] Set up dynamic bone chains automatically
  - [ ] Set up linked blendshapes automatically
  - [ ] Set up IK fixes automatically
    - [ ] Arrange proxies
    - [ ] Full body IK fixes
  - [ ] Set up local head scaling automatically
  - [ ] Set up variable spaces
- [ ] Handle importing of an avatar straight from its source files
  - [ ] Asset cache (speed up consequtive builds by reusing textures and other assets)
- [ ] Avatar assembly in Unity
  - [ ] Exporting to package that the mod can import
    - [ ] Ideally export straight to a `.resonite_package`
  - [ ] Translate Unity features to Resonite
    - [ ] Components
    - [ ] Animations
    - [ ] Default values
      - [ ] Blendshapes
      - [ ] Poses
    - [ ] Assets
      - [ ] Materials
      - [ ] Meshes
      - [ ] Textures
  - [ ] Translate VRChat/VRCFury features to Resonite
    - [ ] Blendshape link
    - [ ] Armature link
    - [ ] Modules
      - [ ] Lazy asset loading
- [ ] Optimizer
  - [ ] Slot count reducer
  - [ ] Blendshape optimizer
  - [ ] Trimming
    - [ ] Removing extra features
    - [ ] Asset crunching

---
Its a long list of ambitions, but I want to get started with the basic parts of automated importing.
I want the initial versions of this mod to handle importing the FBX and textures and then correctly assign all the assets.
I also want it to always place my proxies and anchors the same way every time, either by copying from another avatar, or by basing them off of a reference pose.
That should save me a lot of time.

Everything else (especially the Unity assembling part) is cool on paper, but likely extremely time consuming in practice.
My main priority is creating a tool that does all the things that I don't want to.

## What's it got so far?
Besides that checklist from earlier, there are some other things I already have finished. These things most definitely distracted me from the main goal, but I can't help myself sometimes.

- Context shortcuts
  - Defining an `IShortcut` with a single constructor parameter lets me create extra context menu items when holding an object with a compatible type.  
    For example, I can create a shortcut where I can just spawn a slider for any `IField<float>` that I hold. I can do this by creating a `class SpawnSliderShortcut(IField<float> field) : IShortcut`.
- Toast popups
  - I found myself wanting a way to let my mod notify the current user without it being clunky or requiring the logs. Thats what the toast popups are for.
    That being said though; I have this nagging feeling like there's already code lying around in Resonite that essentially does the same thing. I haven't found it yet though.
- 

---

I also slapped my utilities project in here. Its basically just a fat toolbox that I drag around from project to project.  
Yes, I should really use git submodules for this, but using submodules is also clunky so I just keep committing different versions of the utilities project.
