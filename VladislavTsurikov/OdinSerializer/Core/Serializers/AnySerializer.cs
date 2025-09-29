//-----------------------------------------------------------------------
// <copyright file="AnySerializer.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using OdinSerializer.Utilities;

namespace OdinSerializer
{
    public sealed class AnySerializer : Serializer
    {
        private static readonly ISerializationPolicy UnityPolicy = SerializationPolicies.Unity;
        private static readonly ISerializationPolicy StrictPolicy = SerializationPolicies.Strict;
        private static readonly ISerializationPolicy EverythingPolicy = SerializationPolicies.Everything;

        private readonly bool AllowDeserializeInvalidData;

        private readonly Dictionary<ISerializationPolicy, IFormatter> FormattersByPolicy =
            new(ReferenceEqualityComparer<ISerializationPolicy>.Default);

        private readonly object FormattersByPolicy_LOCK = new();
        private readonly bool IsAbstract;
        private readonly bool IsEnum;
        private readonly bool IsNullable;
        private readonly bool IsValueType;
        private readonly bool MayBeBoxedValueType;

        private readonly Type SerializedType;
        private IFormatter EverythingPolicyFormatter;
        private IFormatter StrictPolicyFormatter;

        private IFormatter UnityPolicyFormatter;

        public AnySerializer(Type serializedType)
        {
            SerializedType = serializedType;
            IsEnum = SerializedType.IsEnum;
            IsValueType = SerializedType.IsValueType;
            MayBeBoxedValueType = SerializedType.IsInterface || SerializedType == typeof(object) ||
                                  SerializedType == typeof(ValueType) || SerializedType == typeof(Enum);
            IsAbstract = SerializedType.IsAbstract || SerializedType.IsInterface;
            IsNullable = SerializedType.IsGenericType &&
                         SerializedType.GetGenericTypeDefinition() == typeof(Nullable<>);
            AllowDeserializeInvalidData = SerializedType.IsDefined(typeof(AllowDeserializeInvalidDataAttribute), true);
        }

        public override object ReadValueWeak(IDataReader reader)
        {
            if (IsEnum)
            {
                string name;
                EntryType entry = reader.PeekEntry(out name);

                if (entry == EntryType.Integer)
                {
                    ulong value;
                    if (reader.ReadUInt64(out value) == false)
                    {
                        reader.Context.Config.DebugContext.LogWarning("Failed to read entry '" + name + "' of type " +
                                                                      entry);
                    }

                    return Enum.ToObject(SerializedType, value);
                }

                reader.Context.Config.DebugContext.LogWarning("Expected entry of type " + EntryType.Integer +
                                                              ", but got entry '" + name + "' of type " + entry);
                reader.SkipEntry();
                return Activator.CreateInstance(SerializedType);
            }
            else
            {
                DeserializationContext context = reader.Context;

                if (context.Config.SerializationPolicy.AllowNonSerializableTypes == false &&
                    SerializedType.IsSerializable == false)
                {
                    context.Config.DebugContext.LogError("The type " + SerializedType.Name +
                                                         " is not marked as serializable.");
                    return IsValueType ? Activator.CreateInstance(SerializedType) : null;
                }

                var exitNode = true;

                string name;
                EntryType entry = reader.PeekEntry(out name);

                if (IsValueType)
                {
                    if (entry == EntryType.Null)
                    {
                        context.Config.DebugContext.LogWarning("Expecting complex struct of type " +
                                                               SerializedType.GetNiceFullName() +
                                                               " but got null value.");
                        reader.ReadNull();
                        return Activator.CreateInstance(SerializedType);
                    }

                    if (entry != EntryType.StartOfNode)
                    {
                        context.Config.DebugContext.LogWarning("Unexpected entry '" + name + "' of type " + entry +
                                                               ", when " + EntryType.StartOfNode +
                                                               " was expected. A value has likely been lost.");
                        reader.SkipEntry();
                        return Activator.CreateInstance(SerializedType);
                    }

                    try
                    {
                        Type expectedType = SerializedType;
                        Type serializedType;

                        if (reader.EnterNode(out serializedType))
                        {
                            if (serializedType != expectedType)
                            {
                                if (serializedType != null)
                                {
                                    context.Config.DebugContext.LogWarning("Expected complex struct value " +
                                                                           expectedType.Name +
                                                                           " but the serialized value is of type " +
                                                                           serializedType.Name + ".");

                                    if (serializedType.IsCastableTo(expectedType))
                                    {
                                        var value = FormatterLocator
                                            .GetFormatter(serializedType, context.Config.SerializationPolicy)
                                            .Deserialize(reader);

                                        var serializedTypeIsNullable = serializedType.IsGenericType &&
                                                                       serializedType.GetGenericTypeDefinition() ==
                                                                       typeof(Nullable<>);
                                        var allowCastMethod = !IsNullable && !serializedTypeIsNullable;

                                        Func<object, object> castMethod = allowCastMethod
                                            ? serializedType.GetCastMethodDelegate(expectedType)
                                            : null;

                                        if (castMethod != null)
                                        {
                                            return castMethod(value);
                                        }
                                        else
                                        {
                                            return value;
                                        }
                                    }
                                    else if (AllowDeserializeInvalidData ||
                                             reader.Context.Config.AllowDeserializeInvalidData)
                                    {
                                        context.Config.DebugContext.LogWarning("Can't cast serialized type " +
                                                                               serializedType.GetNiceFullName() +
                                                                               " into expected type " +
                                                                               expectedType.GetNiceFullName() +
                                                                               ". Attempting to deserialize with possibly invalid data. Value may be lost or corrupted for node '" +
                                                                               name + "'.");
                                        return GetBaseFormatter(context.Config.SerializationPolicy).Deserialize(reader);
                                    }
                                    else
                                    {
                                        context.Config.DebugContext.LogWarning("Can't cast serialized type " +
                                                                               serializedType.GetNiceFullName() +
                                                                               " into expected type " +
                                                                               expectedType.GetNiceFullName() +
                                                                               ". Value lost for node '" + name + "'.");
                                        return Activator.CreateInstance(SerializedType);
                                    }
                                }
                                else if (AllowDeserializeInvalidData ||
                                         reader.Context.Config.AllowDeserializeInvalidData)
                                {
                                    context.Config.DebugContext.LogWarning("Expected complex struct value " +
                                                                           expectedType.GetNiceFullName() +
                                                                           " but the serialized type could not be resolved. Attempting to deserialize with possibly invalid data. Value may be lost or corrupted for node '" +
                                                                           name + "'.");
                                    return GetBaseFormatter(context.Config.SerializationPolicy).Deserialize(reader);
                                }
                                else
                                {
                                    context.Config.DebugContext.LogWarning("Expected complex struct value " +
                                                                           expectedType.Name +
                                                                           " but the serialized type could not be resolved. Value lost for node '" +
                                                                           name + "'.");
                                    return Activator.CreateInstance(SerializedType);
                                }
                            }
                            else
                            {
                                return GetBaseFormatter(context.Config.SerializationPolicy).Deserialize(reader);
                            }
                        }
                        else
                        {
                            context.Config.DebugContext.LogError("Failed to enter node '" + name + "'.");
                            return Activator.CreateInstance(SerializedType);
                        }
                    }
                    catch (SerializationAbortException ex)
                    {
                        exitNode = false;
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        context.Config.DebugContext.LogException(ex);
                        return Activator.CreateInstance(SerializedType);
                    }
                    finally
                    {
                        if (exitNode)
                        {
                            reader.ExitNode();
                        }
                    }
                }

                switch (entry)
                {
                    case EntryType.Null:
                    {
                        reader.ReadNull();
                        return null;
                    }

                    case EntryType.ExternalReferenceByIndex:
                    {
                        int index;
                        reader.ReadExternalReference(out index);

                        var value = context.GetExternalObject(index);

                        if (!ReferenceEquals(value, null) && !SerializedType.IsAssignableFrom(value.GetType()))
                        {
                            context.Config.DebugContext.LogWarning("Can't cast external reference type " +
                                                                   value.GetType().GetNiceFullName() +
                                                                   " into expected type " +
                                                                   SerializedType.GetNiceFullName() +
                                                                   ". Value lost for node '" + name + "'.");
                            return null;
                        }

                        return value;
                    }

                    case EntryType.ExternalReferenceByGuid:
                    {
                        Guid guid;
                        reader.ReadExternalReference(out guid);

                        var value = context.GetExternalObject(guid);

                        if (!ReferenceEquals(value, null) && !SerializedType.IsAssignableFrom(value.GetType()))
                        {
                            context.Config.DebugContext.LogWarning("Can't cast external reference type " +
                                                                   value.GetType().GetNiceFullName() +
                                                                   " into expected type " +
                                                                   SerializedType.GetNiceFullName() +
                                                                   ". Value lost for node '" + name + "'.");
                            return null;
                        }

                        return value;
                    }

                    case EntryType.ExternalReferenceByString:
                    {
                        string id;
                        reader.ReadExternalReference(out id);

                        var value = context.GetExternalObject(id);

                        if (!ReferenceEquals(value, null) && !SerializedType.IsAssignableFrom(value.GetType()))
                        {
                            context.Config.DebugContext.LogWarning("Can't cast external reference type " +
                                                                   value.GetType().GetNiceFullName() +
                                                                   " into expected type " +
                                                                   SerializedType.GetNiceFullName() +
                                                                   ". Value lost for node '" + name + "'.");
                            return null;
                        }

                        return value;
                    }

                    case EntryType.InternalReference:
                    {
                        int id;
                        reader.ReadInternalReference(out id);

                        var value = context.GetInternalReference(id);

                        if (!ReferenceEquals(value, null) && !SerializedType.IsAssignableFrom(value.GetType()))
                        {
                            context.Config.DebugContext.LogWarning("Can't cast internal reference type " +
                                                                   value.GetType().GetNiceFullName() +
                                                                   " into expected type " +
                                                                   SerializedType.GetNiceFullName() +
                                                                   ". Value lost for node '" + name + "'.");
                            return null;
                        }

                        return value;
                    }

                    case EntryType.StartOfNode:
                    {
                        try
                        {
                            Type expectedType = SerializedType;
                            Type serializedType;
                            int id;

                            if (reader.EnterNode(out serializedType))
                            {
                                id = reader.CurrentNodeId;

                                object result;

                                if (serializedType != null &&
                                    expectedType !=
                                    serializedType) // We have type metadata different from the expected type
                                {
                                    var success = false;
                                    var isPrimitive = FormatterUtilities.IsPrimitiveType(serializedType);

                                    bool assignableCast;

                                    if (MayBeBoxedValueType && isPrimitive)
                                    {
                                        // It's a boxed primitive type, so simply read that straight and register success
                                        Serializer serializer = Get(serializedType);
                                        result = serializer.ReadValueWeak(reader);
                                        success = true;
                                    }
                                    else if ((assignableCast = expectedType.IsAssignableFrom(serializedType)) ||
                                             serializedType.HasCastDefined(expectedType, false))
                                    {
                                        try
                                        {
                                            object value;

                                            if (isPrimitive)
                                            {
                                                Serializer serializer = Get(serializedType);
                                                value = serializer.ReadValueWeak(reader);
                                            }
                                            else
                                            {
                                                IFormatter alternateFormatter = FormatterLocator.GetFormatter(
                                                    serializedType,
                                                    context.Config.SerializationPolicy);
                                                value = alternateFormatter.Deserialize(reader);
                                            }

                                            if (assignableCast)
                                            {
                                                result = value;
                                            }
                                            else
                                            {
                                                Func<object, object> castMethod =
                                                    serializedType.GetCastMethodDelegate(expectedType);

                                                if (castMethod != null)
                                                {
                                                    result = castMethod(value);
                                                }
                                                else
                                                    // Let's just give it a go anyways
                                                {
                                                    result = value;
                                                }
                                            }

                                            success = true;
                                        }
                                        catch (SerializationAbortException ex)
                                        {
                                            exitNode = false;
                                            throw ex;
                                        }
                                        catch (InvalidCastException)
                                        {
                                            success = false;
                                            result = null;
                                        }
                                    }
                                    else if (!IsAbstract && (AllowDeserializeInvalidData ||
                                                             reader.Context.Config.AllowDeserializeInvalidData))
                                    {
                                        // We will try to deserialize an instance of T with the invalid data.
                                        context.Config.DebugContext.LogWarning("Can't cast serialized type " +
                                                                               serializedType.GetNiceFullName() +
                                                                               " into expected type " +
                                                                               expectedType.GetNiceFullName() +
                                                                               ". Attempting to deserialize with invalid data. Value may be lost or corrupted for node '" +
                                                                               name + "'.");
                                        result = GetBaseFormatter(context.Config.SerializationPolicy)
                                            .Deserialize(reader);
                                        success = true;
                                    }
                                    else
                                    {
                                        // We couldn't cast or use the type, but we still have to deserialize it and register
                                        // the reference so the reference isn't lost if it is referred to further down
                                        // the data stream.

                                        IFormatter alternateFormatter = FormatterLocator.GetFormatter(serializedType,
                                            context.Config.SerializationPolicy);
                                        var value = alternateFormatter.Deserialize(reader);

                                        if (id >= 0)
                                        {
                                            context.RegisterInternalReference(id, value);
                                        }

                                        result = null;
                                    }

                                    if (!success)
                                    {
                                        // We can't use this
                                        context.Config.DebugContext.LogWarning("Can't cast serialized type " +
                                                                               serializedType.GetNiceFullName() +
                                                                               " into expected type " +
                                                                               expectedType.GetNiceFullName() +
                                                                               ". Value lost for node '" + name + "'.");
                                        result = null;
                                    }
                                }
                                else if (IsAbstract)
                                {
                                    result = null;
                                }
                                else
                                {
                                    result = GetBaseFormatter(context.Config.SerializationPolicy).Deserialize(reader);
                                }

                                if (id >= 0)
                                {
                                    context.RegisterInternalReference(id, result);
                                }

                                return result;
                            }
                            else
                            {
                                context.Config.DebugContext.LogError("Failed to enter node '" + name + "'.");
                                return null;
                            }
                        }
                        catch (SerializationAbortException ex)
                        {
                            exitNode = false;
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            context.Config.DebugContext.LogException(ex);
                            return null;
                        }
                        finally
                        {
                            if (exitNode)
                            {
                                reader.ExitNode();
                            }
                        }
                    }

                    //
                    // The below cases are for when we expect an object, but have
                    // serialized a straight primitive type. In such cases, we can
                    // often box the primitive type as an object.
                    //
                    // Sadly, the exact primitive type might be lost in case of
                    // integer and floating points numbers, as we don't know what
                    // type to expect.
                    //
                    // To be safe, we read and box the most precise type available.
                    //

                    case EntryType.Boolean:
                    {
                        if (!MayBeBoxedValueType)
                        {
                            goto default;
                        }

                        bool value;
                        reader.ReadBoolean(out value);
                        return value;
                    }

                    case EntryType.FloatingPoint:
                    {
                        if (!MayBeBoxedValueType)
                        {
                            goto default;
                        }

                        double value;
                        reader.ReadDouble(out value);
                        return value;
                    }

                    case EntryType.Integer:
                    {
                        if (!MayBeBoxedValueType)
                        {
                            goto default;
                        }

                        long value;
                        reader.ReadInt64(out value);
                        return value;
                    }

                    case EntryType.String:
                    {
                        if (!MayBeBoxedValueType)
                        {
                            goto default;
                        }

                        string value;
                        reader.ReadString(out value);
                        return value;
                    }

                    case EntryType.Guid:
                    {
                        if (!MayBeBoxedValueType)
                        {
                            goto default;
                        }

                        Guid value;
                        reader.ReadGuid(out value);
                        return value;
                    }

                    default:

                        // Lost value somehow
                        context.Config.DebugContext.LogWarning("Unexpected entry of type " + entry +
                                                               ", when a reference or node start was expected. A value has been lost.");
                        reader.SkipEntry();
                        return null;
                }
            }
        }

        public override void WriteValueWeak(string name, object value, IDataWriter writer)
        {
            if (IsEnum)
            {
                // Copied from EnumSerializer.cs
                ulong ul;

                FireOnSerializedType(SerializedType);

                try
                {
                    ul = Convert.ToUInt64(value as Enum);
                }
                catch (OverflowException)
                {
                    unchecked
                    {
                        ul = (ulong)Convert.ToInt64(value as Enum);
                    }
                }

                writer.WriteUInt64(name, ul);
            }
            else
            {
                // Copied from ComplexTypeSerializer.cs
                SerializationContext context = writer.Context;
                ISerializationPolicy policy = context.Config.SerializationPolicy;

                if (policy.AllowNonSerializableTypes == false && SerializedType.IsSerializable == false)
                {
                    context.Config.DebugContext.LogError("The type " + SerializedType.Name +
                                                         " is not marked as serializable.");
                    return;
                }

                FireOnSerializedType(SerializedType);

                if (IsValueType)
                {
                    var endNode = true;

                    try
                    {
                        writer.BeginStructNode(name, SerializedType);
                        GetBaseFormatter(policy).Serialize(value, writer);
                    }
                    catch (SerializationAbortException ex)
                    {
                        endNode = false;
                        throw ex;
                    }
                    finally
                    {
                        if (endNode)
                        {
                            writer.EndNode(name);
                        }
                    }
                }
                else
                {
                    int id;
                    int index;
                    string strId;
                    Guid guid;

                    var endNode = true;

                    if (ReferenceEquals(value, null))
                    {
                        writer.WriteNull(name);
                    }
                    else if (context.TryRegisterExternalReference(value, out index))
                    {
                        writer.WriteExternalReference(name, index);
                    }
                    else if (context.TryRegisterExternalReference(value, out guid))
                    {
                        writer.WriteExternalReference(name, guid);
                    }
                    else if (context.TryRegisterExternalReference(value, out strId))
                    {
                        writer.WriteExternalReference(name, strId);
                    }
                    else if (context.TryRegisterInternalReference(value, out id))
                    {
                        // Get type of actual stored object
                        //
                        // Don't have it as a strongly typed T value, since people can "override" (shadow)
                        // GetType() on derived classes with the "new" operator. By referencing the type
                        // as a System.Object, we ensure the correct GetType() method is always called.
                        //
                        // (Yes, this has actually happened, and this was done to fix it.)

                        Type type = value.GetType();

                        if (MayBeBoxedValueType && FormatterUtilities.IsPrimitiveType(type))
                            // It's a boxed primitive type
                        {
                            try
                            {
                                writer.BeginReferenceNode(name, type, id);

                                Serializer serializer = Get(type);
                                serializer.WriteValueWeak(value, writer);
                            }
                            catch (SerializationAbortException ex)
                            {
                                endNode = false;
                                throw ex;
                            }
                            finally
                            {
                                if (endNode)
                                {
                                    writer.EndNode(name);
                                }
                            }
                        }
                        else
                        {
                            IFormatter formatter;

                            if (ReferenceEquals(type, SerializedType))
                            {
                                formatter = GetBaseFormatter(policy);
                            }
                            else
                            {
                                formatter = FormatterLocator.GetFormatter(type, policy);
                            }

                            try
                            {
                                writer.BeginReferenceNode(name, type, id);
                                formatter.Serialize(value, writer);
                            }
                            catch (SerializationAbortException ex)
                            {
                                endNode = false;
                                throw ex;
                            }
                            finally
                            {
                                if (endNode)
                                {
                                    writer.EndNode(name);
                                }
                            }
                        }
                    }
                    else
                    {
                        writer.WriteInternalReference(name, id);
                    }
                }
            }
        }

        private IFormatter GetBaseFormatter(ISerializationPolicy serializationPolicy)
        {
            // This is an optimization - it's a lot cheaper to compare three references and do a null check,
            //  than it is to look something up in a dictionary. By far most of the time, we will be using
            //  one of these three policies.

            if (ReferenceEquals(serializationPolicy, UnityPolicy))
            {
                if (UnityPolicyFormatter == null)
                {
                    UnityPolicyFormatter = FormatterLocator.GetFormatter(SerializedType, UnityPolicy);
                }

                return UnityPolicyFormatter;
            }

            if (ReferenceEquals(serializationPolicy, EverythingPolicy))
            {
                if (EverythingPolicyFormatter == null)
                {
                    EverythingPolicyFormatter = FormatterLocator.GetFormatter(SerializedType, EverythingPolicy);
                }

                return EverythingPolicyFormatter;
            }

            if (ReferenceEquals(serializationPolicy, StrictPolicy))
            {
                if (StrictPolicyFormatter == null)
                {
                    StrictPolicyFormatter = FormatterLocator.GetFormatter(SerializedType, StrictPolicy);
                }

                return StrictPolicyFormatter;
            }

            IFormatter formatter;

            lock (FormattersByPolicy_LOCK)
            {
                if (!FormattersByPolicy.TryGetValue(serializationPolicy, out formatter))
                {
                    formatter = FormatterLocator.GetFormatter(SerializedType, serializationPolicy);
                    FormattersByPolicy.Add(serializationPolicy, formatter);
                }
            }

            return formatter;
        }
    }
}
