using LootEditor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace LootEditor.View.ViewModel
{
    public static class LootCriteriaViewModelFactory
    {
        private static Dictionary<Type, Type> dynamicTypes = new Dictionary<Type, Type>();

        public static LootCriteriaViewModel CreateViewModel(LootCriteria criteria)
        {
            var vmType = criteria.GetType();
            if (!dynamicTypes.TryGetValue(vmType, out var dynamicType))
            {
                var assemblyName = new AssemblyName("LootEditor.View.ViewModel.Dynamic");
                var domain = AppDomain.CurrentDomain;
                var assemblyBuilder = domain.DefineDynamicAssembly(assemblyName, System.Reflection.Emit.AssemblyBuilderAccess.Run);
                var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

                var dynamicTypeName = Assembly.CreateQualifiedName(vmType.AssemblyQualifiedName, vmType.Name + "ViewModel");

                var typeBuilder = moduleBuilder.DefineType(dynamicTypeName, TypeAttributes.Public | TypeAttributes.Class, typeof(LootCriteriaViewModel));

                var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard | CallingConventions.HasThis, new[] { typeof(LootCriteria) });
                var baseCtor = typeof(LootCriteriaViewModel).GetConstructors()[0];
                var param = baseCtor.GetParameters()[0];
                ctor.DefineParameter(1, param.Attributes, param.Name);
                var gen = ctor.GetILGenerator();
                gen.Emit(OpCodes.Nop);
                gen.Emit(OpCodes.Ldarg_0); // load this
                gen.Emit(OpCodes.Ldarg, 1); // load param
                gen.Emit(OpCodes.Call, baseCtor); // call base constructor
                gen.Emit(OpCodes.Ret);

                var raisePropertyChangedMethod = typeof(LootCriteriaViewModel).GetMethod("RaisePropertyChanged", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, null);

                foreach (var prop in vmType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (prop.CanWrite && typeof(LootCriteriaViewModel).GetProperty(prop.Name) == null)
                    {
                        // Update the setter of the class
                        var propertyBuilder = typeBuilder.DefineProperty(prop.Name, PropertyAttributes.None, prop.PropertyType, null);

                        var builder = typeBuilder.DefineMethod("get_" + prop.Name, MethodAttributes.Public | MethodAttributes.Virtual, prop.PropertyType, Type.EmptyTypes);
                        var generator = builder.GetILGenerator();
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Call, prop.GetGetMethod());
                        generator.Emit(OpCodes.Ret);
                        propertyBuilder.SetGetMethod(builder);

                        // Create set method
                        builder = typeBuilder.DefineMethod("set_" + prop.Name,
                            MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[] { prop.PropertyType });
                        builder.DefineParameter(1, ParameterAttributes.None, "value");
                        generator = builder.GetILGenerator();

                        // Add IL code for set method
                        generator.Emit(OpCodes.Nop);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldarg_1);
                        generator.Emit(OpCodes.Call, prop.GetSetMethod());

                        // Call property changed for object
                        generator.Emit(OpCodes.Nop);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldstr, prop.Name);
                        generator.Emit(OpCodes.Callvirt, raisePropertyChangedMethod);
                        generator.Emit(OpCodes.Nop);
                        generator.Emit(OpCodes.Ret);
                        propertyBuilder.SetSetMethod(builder);
                    }
                }

                dynamicType = typeBuilder.CreateType();
                dynamicTypes.Add(vmType, dynamicType);
            }

            return (LootCriteriaViewModel)Activator.CreateInstance(dynamicType, new[] { criteria });
        }
    }
}
