﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;

namespace DAL.Repositories
{
    public class SysUserRepository : ISysUserRepository
    {
        protected DataWallContext db = new DataWallContext();

        //查询所有用户
        public IQueryable<SysUser> SelectAll()
        {
            using (DataWallContext db = new DataWallContext())
            {
                return db.SysUsers;
            }
        }

        //通过用户名查询用户
        public SysUser SelectByName(string userName)
        {
            using (DataWallContext db = new DataWallContext())
            {
                return db.SysUsers.FirstOrDefault(u => u.UserName == userName);
            }
        }

        //登陆验证
        public SysUser SelectByNamePs(string userName, string passWord)
        {
            using (DataWallContext db = new DataWallContext())
            {
                return db.SysUsers.FirstOrDefault(u => (u.UserName == userName || u.Email == userName) & u.Password == passWord);
            }
        }

        //添加用户
        public void Add(SysUser sysUser)
        {
            using (DataWallContext db = new DataWallContext())
            {
                db.SysUsers.Add(sysUser);
                db.SaveChanges();
            }
        }

        //删除用户
        public bool Delete(int id)
        {
            using (DataWallContext db = new DataWallContext())
            {
                var delSysUser = db.SysUsers.FirstOrDefault(u => u.ID == id);
                if (delSysUser != null)
                {
                    db.SysUsers.Remove(delSysUser);
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}