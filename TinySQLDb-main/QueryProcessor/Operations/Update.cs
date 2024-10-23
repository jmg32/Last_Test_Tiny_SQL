using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Operations
{
    internal class Update
    {
        internal OperationStatus Execute(string sentence)
        {
            // Extraemos el nombre de la tabla y los valores a actualizar
            var tableName = ExtractTableName(sentence);
            var setValues = ExtractSetValues(sentence);
            var condition = ExtractCondition(sentence);

            // Llamamos al método Update del Store para actualizar los datos en la tabla
            return Store.GetInstance().UpdateTable(tableName, setValues, condition);
        }

        // Extraer el nombre de la tabla
        private string ExtractTableName(string sentence)
        {
            var tokens = sentence.Split();
            return tokens[1]; // El nombre de la tabla debería estar en la segunda posición
        }

        // Extraer los valores que se van a actualizar
        private Dictionary<string, string> ExtractSetValues(string sentence)
        {
            var values = new Dictionary<string, string>();
            var setIndex = sentence.IndexOf("SET");
            var whereIndex = sentence.IndexOf("WHERE");

            var setClause = sentence.Substring(setIndex + 4, whereIndex - setIndex - 4);
            var setItems = setClause.Split(',');

            foreach (var item in setItems)
            {
                var parts = item.Trim().Split('=');
                var column = parts[0].Trim();
                var value = parts[1].Trim().Replace("'", "");
                values.Add(column, value);
            }

            return values;
        }

        // Extraer la condición para el WHERE
        private string ExtractCondition(string sentence)
        {
            var whereIndex = sentence.IndexOf("WHERE");
            return sentence.Substring(whereIndex + 6).Trim(); // Tomamos la condición después de WHERE
        }
    }
}
