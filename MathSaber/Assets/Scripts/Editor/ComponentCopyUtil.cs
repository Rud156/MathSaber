namespace EditorUtils
{
#if UNITY_EDITOR
    using UnityEditor;

    public static class ComponentCopyUtil
    {
        static SerializedObject source;

        [MenuItem("CONTEXT/Component/Copy Serialized")]
        public static void CopySerializedFromBase(MenuCommand command) => source = new SerializedObject(command.context);

        [MenuItem("CONTEXT/Component/Paste Serialized")]
        public static void PasteSerializedFromBase(MenuCommand command)
        {
            SerializedObject dest = new SerializedObject(command.context);
            SerializedProperty prop_iterator = source.GetIterator();

            //jump into serialized object, this will skip script type so that we dont override the destination component's type
            if (prop_iterator.NextVisible(true))
            {
                while (prop_iterator.NextVisible(true)) //itterate through all serializedProperties
                {
                    //try obtaining the property in destination component
                    SerializedProperty prop_element = dest.FindProperty(prop_iterator.name);

                    //validate that the properties are present in both components, and that they're the same type
                    if (prop_element != null && prop_element.propertyType == prop_iterator.propertyType)
                    {
                        //copy value from source to destination component
                        dest.CopyFromSerializedProperty(prop_iterator);
                    }
                }
            }

            dest.ApplyModifiedProperties();
        }
    }
#endif
}