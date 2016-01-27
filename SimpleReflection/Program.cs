using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace idv.CircleHsiao.SimpleReflection
{
    class Program
    {
        static void Main()
        {
            A a = new A
            {
                Prop1 = 1,
                Prop2 = "2",
                Prop3 = new Person
                {
                    Age = 18,
                    Msg = "King"
                },
                Prop4 = DateTime.Now,
                Prop5 = false
            };

            B b = new B();
            b = CopyProperties<A, B>(a, b);

            a.Prop4 = a.Prop4.AddMinutes(1);

            b.Prop3.Msg = "Edited";

            B b2 = (B)b.Clone();
            b2.Prop3.Msg = "for C";
            b2.Prop2 = "C";

            C c = new C();
            c = CopyProperties<A, C>(a, c);

        }

        public static B CopyProperties<A, B>(A a, B b)
        {
            var typeOfA = a.GetType();
            var typeOfB = b.GetType();

            // copy properties
            foreach (var propertyOfA in typeOfA.GetProperties())
            {
                try
                {
                    var propertyOfB = typeOfB.GetProperty(propertyOfA.Name);
                    if (propertyOfB != null)
                    {
                        if (propertyOfA.PropertyType.Namespace == "System")
                            propertyOfB.SetValue(b, propertyOfA.GetValue(a));
                        else
                        {
                            if (b is ICloneable)
                                propertyOfB.SetValue(b, ((ICloneable)propertyOfA.GetValue(a)).Clone());
                            else
                                throw new Exception(string.Format(
                                    "Property {0} is customer class but didn't implement ICloneable.", 
                                    propertyOfB.Name));
                        }
                        
                    }
                    else
                        throw new Exception(string.Format(
                            "Target class didn't have Property {0}.", 
                            propertyOfA.Name));

                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }

            return b;

        }
    }

    class A
    {
        public int Prop1 { get; set; }
        public string Prop2 { get; set; }
        public Person Prop3 { get; set; }
        public DateTime Prop4 { get; set; }
        public bool Prop5 { get; set; }

        public A()
        {

        }
    }

    class B : ICloneable
    {
        public int Prop1 { get; set; }
        public string Prop2 { get; set; }
        public Person Prop3 { get; set; }
        public DateTime Prop4 { get; set; }

        public B()
        {

        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    class C : ICloneable
    {
        public int Prop1 { get; set; }
        public string Prop2 { get; set; }
        public Person Prop3 { get; set; }
        public DateTime Prop4 { get { return DateTime.Now; } }
        //public bool Prop5 { get; set; }

        public C()
        {

        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    class Person : ICloneable
    {
        public int Age { get; set; }
        public string Msg { get; set; }

        public Person()
        {
        }

        public object Clone()
        {
            return new Person()
            {
                Age = Age,
                Msg = Msg
            };
        }
    }
}
