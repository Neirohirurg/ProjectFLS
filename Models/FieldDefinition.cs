using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFLS.Models
{
    public enum FieldType
    {
        TextBox,
        ComboBox
    }

    public class FieldDefinition
    {
        public string Name { get; set; }              // Внутреннее имя поля
        public string Label { get; set; }             // Отображаемая подпись
        public FieldType Type { get; set; }           // Тип: TextBox или ComboBox
        public IEnumerable<string> Options { get; set; }  // Для ComboBox, если нужно

        public FieldDefinition(string name, string label, FieldType type, IEnumerable<string> options = null)
        {
            Name = name;
            Label = label;
            Type = type;
            Options = options;
        }
    }

}
