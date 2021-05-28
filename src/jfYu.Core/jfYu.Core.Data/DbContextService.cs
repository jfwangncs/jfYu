using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace jfYu.Core.Data
{

    public class DbContextService<T> : IDbContextService<T> where T : DbContext
    {

        /// <summary>
        /// 主数据库
        /// </summary>
        public T Master { get; }

        /// <summary>
        /// 从数据库
        /// </summary>
        public T Slave { get; }

        /// <summary>
        /// 从数据库集
        /// </summary>
        public List<T> Slaves { get; }

        public DbContextService(T master, List<T> salves, DatabaseConfiguration configuration)
        {
            Master = master;

            Slaves = salves;

            Random r = new Random();

            if (salves.Count <= 0)
            {
                Slave = Master;
            }
            else if (salves.Count == 1)
            {
                Slave = salves[0];
            }
            else
            {
                var num = r.Next(0, salves.Count);
                Slave = salves[num];
                if (configuration.SlaveCheck)
                {
                    var end = num;
                    var front = num;
                    try
                    {
                        //正向查找
                        while (Slave == null || !Slave.Database.CanConnect())
                        {
                            Slave = null;
                            end += 1;
                            if (end >= salves.Count)
                                break;
                            else
                                Slave = salves[end];
                        }
                        while (Slave == null || !Slave.Database.CanConnect())
                        {
                            Slave = null;
                            front -= 1;
                            if (front < 1)
                                break;
                            else
                                Slave = salves[front];
                        }
                        if (Slave == null)
                            Slave = Master;
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }
}
