using jfYu.Core.Office.Excel.Write.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace jfYu.Core.Office.Excel.Write.Implementation
{
    public class JfYuExcelWriterFactory(IServiceProvider serviceProvider) : IJfYuExcelWriterFactory
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public IJfYuExcelWrite<T> GetWriter<T>()
        {
            return _serviceProvider.GetRequiredService<IJfYuExcelWrite<T>>();          
        }
    }
}
