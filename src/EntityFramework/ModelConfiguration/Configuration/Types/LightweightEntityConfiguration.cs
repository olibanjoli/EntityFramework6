﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.ModelConfiguration.Configuration.Types
{
    using System.Collections.Generic;
    using System.Data.Entity.Utilities;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     Allows configuration to be performed for an entity type in a model.
    ///     This configuration functionality is available via lightweight conventions.
    /// </summary>
    public class LightweightEntityConfiguration
    {
        private readonly Type _type;
        private readonly Func<EntityTypeConfiguration> _configuration;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LightweightEntityConfiguration" /> class.
        /// </summary>
        /// <param name="type">
        ///     The <see cref="Type" /> of this entity type.
        /// </param>
        /// <param name="configuration"> The configuration object that this instance wraps. </param>
        public LightweightEntityConfiguration(Type type, Func<EntityTypeConfiguration> configuration)
        {
            Check.NotNull(type, "type");
            Check.NotNull(configuration, "configuration");

            _type = type;
            _configuration = configuration;
        }

        /// <summary>
        ///     Gets the <see cref="Type" /> of this entity type.
        /// </summary>
        public Type ClrType
        {
            get { return _type; }
        }

        /// <summary>
        ///     Configures the entity set name to be used for this entity type.
        ///     The entity set name can only be configured for the base type in each set.
        /// </summary>
        /// <param name="entitySetName"> The name of the entity set. </param>
        /// <returns>
        ///     The same <see cref="LightweightEntityConfiguration" /> instance so that multiple calls can be chained.
        /// </returns>
        /// <remarks>
        ///     Calling this will have no effect once it has been configured.
        /// </remarks>
        public LightweightEntityConfiguration HasEntitySetName(string entitySetName)
        {
            Check.NotEmpty(entitySetName, "entitySetName");

            if (_configuration().EntitySetName == null)
            {
                _configuration().EntitySetName = entitySetName;
            }

            return this;
        }

        /// <summary>
        ///     Excludes a property from the model so that it will not be mapped to the database.
        /// </summary>
        /// <param name="propertyName"> The name of the property to be configured. </param>
        /// <remarks>
        ///     Calling this will have no effect if the property does not exist.
        /// </remarks>
        public void Ignore(string propertyName)
        {
            Check.NotEmpty(propertyName, "propertyName");

            Ignore(_type.GetProperty(propertyName));
        }

        /// <summary>
        ///     Excludes a property from the model so that it will not be mapped to the database.
        /// </summary>
        /// <param name="propertyInfo"> The property to be configured. </param>
        /// <remarks>
        ///     Calling this will have no effect if the property does not exist.
        /// </remarks>
        public void Ignore(PropertyInfo propertyInfo)
        {
            if (propertyInfo != null)
            {
                _configuration().Ignore(propertyInfo);
            }
        }

        /// <summary>
        ///     Configures the primary key property for this entity type.
        /// </summary>
        /// <param name="propertyName"> The name of the property to be used as the primary key. </param>
        /// <returns>
        ///     The same <see cref="LightweightEntityConfiguration" /> instance so that multiple calls can be chained.
        /// </returns>
        /// <remarks>
        ///     Calling this will have no effect once it has been configured of if the
        ///     property does not exist.
        /// </remarks>
        public LightweightEntityConfiguration HasKey(string propertyName)
        {
            Check.NotEmpty(propertyName, "propertyName");

            return HasKey(new[] { propertyName });
        }

        /// <summary>
        ///     Configures the primary key property for this entity type.
        /// </summary>
        /// <param name="propertyInfo"> The property to be used as the primary key. </param>
        /// <returns>
        ///     The same <see cref="LightweightEntityConfiguration" /> instance so that multiple calls can be chained.
        /// </returns>
        /// <remarks>
        ///     Calling this will have no effect once it has been configured of if the
        ///     property does not exist.
        /// </remarks>
        public LightweightEntityConfiguration HasKey(PropertyInfo propertyInfo)
        {
            return HasKey(new[] { propertyInfo });
        }

        /// <summary>
        ///     Configures the primary key property(s) for this entity type.
        /// </summary>
        /// <param name="propertyNames"> The names of the properties to be used as the primary key. </param>
        /// <returns>
        ///     The same <see cref="LightweightEntityConfiguration" /> instance so that multiple calls can be chained.
        /// </returns>
        /// <remarks>
        ///     Calling this will have no effect once it has been configured of if any
        ///     property does not exist.
        /// </remarks>
        public LightweightEntityConfiguration HasKey(IEnumerable<string> propertyNames)
        {
            Check.NotNull(propertyNames, "propertyNames");

            var propertyInfos = propertyNames
                .Select(n => _type.GetProperty(n))
                .ToArray();

            return HasKey(propertyInfos);
        }

        /// <summary>
        ///     Configures the primary key property(s) for this entity type.
        /// </summary>
        /// <param name="keyProperties"> The properties to be used as the primary key. </param>
        /// <returns>
        ///     The same <see cref="LightweightEntityConfiguration" /> instance so that multiple calls can be chained.
        /// </returns>
        /// <remarks>
        ///     Calling this will have no effect once it has been configured of if any
        ///     property does not exist.
        /// </remarks>
        public LightweightEntityConfiguration HasKey(IEnumerable<PropertyInfo> keyProperties)
        {
            Check.NotNull(keyProperties, "keyProperties");

            if (!_configuration().IsKeyConfigured
                && keyProperties.Any()
                && keyProperties.All(p => p != null))
            {
                _configuration().Key(keyProperties);
            }

            return this;
        }

        /// <summary>
        ///     Configures the table name that this entity type is mapped to.
        /// </summary>
        /// <param name="tableName"> The name of the table. </param>
        /// <remarks>
        ///     Calling this will have no effect once it has been configured.
        /// </remarks>
        public void ToTable(string tableName)
        {
            Check.NotEmpty(tableName, "tableName");

            if (!_configuration().IsTableNameConfigured)
            {
                var databaseName = DatabaseName.Parse(tableName);

                _configuration().ToTable(databaseName.Name, databaseName.Schema);
            }
        }

        /// <summary>
        ///     Configures the table name that this entity type is mapped to.
        /// </summary>
        /// <param name="tableName"> The name of the table. </param>
        /// <param name="schemaName"> The database schema of the table. </param>
        /// <remarks>
        ///     Calling this will have no effect once it has been configured.
        /// </remarks>
        public void ToTable(string tableName, string schemaName)
        {
            Check.NotEmpty(tableName, "tableName");

            if (!_configuration().IsTableNameConfigured)
            {
                _configuration().ToTable(tableName, schemaName);
            }
        }
    }
}
