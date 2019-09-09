﻿using LootEditor.Model;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Markup;

namespace LootEditor.View
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        private class EnumComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var converter = TypeDescriptor.GetConverter(x.GetType());
                return StringComparer.OrdinalIgnoreCase.Compare(converter.ConvertToInvariantString(x), converter.ConvertToInvariantString(y));
            }

        }
        private Type _enumType;
        public Type EnumType
        {
            get { return this._enumType; }
            set
            {
                if (value != this._enumType)
                {
                    if (null != value)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;

                        if (!enumType.IsEnum)
                            throw new ArgumentException("Type must be for an Enum.");
                    }

                    this._enumType = value;
                }
            }
        }

        public EnumBindingSourceExtension() { }

        public EnumBindingSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == this._enumType)
                throw new InvalidOperationException("The EnumType must be specified.");

            Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
            Array enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == this._enumType)
            {
                Array.Sort(enumValues, new EnumComparer());
                return enumValues;
            }

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            Array.Sort(tempArray, new EnumComparer());
            return tempArray;
        }
    }
}
