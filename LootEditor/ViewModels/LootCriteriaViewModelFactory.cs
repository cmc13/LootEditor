using LootEditor.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LootEditor.ViewModels;

public static class LootCriteriaViewModelFactory
{
    private static readonly Dictionary<Type, Type> typedict = [];

    public static LootCriteriaViewModel CreateViewModel(LootCriteria criteria)
    {
        var vmType = criteria.GetType();
        if (!typedict.TryGetValue(vmType, out var dynamicType))
        {
            // generate new type 
            // Create everything required to get a module builder
            var assemblyName = new AssemblyName("LootEditor.DynamicViewModel");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            //AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            string dynamicTypeName = Assembly.CreateQualifiedName(vmType.AssemblyQualifiedName, vmType.Name+"DynamicViewModel");

            var typeBuilder = moduleBuilder.DefineType(dynamicTypeName, TypeAttributes.Public | TypeAttributes.Class, typeof(LootCriteriaViewModel));

            MethodInfo raisePropertyChangedMethod = typeof(ObservableRecipient).GetMethod("OnPropertyChanged",
                BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(string)], null);

            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard | CallingConventions.HasThis,
                [typeof(LootCriteria)]);
            var baseConstructor = typeof(LootCriteriaViewModel).GetConstructors()[0];
            var baseParam = baseConstructor.GetParameters()[0];

            var param = constructorBuilder.DefineParameter(1, baseParam.Attributes, baseParam.Name);

            var generator = constructorBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg, 1);
            generator.Emit(OpCodes.Call, baseConstructor);
            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ret);

            foreach (PropertyInfo propertyInfo in vmType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && typeof(LootCriteriaViewModel).GetProperty(p.Name) == null))
            {        // Update the setter of the class
                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyInfo.Name,
                    PropertyAttributes.None, propertyInfo.PropertyType, null);

                // Create get method
                MethodBuilder builder = typeBuilder.DefineMethod("get_" + propertyInfo.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    propertyInfo.PropertyType, TypeInfo.EmptyTypes);
                generator = builder.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Call, typeof(LootCriteriaViewModel).GetProperty(nameof(LootCriteriaViewModel.Criteria)).GetGetMethod());
                generator.Emit(OpCodes.Castclass, vmType);
                generator.Emit(OpCodes.Callvirt, propertyInfo.GetGetMethod());
                generator.Emit(OpCodes.Ret);
                propertyBuilder.SetGetMethod(builder);

                // Create set method
                builder = typeBuilder.DefineMethod("set_" + propertyInfo.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual, null, [propertyInfo.PropertyType]);
                builder.DefineParameter(1, ParameterAttributes.None, "value");
                generator = builder.GetILGenerator();

                // Set value
                generator.Emit(OpCodes.Nop);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Call, typeof(LootCriteriaViewModel).GetProperty(nameof(LootCriteriaViewModel.Criteria)).GetGetMethod());
                generator.Emit(OpCodes.Castclass, vmType);
                generator.Emit(OpCodes.Ldarg, 1);
                generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                generator.Emit(OpCodes.Nop);

                // Raise property changed
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, propertyInfo.Name);
                generator.Emit(OpCodes.Callvirt, raisePropertyChangedMethod);
                generator.Emit(OpCodes.Nop);

                // Raise property changed for display value
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, nameof(LootCriteriaViewModel.DisplayValue));
                generator.Emit(OpCodes.Callvirt, raisePropertyChangedMethod);
                generator.Emit(OpCodes.Nop);

                // Set IsDirty flag on VM
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4_1);
                generator.Emit(OpCodes.Call, typeof(LootCriteriaViewModel).GetProperty(nameof(LootCriteriaViewModel.IsDirty)).GetSetMethod());
                generator.Emit(OpCodes.Nop);
                generator.Emit(OpCodes.Nop);
                
                generator.Emit(OpCodes.Ret);

                propertyBuilder.SetSetMethod(builder);
            }

            dynamicType = typeBuilder.CreateType();
            typedict.Add(vmType, dynamicType);
        }

        return (LootCriteriaViewModel)Activator.CreateInstance(dynamicType, criteria);
    }
}
