using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace QueryProcessor.Operations
{
    internal class Insert
    {
        internal OperationStatus Execute(string sentence)
        {
            // Extraer el nombre de la tabla y los valores a insertar
            var tableName = ExtractTableName(sentence);
            var values = ExtractValues(sentence);

            // Llamar al método Insert del Store para agregar los datos a la tabla
            return Store.GetInstance().InsertIntoTable(tableName, values);
        }

        // Método para extraer el nombre de la tabla de la sentencia SQL
        private string ExtractTableName(string sentence)
        {
            // Ejemplo de sentencia: "INSERT INTO Estudiante ..."
            var tokens = sentence.Split();
            return tokens[2]; // El nombre de la tabla está en la tercera posición
        }

        // Método para extraer los valores de la sentencia SQL
        private List<string> ExtractValues(string sentence)
        {
            var values = new List<string>();

            // Extraer los valores dentro del paréntesis
            int startIndex = sentence.IndexOf('(');
            int endIndex = sentence.IndexOf(')');
            if (startIndex == -1 || endIndex == -1 || startIndex >= endIndex)
            {
                throw new ArgumentException("La sentencia SQL no tiene un formato válido de valores.");
            }

            // Extraer los valores entre los paréntesis y dividirlos por comas
            var valuesString = sentence.Substring(startIndex + 1, endIndex - startIndex - 1);
            var valuesArray = valuesString.Split(',');

            // Procesar y limpiar cada valor, incluyendo manejo de comillas
            foreach (var value in valuesArray)
            {
                var trimmedValue = value.Trim();

                // Si el valor está entre comillas simples, las removemos
                if (trimmedValue.StartsWith("'") && trimmedValue.EndsWith("'"))
                {
                    trimmedValue = trimmedValue.Substring(1, trimmedValue.Length - 2);
                }

                values.Add(trimmedValue);
            }

            return values;
        }

    }
}
