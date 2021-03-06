﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sukt.Core.Domain.ISuktBaseRepository;
using Sukt.Core.DomainRealization.Base;
using Sukt.Core.EntityFrameworkCore;
using Sukt.Core.Shared;
using Sukt.Core.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sukt.Core.API.Startups
{
    public class EntityFrameworkCoreMySqlModule : EntityFrameworkCoreModuleBase
    {
        /// <summary>
        /// 添加仓储
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        protected override IServiceCollection AddRepository(IServiceCollection services)
        {
            return services.AddScoped(typeof(IEFCoreRepository<,>), typeof(BaseRepository<,>));
        }
        /// <summary>
        /// 添加工作单元
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        protected override IServiceCollection AddUnitOfWork(IServiceCollection services)
        {
            return services.AddScoped<IUnitOfWork, UnitOfWork<DefaultDbContext>>();
        }
        /// <summary>
        /// 重写方法
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        protected override IServiceCollection UseSql(IServiceCollection services)
        {
            var Dbpath = services.GetConfiguration()["SuktCore:DbContext:MysqlConnectionString"];
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath; //获取项目路径
            var dbcontext = Path.Combine(basePath, Dbpath);
            var Assembly = typeof(EntityFrameworkCoreMySqlModule).GetTypeInfo().Assembly.GetName().Name;//获取程序集
            if (!File.Exists(dbcontext))
            {
                throw new Exception("未找到存放数据库链接的文件");
            }
            var mysqlconn = File.ReadAllText(dbcontext).Trim(); ;
            services.AddDbContext<DefaultDbContext>(option => {
                option.UseMySql(mysqlconn, assembly => { assembly.MigrationsAssembly("Sukt.Core.Domain"); });
            });
            return services;
        }
    }
}
