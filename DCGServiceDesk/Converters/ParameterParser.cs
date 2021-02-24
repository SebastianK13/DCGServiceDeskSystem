using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace DCGServiceDesk.Converters
{
    public sealed class DoubleExtension : MarkupExtension
    {
        public DoubleExtension(double value) { this.Value = value; }
        public double Value { get; set; }
        public override Object ProvideValue(IServiceProvider sp) { return Value; }
    };
}
