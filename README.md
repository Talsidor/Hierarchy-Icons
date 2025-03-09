# Hierarchy-Icons
Unity Plugin to show component icons next to each object in the Unity Hierarchy

![image](https://github.com/user-attachments/assets/32675223-628b-43cd-a041-f47fcd1e3606)

## Getting Started

### Install via Package Manager

1. Open the Package Manager window in Unity
2. Click the "`+`" button in the top left corner and select "`Add package from git URL...`"
3. Enter the value "`https://github.com/Talsidor/Hierarchy-Icons.git`"
4. Hierarchy Icons should now be installed!

## Settings

Note: Hierarchy Icons' Settings are saved only for the local user (via Editor Prefs), so that different users in the same project can have different configurations.

### Set level

1. In your Unity Editor's top menu, navigate to `Tools > Hierarchy Icons > Set Level to...`
2. The current option will have a tick next to it, select a different level to change whether Hierarchy Icons are shown for:
  a. All Components,
  b. just Components that inherit from Behaviour,
  c. just Components that inherit from MonoBehaviour, or
  d. Disable the Hierarchy Icons plugin completely

![image](https://github.com/user-attachments/assets/db9e805c-16e1-4a16-bb76-f58ed8e18667)

### Overlap Repeating Icons

1. By default, Hierarchy Icons will overlap repetitions of the same icon
2. You can disable this by un-ticking `Tools > Hierarchy Icons > Overlap Repeating Icons`

![image](https://github.com/user-attachments/assets/b01d25e2-1845-4a19-a5c2-8d4385e57931)

### Show if a Light's Mode is set to Baked

If enabled, an icon of a birthday cake will be shown next to the icon for Light components if their [LightmapBakeType](https://docs.unity3d.com/ScriptReference/Light-lightmapBakeType.html) is set to Baked.

Off by default.

### Show Static Editor Flags

If enabled, a lock icon will be shown on objects that have any of their [Static Editor Flags](https://docs.unity3d.com/ScriptReference/StaticEditorFlags.html) raised.

The lock will be fully locked if all the flags are raised, and slightly open if only some are raised.

On by default.