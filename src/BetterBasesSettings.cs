using System.IO;
using System.Reflection;
using ModSettings;
using System;

namespace BetterBases
{
    class BetterBasesSettings : JsonModSettings
    {
        [Section("Mod Features")]

        [Name("Disable clutter removal")]
        [Description("Disables the ability to remove clutter from scenes, will improve load times specially in big scenes. WARNING: all removed clutter will be back.")]
        public bool disableRemoveClutter = false;

        [Name("Disable clutter removal outdoors")]
        [Description("Enables the clutter removal outdoors. WARNING: this will increase load times DRASTICALLY. ON by default.")]
        public bool disableOutsideClutter = true;

        [Name("Disable all items movement")]
        [Description("Disables moving of all items. WARNING: will reset the position of these items. To see the effect, scene must be reloaded.")]
        public bool disableItemMove = false;

        [Name("Disable small items movement")]
        [Description("Disables moving of small items, can increase performance on some scenes. WARNING: will reset the position of these items. To see the effect, scene must be reloaded.")]
        public bool disableSmallItems = false;

        [Name("Disable repairing")]
        [Description("Disables the repairing of cabinet doors and drawers. WARNING: Repairs already made will be reset.")]
        public bool disableRepairs = false;

        [Section("Clutter Options")]

        [Name("Tools required")]
        [Description("If set to NO, clutter won't need any tools to be removed.")]
        public bool toolsNeeded = true;

        [Name("Fast breakdown")]
        [Description("If active, clutter will just take 1 minute to remove.")]
        public bool fastBreakDown = false;

        [Name("Objects yield")]
        [Description("If set to NO, clutter will not yield any objects when harvested.")]
        public bool objectYields = true;

        [Name("Show object names (no translation)")]
        [Description("If deactivated, mod will show a generic localized string, if activated, it will show the original object name without translation.")]
        public bool showObjectNames = true;

        [Section("Furniture Placement")]

        [Name("Remove clutter automatically")]
        [Description("If activated, clutter will be removed from moveable furniture automatically. If not activated, clutter will have to be removed manually before moving the furniture.")]
        public bool autoDeclutter = false;

        [Name("Drop objects when picking up furniture")]
        [Description("If activated, objects that are over the furniture will fall to the ground when picking it.")]
        public bool dropOnPickup = false;

        [Name("Verbose Log")]
        [Description("Makes the mod print more message to the logs. Useful for troubleshooting.")]
        public bool verbose = false;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == nameof(disableRemoveClutter) || field.Name == nameof(disableItemMove))
            {
                RefreshFields();
            }
        }

        internal void RefreshFields()
        {
            SetFieldVisible(nameof(disableOutsideClutter), !disableRemoveClutter);
            SetFieldVisible(nameof(disableSmallItems), !disableItemMove);
        }
    }

    internal static class Settings
    {
        public static BetterBasesSettings options;

        public static void OnLoad()
        {
            options = new BetterBasesSettings();
            options.RefreshFields();
            options.AddToModSettings("Better Bases Settings");
        }
    }
}
