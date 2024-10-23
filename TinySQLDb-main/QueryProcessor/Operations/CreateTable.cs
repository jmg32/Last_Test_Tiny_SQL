using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute(string sentence)
        {
            // Extraemos el nombre de la tabla y las columnas de la sentencia SQL
            var tableName = ExtractTableName(sentence);
            var columns = ExtractColumns(sentence);

            // Llamamos al método CreateTable del Store con el nombre de la tabla y las columnas
            return Store.GetInstance().CreateTable(tableName, columns);
        }

        // Método para extraer el nombre de la tabla de la sentencia SQL
        private string ExtractTableName(string sentence)
        {
            // Lógica para extraer el nombre de la tabla
            // Ejemplo: "CREATE TABLE Estudiante" -> devuelve "Estudiante"
            var tokens = sentence.Split();
            return tokens[2]; // El nombre de la tabla debería estar en la tercera posición
        }

        // Método para extraer las columnas y sus tipos de la sentencia SQL
        private Dictionary<string, string> ExtractColumns(string sentence)
        {
            var columns = new Dictionary<string, string>();

            // Encontramos la primera aparición de '(' y la última de ')'
            int startIndex = sentence.IndexOf('(');
            int endIndex = sentence.LastIndexOf(')'); // Cambiar a LastIndexOf para asegurar que encontramos el cierre correcto

            // Verificamos si los paréntesis fueron encontrados y que estén en el orden correcto
            if (startIndex == -1 || endIndex == -1 || startIndex >= endIndex)
            {
                throw new ArgumentException("La sentencia SQL no tiene un formato válido de columnas.");
            }

            // Extraemos la definición de columnas entre los paréntesis
            var columnsDefinition = sentence.Substring(startIndex + 1, endIndex - startIndex - 1);

            // Dividimos las definiciones de las columnas
            var columnsArray = columnsDefinition.Split(',');

            // Procesamos cada columna y tipo
            foreach (var column in columnsArray)
            {
                var parts = column.Trim().Split(' ');
                if (parts.Length != 2)
                {
                    throw new ArgumentException("Formato incorrecto en la definición de la columna.");
                }

                var columnName = parts[0];
                var columnType = parts[1];
                columns.Add(columnName, columnType);
            }

            return columns;
        }



    }
}
