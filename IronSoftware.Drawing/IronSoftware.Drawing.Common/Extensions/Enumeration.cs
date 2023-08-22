using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace IronSoftware.Drawing.Extensions
{
    ///<exclude/>
    [Browsable(false)]
    [Bindable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class Enumeration : IComparable
    {
        ///<exclude/>
        public string Name { get; private set; }
        ///<exclude/>
        public int Id { get; private set; }

        internal Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }
        ///<exclude/>
        public override string ToString() => Name;
        ///<exclude/>
        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .Cast<T>();
        ///<exclude/>
        public override bool Equals(object obj)
        {
            if (obj is Enumeration otherValue)
            {
                var typeMatches = GetType().Equals(obj.GetType());
                var valueMatches = Id.Equals(otherValue.Id);

                return typeMatches && valueMatches;
            }
            else
            {
                return false;
            }
        }
        ///<exclude/>
        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
        ///<exclude/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // Other utility methods ...
    }
}