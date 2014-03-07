using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTrees
{

    class SwapVisitor : ExpressionVisitor
    {
        private readonly Expression _from, _to;

        public SwapVisitor(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }

        public override Expression Visit(Expression node)
        {
            return node == _from ? _to : base.Visit(node);
        }
    }

    class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    class Program
    {
        public static List<Person> Names;

        static void Main()
        {
            Names = new List<Person>();
            Names.Add(new Person {Id = 1, FirstName = "Alexander", LastName = "Williamson"});
            Names.Add(new Person { Id = 2, FirstName = "Anitta", LastName = "John" });
            Names.Add(new Person { Id = 2, FirstName = "Ian", LastName = "Wermerling" });

            Expression<Func<Person, bool>> expression1 = person => person.LastName == "Williamson";
            Expression<Func<Person, bool>> expression2 = person => person.LastName == "John";

            var orLambda = Expression.Lambda<Func<Person, bool>> (Expression.OrElse( 
                new SwapVisitor(expression1.Parameters[0], expression2.Parameters[0]).Visit(expression1.Body), expression2.Body), expression2.Parameters);

            var compiled = orLambda.Compile();
            var results = Names.Where(compiled).ToList();

            foreach(var element in results)
                Console.WriteLine("Id: {0}, Name: {1} {2}", element.Id, element.FirstName, element.LastName);

            Console.WriteLine("Found: {0} items", results.Count());
            Console.ReadKey();
        }
    }
}
