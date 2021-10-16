using System;

namespace HS_Feed_Manager.DataModels.DbModels
{
    public class SqLiteLog
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Level { get; set; }
        public string Exception { get; set; }
        public string RenderedMessage { get; set; }
    }
}