﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VideoOnDemand.Data.Data;

namespace VideoOnDemand.Data.Services
{
    public class DbReadService : IDbReadService
    {
        private VODContext _db;
        public DbReadService(VODContext db)
        {
            _db = db;
        }
        public IQueryable<TEntity> Get<TEntity>() where TEntity : class
        {
            return _db.Set<TEntity>();
        }
        private (IEnumerable<string> collections, IEnumerable<string>references) GetEntityNames<TEntity>() where TEntity : class
        {
            var dbsets = typeof(VODContext)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(z => z.PropertyType.Name.Contains("DbSet"))
            .Select(z => z.Name);
            // Get the names of all the properties (tables) in the generic
            // type T that is represented by a DbSet
            var properties = typeof(TEntity)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var collections = properties
            .Where(l => dbsets.Contains(l.Name))
            .Select(s => s.Name);
            var classes = properties
            .Where(c => dbsets.Contains(c.Name + "s"))
            .Select(s => s.Name);
            return (collections: collections, references: classes);
        }
        public TEntity Get<TEntity>(int id, bool includeRelatedEntities = false) where TEntity : class
        {
            var record = _db.Set<TEntity>().Find(new object[] { id });

            if (record != null && includeRelatedEntities)
            {
                var entities = GetEntityNames<TEntity>();
                foreach (var entity in entities.collections)
                    _db.Entry(record).Collection(entity).Load();

                foreach (var entity in entities.references)
                    _db.Entry(record).Reference(entity).Load();
            }

            return record;
        }
        public TEntity Get<TEntity>(string userId, int id) where TEntity : class
        {
            var record = _db.Set<TEntity>().Find(new object[] { userId, id });
            return record;
        }
        public IEnumerable<TEntity> GetWithIncludes<TEntity>() where TEntity : class
        {
            var entityNames = GetEntityNames<TEntity>();
            var dbset = _db.Set<TEntity>();

            var entities = entityNames.collections.Union(entityNames.references);

            foreach (var entity in entities)
                _db.Set<TEntity>().Include(entity).Load();

            return dbset;
        }
        public SelectList GetSelectList<TEntity>(string valueField, string textField) where TEntity : class
        {
            var items = Get<TEntity>();
            return new SelectList(items, valueField, textField);
        }
        public (int courses, int downloads, int instructors, int modules, int videos, int users, int userCourses) Count()
        {
            return (
                courses: _db.Courses.Count(),
                downloads: _db.Downloads.Count(),
                instructors: _db.Instructors.Count(),
                modules: _db.Modules.Count(),
                videos: _db.Videos.Count(),
                users: _db.Users.Count(),
                userCourses: _db.UserCourses.Count());
        }
    }
}
