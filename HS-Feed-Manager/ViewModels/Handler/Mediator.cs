using System;
using System.Collections.Generic;
using Serilog;

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
                Log.Error(ex,"Register Error!");
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
                Log.Error(ex,"Unregister Error!");
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
                Log.Error(ex,"NotifyColleagues Error!");
            }
        }
    }
}
