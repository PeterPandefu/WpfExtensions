﻿using System;
using System.Windows.Data;

namespace WpfExtensions.Xaml.Markup
{
    public class MultiBindingWithoutConverter : MultiBinding
    {
        public new IMultiValueConverter Converter
        {
            get => base.Converter;
            protected set
            {
                if (base.Converter != null)
                {
                    throw new InvalidOperationException($"The {GetType().Name}.Converter property is readonly.");
                }

                base.Converter = value;
            }
        }
    }
}
