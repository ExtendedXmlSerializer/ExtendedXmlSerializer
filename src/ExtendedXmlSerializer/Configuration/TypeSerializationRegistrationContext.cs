﻿using System;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.Configuration {
	/// <summary>
	/// Used to configure the serializer that is used to write this object to the output document during the serialization
	/// process, and to read from the provided document during the deserialization process.
	/// </summary>
	/// <typeparam name="T">The type under configuration.</typeparam>
	public sealed class TypeSerializationRegistrationContext<T>
	{
		readonly ITypeConfiguration<T> _configuration;

		public TypeSerializationRegistrationContext(ITypeConfiguration<T> configuration)
			=> _configuration = configuration;

		/// <summary>
		/// Used to activate the specified type.  Doing so will allow you to design your serializer to import dependencies into its constructor.
		/// </summary>
		/// <typeparam name="TSerializer">The serializer type to activate.</typeparam>
		/// <returns>The configured type configuration.</returns>
		public ITypeConfiguration<T> Of<TSerializer>() where TSerializer : ISerializer<T>
			=> Of(Support<TSerializer>.Key);

		/// <summary>
		/// Used to activate the specified type.  Doing so will allow you to design your serializer to import dependencies into its constructor.
		/// </summary>
		/// <returns>The configured type configuration.</returns>
		public ITypeConfiguration<T> Of(Type serializerType)
			=> Using(new ActivatedSerializer(serializerType, Support<T>.Metadata));

		/// <summary>
		/// Provides an instance of a serializer to register as this type's serializer.
		/// </summary>
		/// <param name="serializer">The serializer to use to serialize/deserialize instances of this type.</param>
		/// <returns>The configured type configuration.</returns>
		public ITypeConfiguration<T> Using(ISerializer<T> serializer) => Using(serializer.Adapt());

		/// <summary>
		/// Provides an instance of a serializer to register as this type's serializer.
		/// </summary>
		/// <param name="serializer">The serializer to use to serialize/deserialize instances of this type.</param>
		/// <returns>The configured type configuration.</returns>
		public ITypeConfiguration<T> Using(ISerializer serializer)
			=> _configuration.Root.With<CustomSerializationExtension>()
			                 .Types.Apply(_configuration.Get(), serializer)
			                 .Return(_configuration);

		/// <summary>
		/// Clears any serializer that is registered with this type.  This will result in the type to use the default
		/// serialization/deserialization mechanisms of the root serializer.
		/// </summary>
		/// <returns>The configuration type configuration.</returns>
		public ITypeConfiguration<T> None() => _configuration.Root.With<CustomSerializationExtension>()
		                                                     .Types.Remove(_configuration.Get())
		                                                     .Return(_configuration);
	}
}