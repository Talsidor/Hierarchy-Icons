
using UnityEditor;
using UnityEngine;

using System;
using System.Linq;

/// <summary>
/// Hierarchy Window Component Icons
/// Thanks to Diego Giacomelli for inspiring this package with this article!
/// http://diegogiacomelli.com.br/unitytips-hierarchy-window-gameobject-icon/
/// </summary>
[InitializeOnLoad]
public static class ShowScriptIconsInHierarchy
{
	#region EDITOR PREF NAME CONSTS

	const string _editorPref_displayLevel = "_hierarchyIcons_displayLevel";
    const string _editorPref_overlapping = "_hierarchyIcons_overlapping";
	const string _editorPref_static = "_hierarchyIcons_static";
	const string _editorPref_bakedLights = "_hierarchyIcons_bakedLights";

	#endregion EDITOR PREF NAME CONSTS

	#region MENU ITEM NAME CONSTS

	const string _menuItem_prefix = "Tools/Hierarchy Icons/";
    const string _menuItem_levelPrefix = _menuItem_prefix + "Set Level to.../";

	const string _menuItem_overlap = _menuItem_prefix + "Overlap Repeating Icons";
	const string _menuItem_static = _menuItem_prefix + "Show Static Editor Flags";
	const string _menuItem_bakedLights = _menuItem_prefix + "Show if a Light's Mode is set to Baked";

	const string _menuItem_level0 = _menuItem_levelPrefix + "0. None";
	const string _menuItem_level1 = _menuItem_levelPrefix + "1. Only MonoBehaviours";
	const string _menuItem_level2 = _menuItem_levelPrefix + "2. Only Behaviours";
	const string _menuItem_level3 = _menuItem_levelPrefix + "3. All Components";

	#endregion MENU ITEM NAME CONSTS

	enum DisplayLevel
    {
        None,
        MonoBehavioursOnly,
        BehavioursOnly,
        AllComponents,
    }

    static DisplayLevel _displayLevel;
    const DisplayLevel _defaultDisplayLevel = DisplayLevel.AllComponents;

    static bool _overlapping;
	static bool _showStatic;
	static bool _showBakedLights;

	static Texture _bakedIcon;

	static ShowScriptIconsInHierarchy()
    {
		GetAllEditorPrefs();

        UpdateMenuItemCheckedStatuses();

        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

	static void GetAllEditorPrefs()
	{
		GetDisplayLevel();
		GetOverlapping();
		GetShowStatic();
		GetShowBakedLights();
	}

    static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID);

        if (!obj)
        {
            //Debug.LogError($"No object could be found with instance id '{instanceID}'");
            return;
        }

		var gameObj = obj as GameObject;

        if (!gameObj)
        {
            Debug.LogError($"obj '{obj.name}' is not a GameObject");
            return;
        }

        switch (_displayLevel)
        {
            default:
            case DisplayLevel.None:
                break;

            case DisplayLevel.MonoBehavioursOnly:
                DrawIconIfHasComponent<MonoBehaviour>(gameObj, selectionRect);
                break;

            case DisplayLevel.BehavioursOnly:
                DrawIconIfHasComponent<Behaviour>(gameObj, selectionRect);
                break;

            case DisplayLevel.AllComponents:
                DrawIconIfHasComponent<Component>(gameObj, selectionRect);
                break;
        }
    }

    static void DrawIconIfHasComponent<T>(GameObject gameObj, Rect selectionRect) where T : Component
    {
        const int size = 16;

        int offset = 0;

        var components = gameObj.GetComponents<T>();

        Texture lastIcon = null;

        foreach (var component in components)
        {
            var content = EditorGUIUtility.ObjectContent(component, typeof(T));

			if (_overlapping &&
                content.image == lastIcon)
            {
                offset += (size / 3);
            }
            else
            {
                offset += size;
            }

            var rect = new Rect(selectionRect.xMax - offset, selectionRect.yMin, size, size);

            GUI.DrawTexture(rect, content.image);

            lastIcon = content.image;

			if (_showBakedLights && 
				component is Light light &&
				light.lightmapBakeType == LightmapBakeType.Baked)
			{
				if (_bakedIcon == null)
				{
					_bakedIcon = AssetDatabase.LoadAssetAtPath<Texture>("Packages/com.talsidor.hierarchy-icons/cake-24px.png");
				}

				offset += 12;
				rect = new Rect(selectionRect.xMax - offset, selectionRect.yMin, size, size);
				GUI.DrawTexture(rect, _bakedIcon);
			}
        }

		if (_showStatic)
		{
			offset += size;

			var rect = new Rect(selectionRect.xMax - offset + 2, selectionRect.yMin + 2, 11, 11);

			DrawStaticStatus(gameObj, rect);
		}
	}

	static void DrawStaticStatus(GameObject gameObj, Rect selectionRect)
	{
		const StaticEditorFlags allUsedFlags = (StaticEditorFlags) 0b_1110_1010;

		StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(gameObj);

		if (flags.HasFlag(allUsedFlags))
		{
			GUI.DrawTexture(selectionRect, EditorGUIUtility.IconContent("LockIcon-On").image);
		} 
        else if (flags > 0)
        {
			GUI.DrawTexture(selectionRect, EditorGUIUtility.IconContent("LockIcon").image);
		}
    }

    #region Menu Items

    static void UpdateMenuItemCheckedStatuses()
    {
        Menu.SetChecked(_menuItem_level0, _displayLevel == DisplayLevel.None);
        Menu.SetChecked(_menuItem_level1, _displayLevel == DisplayLevel.MonoBehavioursOnly);
        Menu.SetChecked(_menuItem_level2, _displayLevel == DisplayLevel.BehavioursOnly);
        Menu.SetChecked(_menuItem_level3, _displayLevel == DisplayLevel.AllComponents);

        Menu.SetChecked(_menuItem_overlap, _overlapping);
		Menu.SetChecked(_menuItem_static, _showStatic);
		Menu.SetChecked(_menuItem_bakedLights, _showBakedLights);
	}

    [MenuItem(_menuItem_level0)]
    static void SetDisplayLevel_0()
    {
        SetDisplayLevel(DisplayLevel.None);
    }

    [MenuItem(_menuItem_level1)]
    static void SetDisplayLevel_1()
    {
        SetDisplayLevel(DisplayLevel.MonoBehavioursOnly);
    }

    [MenuItem(_menuItem_level2)]
    static void SetDisplayLevel_2()
    {
        SetDisplayLevel(DisplayLevel.BehavioursOnly);
    }

    [MenuItem(_menuItem_level3)]
    static void SetDisplayLevel_3()
    {
        SetDisplayLevel(DisplayLevel.AllComponents);
    }

    [MenuItem(_menuItem_overlap)]
    static void ToggleOverlapping()
    {
        SetOverlapping(!_overlapping);
    }

	[MenuItem(_menuItem_static)]
	static void ToggleShowStatic()
	{
		SetShowStatic(!_showStatic);
	}

	[MenuItem(_menuItem_bakedLights)]
	static void ToggleShowBakedLights()
	{
		SetShowStatic(!_showBakedLights);
	}

	#endregion Menu Items

	#region Editor Prefs

	static void SetDisplayLevel(DisplayLevel displayLevel)
    {
        _displayLevel = displayLevel;

        EditorPrefs.SetString(_editorPref_displayLevel, _displayLevel.ToString());

        UpdateMenuItemCheckedStatuses();

        RepaintAllHierarchyWindows();
    }

    static DisplayLevel GetDisplayLevel()
    {
        var displayLevelSettingString = EditorPrefs.GetString(_editorPref_displayLevel, _defaultDisplayLevel.ToString());

		if (Enum.TryParse<DisplayLevel>(displayLevelSettingString, true, out DisplayLevel displayLevel))
		{
			_displayLevel = displayLevel;
		}
		else
		{
			SetDisplayLevel(_defaultDisplayLevel);
		}

		return _displayLevel;
    }

    static void SetOverlapping(bool newState)
    {
        _overlapping = newState;
		SetEditorPref(_editorPref_overlapping, newState);
    }

    static bool GetOverlapping()
    {
        var value = EditorPrefs.GetBool(_editorPref_overlapping, true);
        
        _overlapping = value;
        
        return value;
    }

	static void SetShowStatic(bool newState)
	{
		_showStatic = newState;
		SetEditorPref(_editorPref_static, newState);
	}

	static bool GetShowStatic()
	{
		var value = EditorPrefs.GetBool(_editorPref_static, true);

		_showStatic = value;

		return value;
	}

	static void SetShowBakedLights(bool newState)
	{
		_showBakedLights = newState;
		SetEditorPref(_editorPref_bakedLights, newState);
	}

	static bool GetShowBakedLights()
	{
		var value = EditorPrefs.GetBool(_editorPref_bakedLights, false);

		_showBakedLights = value;

		return value;
	}

	static void SetEditorPref(string key, bool value)
	{
		EditorPrefs.SetBool(key, value);

		UpdateMenuItemCheckedStatuses();

		RepaintAllHierarchyWindows();
	}

    static void RepaintAllHierarchyWindows()
    {
        EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
        EditorWindow[] hierarchyWindows = allWindows.Where(w => w.titleContent.text == "Hierarchy").ToArray();

        foreach (var hierarchyWindow in hierarchyWindows)
        { 
            hierarchyWindow.Repaint(); 
        }
    }

    #endregion Editor Pref & Menu Items
}
