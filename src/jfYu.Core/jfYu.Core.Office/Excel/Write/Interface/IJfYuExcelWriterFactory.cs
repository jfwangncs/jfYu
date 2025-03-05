namespace jfYu.Core.Office.Excel.Write.Interface
{     
    public interface IJfYuExcelWriterFactory
    {
        IJfYuExcelWrite<T> GetWriter<T>();
    }
}