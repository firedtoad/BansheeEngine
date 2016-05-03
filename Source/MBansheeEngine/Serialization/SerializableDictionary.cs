﻿//********************************** Banshee Engine (www.banshee3d.com) **************************************************//
//**************** Copyright (c) 2016 Marko Pintera (marko.pintera@gmail.com). All rights reserved. **********************//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BansheeEngine
{
    /** @addtogroup Serialization
     *  @{
     */

    #pragma warning disable 649

    /// <summary>
    /// Allows you to access meta-data about a managed dictionary and its children. Similar to Reflection but simpler and 
    /// faster.
    /// </summary>
    public sealed class SerializableDictionary : ScriptObject
    {
        private SerializableProperty.FieldType keyPropertyType;
        private SerializableProperty.FieldType valuePropertyType;
        private Type keyType;
        private Type valueType;
        private SerializableProperty parentProperty;

        /// <summary>
        /// Type of serializable property used for the keys stored in the dictionary.
        /// </summary>
        public SerializableProperty.FieldType KeyPropertyType
        {
            get { return keyPropertyType; }
        }

        /// <summary>
        /// Type of serializable property used for the values stored in the dictionary.
        /// </summary>
        public SerializableProperty.FieldType ValuePropertyType
        {
            get { return valuePropertyType; }
        }

        /// <summary>
        /// Type used for of keys stored in the dictionary.
        /// </summary>
        public Type KeyType
        {
            get { return keyType; }
        }

        /// <summary>
        /// Type used for values stored in the dictionary.
        /// </summary>
        public Type ValueType
        {
            get { return valueType; }
        }

        /// <summary>
        /// Constructor for use by the runtime only.
        /// </summary>
        /// <param name="keyType">C# type of the keys in the dictionary.</param>
        /// <param name="valueType">C# type of the values in the dictionary.</param>
        /// <param name="parentProperty">Property used for retrieving this entry.</param>
        private SerializableDictionary(Type keyType, Type valueType, SerializableProperty parentProperty)
        {
            this.parentProperty = parentProperty;
            this.keyType = keyType;
            this.valueType = valueType;
            keyPropertyType = SerializableProperty.DetermineFieldType(keyType);
            valuePropertyType = SerializableProperty.DetermineFieldType(valueType);
        }

        /// <summary>
        /// Returns a serializable property for the specified entry.
        /// </summary>
        /// <param name="key">Dictionary key for the value to retrieve.</param>
        /// <returns>Serializable property that allows you to manipulate contents of the dictionary entry.</returns>
        public KeyValuePair<SerializableProperty, SerializableProperty> GetProperty(object key)
        {
            IDictionary dictionary = parentProperty.GetValue<IDictionary>();

            if (dictionary == null || !dictionary.Contains(key))
                return new KeyValuePair<SerializableProperty, SerializableProperty>(null, null);

            SerializableProperty keyProperty;
            {
                SerializableProperty.Getter getter = () => key;
                SerializableProperty.Setter setter = (object value) => {};

                keyProperty = Internal_CreateKeyProperty(mCachedPtr);
                keyProperty.Construct(KeyPropertyType, keyType, getter, setter);
            }

            SerializableProperty valueProperty;
            {
                SerializableProperty.Getter getter = () =>
                {
                    IDictionary dict = parentProperty.GetValue<IDictionary>();

                    if (dict != null)
                        return dict[key];
                    else
                        return null;
                };

                SerializableProperty.Setter setter = (object value) =>
                {
                    IDictionary dict = parentProperty.GetValue<IDictionary>();

                    if (dict != null)
                        dict[key] = value;
                };

                valueProperty = Internal_CreateValueProperty(mCachedPtr);
                valueProperty.Construct(ValuePropertyType, valueType, getter, setter);
            }

            return new KeyValuePair<SerializableProperty, SerializableProperty>(keyProperty, valueProperty);
        }

        /// <summary>
        /// Returns the total number of elements in the dictionary.
        /// </summary>
        /// <returns>Total number of elements in the dictionary.</returns>
        public int GetLength()
        {
            IDictionary dictionary = parentProperty.GetValue<IDictionary>();

            if (dictionary != null)
                return dictionary.Count;
            else
                return 0;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern SerializableProperty Internal_CreateKeyProperty(IntPtr nativeInstance);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern SerializableProperty Internal_CreateValueProperty(IntPtr nativeInstance);
    }

    /** @} */
}