using Entities;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace StoreDataManager
{
    public sealed class Store
    {
        private static Store? instance = null;
        private static readonly object _lock = new object();
               
        public static Store GetInstance()
        {
            lock(_lock)
            {
                if (instance == null) 
                {
                    instance = new Store();
                }
                return instance;
            }
        }

        private const string DatabaseBasePath = @"C:\TinySql\";
        private const string DataPath = $@"{DatabaseBasePath}\Data";
        private const string SystemCatalogPath = $@"{DataPath}\SystemCatalog";
        private const string SystemDatabasesFile = $@"{SystemCatalogPath}\SystemDatabases.table";
        private const string SystemTablesFile = $@"{SystemCatalogPath}\SystemTables.table";

        public Store()
        {
            this.InitializeSystemCatalog();
            
        }

        private void InitializeSystemCatalog()
        {
            // Always make sure that the system catalog and above folder
            // exist when initializing
            Directory.CreateDirectory(SystemCatalogPath);
        }

        public OperationStatus CreateTable(string tableName, Dictionary<string, string> columns)
        {
            try
            {
                // Verificamos si la base de datos existe, de lo contrario, la creamos
                Directory.CreateDirectory($@"{DataPath}\TESTDB");

                // Definimos la ruta para la nueva tabla
                var tablePath = $@"{DataPath}\TESTDB\{tableName}.table";

                // Creamos el archivo de la tabla
                using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    // Escribimos el esquema de la tabla en el archivo
                    writer.Write(columns.Count); // Guardamos el número de columnas
                    foreach (var column in columns)
                    {
                        writer.Write(column.Key); // Nombre de la columna
                        writer.Write(column.Value); // Tipo de la columna (por ejemplo, INT, VARCHAR)
                    }
                }

                return OperationStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al crear la tabla: " + ex.Message);
                return OperationStatus.Error;
            }
        }

        public OperationStatus UpdateTable(string tableName, Dictionary<string, string> setValues, string condition)
        {
            try
            {
                var tablePath = $@"{DataPath}\TESTDB\{tableName}.table";

                if (!File.Exists(tablePath))
                {
                    Console.WriteLine($"La tabla {tableName} no existe.");
                    return OperationStatus.Error;
                }

                // Leemos la tabla, actualizamos los registros, y escribimos los cambios de vuelta
                var records = new List<Dictionary<string, string>>();

                using (FileStream stream = File.Open(tablePath, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (stream.Position < stream.Length)
                    {
                        var record = new Dictionary<string, string>();

                        // Leer los datos del archivo (esto depende de la estructura de tus datos)
                        var id = reader.ReadString(); // Ejemplo de lectura
                        var nombre = reader.ReadString();

                        // Agregar datos al diccionario
                        record.Add("ID", id);
                        record.Add("Nombre", nombre);

                        records.Add(record);
                    }
                }

                // Actualizamos los registros que cumplen la condición
                foreach (var record in records)
                {
                    // Esta es una simple evaluación de condición (puedes mejorar esto para manejar más condiciones)
                    if (record["ID"] == condition) // Si la condición es el ID, actualizamos
                    {
                        foreach (var setValue in setValues)
                        {
                            record[setValue.Key] = setValue.Value; // Actualizamos el campo correspondiente
                        }
                    }
                }

                // Volvemos a escribir los registros actualizados
                using (FileStream stream = File.Open(tablePath, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    foreach (var record in records)
                    {
                        writer.Write(record["ID"]);
                        writer.Write(record["Nombre"]);
                    }
                }

                return OperationStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar la tabla: " + ex.Message);
                return OperationStatus.Error;
            }
        }

        public OperationStatus InsertIntoTable(string tableName, List<string> values)
        {
            try
            {
                // Ruta de la tabla en la que vamos a insertar
                var tablePath = $@"{DataPath}\TESTDB\{tableName}.table";

                // Verificamos que la tabla exista
                if (!File.Exists(tablePath))
                {
                    Console.WriteLine($"La tabla {tableName} no existe.");
                    return OperationStatus.Error;
                }

                // Abrimos la tabla en modo añadir para insertar los nuevos datos
                using (FileStream stream = File.Open(tablePath, FileMode.Append))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    // Escribimos los valores en la tabla, cada uno en el formato adecuado
                    foreach (var value in values)
                    {
                        writer.Write(value);
                    }
                }

                return OperationStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar en la tabla: " + ex.Message);
                return OperationStatus.Error;
            }
        }


        public OperationStatus Select()
        {
            // Creates a default Table called ESTUDIANTES
            var tablePath = $@"{DataPath}\TESTDB\ESTUDIANTES.Table";
            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryReader reader = new (stream))
            {
                // Print the values as a I know exactly the types, but this needs to be done right
                Console.WriteLine(reader.ReadInt32());
                Console.WriteLine(reader.ReadString());
                Console.WriteLine(reader.ReadString());
                return OperationStatus.Success;
            }
        }
    }
}
