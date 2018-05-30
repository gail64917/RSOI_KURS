using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitDLL
{
    public class UserPreferences
    {
        public int ID { get; set; }
        public Dictionary<Category, int> UserPrefer = new Dictionary<Category, int>();
    }
}
