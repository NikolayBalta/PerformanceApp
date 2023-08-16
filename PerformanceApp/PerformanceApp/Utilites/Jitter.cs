using System.Reflection;
using System.Runtime.CompilerServices;

namespace PerformanceApp.Utilites
{
    public class Jitter
    {
        public static void PreJit(object instance)
        {
            PreJitMarkedMethods(instance.GetType());
        }


        public static void PreJitAll(object instance)
        {
            PreJitAllMethods(instance.GetType());
        }

        public static void BeginPreJitAll(object instance)
        {
            Thread preJitThread = new Thread(() =>
            {
                PreJitAllMethods(instance.GetType());
            });

            preJitThread.Name = "PreJittingThread";
            preJitThread.Priority = ThreadPriority.Lowest;
            preJitThread.Start();

        }

        public static void PreJit<T>() where T : class
        {
            PreJitMarkedMethods(typeof(T));
        }


        public static void PreJitAll<T>() where T : class
        {
            PreJitAllMethods(typeof(T));
        }

        public static void BeginPreJitAll<T>() where T : class
        {
            Thread preJitThread = new Thread(() =>
            {
                PreJitAllMethods(typeof(T));
            });

            preJitThread.Name = "PreJittingThread";
            preJitThread.Priority = ThreadPriority.Lowest;
            preJitThread.Start();
        }

        public static void PreJitAll(Assembly assembly)
        {
            var classes = assembly.GetTypes();
            foreach (var classType in classes)
            {
                PreJitAllMethods(classType);
            }
        }

        public static void BeginPreJitAll(Assembly assembly)
        {
            Thread preJitThread = new Thread(() =>
            {
                PreJitAll(assembly);
            });

            preJitThread.Name = "PreJittingThread";
            preJitThread.Priority = ThreadPriority.Lowest;
            preJitThread.Start();
        }


        public static void PreJit(Assembly assembly)
        {
            var classes = assembly.GetTypes();
            foreach (var classType in classes)
            {
                PreJitMarkedMethods(classType);
            }
        }


        public static void BeginPreJit(Assembly assembly)
        {
            Thread preJitThread = new Thread(() =>
            {
                PreJit(assembly);
            });

            preJitThread.Name = "PreJittingThread";
            preJitThread.Priority = ThreadPriority.Lowest;
            preJitThread.Start();
        }

        public static void BeginPreJit(object instance)
        {
            Thread preJitThread = new Thread(() =>
            {
                PreJit(instance);
            });

            preJitThread.Name = "PreJittingThread";
            preJitThread.Priority = ThreadPriority.Lowest;
            preJitThread.Start();
        }

        private static void PreJitMarkedMethods(Type type)
        {
            // get the type of all the methods within this instance
            var methods = type.GetMethods(BindingFlags.DeclaredOnly |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Public |
                                        BindingFlags.Instance |
                                        BindingFlags.Static);

            // for each time, jit methods marked with prejit attribute
            foreach (var method in methods)
            {
                if (ContainsPreJitAttribute(method))
                {
                    // jitting of the method happends here.
                    RuntimeHelpers.PrepareMethod(method.MethodHandle);
                }
            }
        }


        private static void PreJitAllMethods(Type type)
        {
            // get the type of all the methods within this instance
            var methods = type.GetMethods(BindingFlags.DeclaredOnly |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Public |
                                        BindingFlags.Instance |
                                        BindingFlags.Static);

            // Jit all methods
            foreach (var method in methods)
            {
                // jitting of the method happends here.
                RuntimeHelpers.PrepareMethod(method.MethodHandle);
            }
        }


        private static bool ContainsPreJitAttribute(MethodInfo methodInfo)
        {
            var attributes = methodInfo.GetCustomAttributes(typeof(PreJitAttribute), false);
            if (attributes == null) return false;
            return attributes.Length > 0;
        }
    }
}
