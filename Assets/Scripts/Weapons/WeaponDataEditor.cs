using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    WeaponData weaponData;
    string[] weaponSubtypes;
    int selectedWeaponSubtyspe;

    void OnEnable()
    {
        // Cache the weapon data value
        weaponData = (WeaponData)target;

        // Retrieve all the weapon subtypes and cache it
        System.Type baseType = typeof(Weapon);
        List<System.Type> subTypes = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
        .ToList();

        // Add a none option in front
        List<string> subTypesString = subTypes.Select(t => t.Name).ToList();
        subTypesString.Insert(0, "None");
        weaponSubtypes = subTypesString.ToArray();

        // Ensure that we are using the correct weapon subtype
        selectedWeaponSubtyspe = Math.Max(0, Array.IndexOf(weaponSubtypes, weaponData.behavior));
    }

    public override void OnInspectorGUI()
    {
        // Draw a dropdown in the Inspector
        selectedWeaponSubtyspe = EditorGUILayout.Popup("Behavior", Math.Max(0, selectedWeaponSubtyspe), weaponSubtypes);

        if (selectedWeaponSubtyspe > 0)
        {
            // Update the behaviour field
            weaponData.behavior = weaponSubtypes[selectedWeaponSubtyspe];
            EditorUtility.SetDirty(weaponData); // Marks the object to save
            DrawDefaultInspector(); // Draw the default inspector elements
        }
    }
}