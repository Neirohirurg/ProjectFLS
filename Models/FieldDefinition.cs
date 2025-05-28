using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFLS.Models
{
    public enum FieldType
    {
        Text,
        Password,
        ComboBox,
        CheckBox
    }

    public class FieldDefinition
    {
        public string Name { get; set; }                  // Имя поля
        public string DisplayName { get; set; }           // Название для Label
        public FieldType FieldType { get; set; }          // Тип поля
        public object Value { get; set; }                 // Текущее значение
        public IEnumerable<object> Options { get; set; }  // Для ComboBox
    }

}
