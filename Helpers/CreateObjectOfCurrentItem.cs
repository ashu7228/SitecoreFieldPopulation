using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace HappyCoaching.Helpers
{
    public class CreateObjectOfCurrentItem
    {
        public static Type CreateDynamicType(IEnumerable<string> fieldNames)
        {
            AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicType", TypeAttributes.Public);

            foreach (string fieldName in fieldNames)
            {
                FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + fieldName, typeof(string), FieldAttributes.Private);
                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(fieldName, PropertyAttributes.HasDefault, typeof(string), null);
                MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + fieldName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(string), Type.EmptyTypes);
                ILGenerator getIl = getPropMthdBldr.GetILGenerator();
                getIl.Emit(OpCodes.Ldarg_0);
                getIl.Emit(OpCodes.Ldfld, fieldBuilder);
                getIl.Emit(OpCodes.Ret);

                MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + fieldName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { typeof(string) });
                ILGenerator setIl = setPropMthdBldr.GetILGenerator();
                setIl.Emit(OpCodes.Ldarg_0);
                setIl.Emit(OpCodes.Ldarg_1);
                setIl.Emit(OpCodes.Stfld, fieldBuilder);
                setIl.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getPropMthdBldr);
                propertyBuilder.SetSetMethod(setPropMthdBldr);
            }

            return typeBuilder.CreateTypeInfo().AsType();
        }
    }
}