using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Model;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Migrations;

namespace DAL
{
    public class DataWallContext : DbContext
    {
        public DataWallContext() : base("DataWallContext")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //因为表名称默认为复数形式，这里是移除复数形式，所以为单数形式生成
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        /// <summary>
        /// 用户
        /// </summary>
        public DbSet<SysUser> SysUsers { get; set; }

        /// <summary>
        /// 总分馆
        /// </summary>
        public DbSet<SysLibrary> SysLibrarys { get; set; }
        public DbSet<SysLibraryUser> SysLibraryUsers { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public DbSet<SysRole> SysRoles { get; set; }
        public DbSet<SysUserRole> SysUserRoles { get; set; }
        public DbSet<SysActionRole> SysActionRoles { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public DbSet<SysContent> SysContents { get; set; }
        public DbSet<SysFile> SysFiles { get; set; }

        /// <summary>
        /// 新书推荐
        /// </summary>
        public DbSet<SysNewBook> SysNewBooks { get; set; }

        /// <summary>
        /// 额外数据
        /// </summary>
        public DbSet<SysAdditionalData> SysAdditionalDatas { get; set; }

        /// <summary>
        /// 系统信息
        /// </summary>
        public DbSet<SysProgramInfo> SysProgramInfos { get; set; }

        /// <summary>
        /// 客流设备
        /// </summary>
        public DbSet<SysCustDevice> SysCustDevices { get; set; }

        /// <summary>
        /// 客流数据
        /// </summary>
        public DbSet<SysCustData> SysCustDatas { get; set; }

        /// <summary>
        /// 历史上的今天
        /// </summary>
        public DbSet<History> Historys { get; set; }

        /// <summary>
        /// 系统菜单
        /// </summary>
        public DbSet<SysMenu> SysMenus { get; set; }

        /// <summary>
        /// 模块
        /// </summary>
        public DbSet<SysModule> SysModules { get; set; }

        /// <summary>
        /// 主题设置
        /// </summary>
        public DbSet<SysThemeSet> SysThemeSets { get; set; }

        /// <summary>
        /// 系统日志
        /// </summary>
        public DbSet<SysLog> SysLogs { get; set; }
    }

}