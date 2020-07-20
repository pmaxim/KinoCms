using System;

namespace KinoCMS.Server.Infrastructure
{
    public static class DynamicToStatic
    {
        public static T ToType<T>(object obj)
        {
            var tmp = Activator.CreateInstance<T>();
            var propList = obj.GetType().GetProperties();
            foreach (var pi in propList)
            {
                try
                {
                    tmp.GetType().GetProperty(pi.Name)
                        ?.SetValue(tmp,
                        pi.GetValue(obj, null), null);
                }
                catch
                {
                    // ignored
                }
            }       
            return tmp;
        }
    }
}
