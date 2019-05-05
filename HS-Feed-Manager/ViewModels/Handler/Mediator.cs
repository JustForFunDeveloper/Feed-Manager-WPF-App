using System;
using System.Collections.Generic;
using HS_Feed_Manager.Core.Handler;

namespace HS_Feed_Manager.ViewModels.Handler
{
    static public class Mediator
    {
        static IDictionary<string, List<Action<object>>> pl_dict = new Dictionary<string, List<Action<object>>>();

        static public void Register(string token, Action<object> callback)
        {
            try
            {
                if (!pl_dict.ContainsKey(token))
                {
                    var list = new List<Action<object>>();
                    list.Add(callback);
                    pl_dict.Add(token, list);
                }
                else
                {
                    if (!pl_dict[token].Contains(callback))
                    {
                        pl_dict[token].Add(callback);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("Register: " + ex, LogLevel.Error);
            }
        }

        static public void Unregister(string token, Action<object> callback)
        {
            try
            {
                if (pl_dict.ContainsKey(token))
                    pl_dict[token].Remove(callback);
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("Unregister: " + ex, LogLevel.Error);
            }
        }

        static public void NotifyColleagues(string token, object args)
        {
            try
            {
                if (pl_dict.ContainsKey(token))
                    foreach (var callback in pl_dict[token])
                        callback(args);
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("NotifyColleagues: " + ex, LogLevel.Error);
            }
        }
    }
}
