using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using StoreDataManager;

namespace QueryProcessor
{
    public class SQLQueryProcessor
    {
        public static OperationStatus Execute(string sentence)
        {
            if (sentence.StartsWith("CREATE TABLE"))
            {
                return new CreateTable().Execute(sentence);
            }

            if (sentence.StartsWith("SELECT"))
            {
                return new Select().Execute();
            }

            if (sentence.StartsWith("INSERT INTO"))
            {
                return new Insert().Execute(sentence);
            }

            // Agregamos soporte para UPDATE
            if (sentence.StartsWith("UPDATE"))
            {
                return new Update().Execute(sentence);
            }

            // Agregamos soporte para DELETE
            if (sentence.StartsWith("DELETE FROM"))
            {
                return new Delete().Execute(sentence);
            }

            throw new UnknownSQLSentenceException();
        }
    }


}
