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
        /// 丛数据库
        /// </summary>
        public T Slave { get; }

        public List<T> Slaves { get; }

        public DbContextService(T Master, List<T> Salves, DatabaseConfiguration configuration)
        {
            this.Master = Master;

            this.Slaves = Salves;

            Random r = new Random();
            if (Salves.Count <= 0)
            {
                this.Slave = Master;
            }
            else if (Salves.Count == 1)
            {
                this.Slave = Salves[0];
            }
            else
            {
                var num = r.Next(0, Salves.Count);
                this.Slave = Salves[num];
                var end = num;
                var front = num;
                try
                {
                    //正向查找
                    while (this.Slave == null || !this.Slave.Database.CanConnect())
                    {
                        this.Slave = null;
                        end += 1;
                        if (end >= Salves.Count)
                            break;
                        else
                            this.Slave = Salves[end];
                    }
                    while (this.Slave == null || !this.Slave.Database.CanConnect())
                    {
                        this.Slave = null;
                        front -= 1;
                        if (front < 1)
                            break;
                        else
                            this.Slave = Salves[front];
                    }
                    if (this.Slave == null)
                        this.Slave = Master;
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
