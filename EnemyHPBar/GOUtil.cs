using System;
using System.Collections.Generic;
using System.Text;

namespace SSEnemyHPBar
{
    public static class GOUtil
    {
        public static T GetAddComponent<T>(this GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }
            return comp;
        }
    }
}
